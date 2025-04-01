using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SailwindModVersionChecker
{
    internal class VersionChecker
    {
        internal static List<string> websites = new List<string>();

        static readonly string githubAPI = "https://api.github.com/repos/";
        static readonly string thunderstoreAPI = "https://thunderstore.io/api/experimental/package/";
        static readonly string githubWebsite = "https://github.com/";
        static readonly string thunderstoreWebsite = "https://thunderstore.io/c/sailwind/p/";

        internal static async void Check(Dictionary<string, PluginInfo> pluginInfos)
        {
            var updates = "";
            foreach (var plugin in pluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                var version = metadata.Version.ToString();

                var mvcConfigPath = Path.Combine(Path.GetDirectoryName(plugin.Value.Location), "About", "mvc.json");
                if (!File.Exists(mvcConfigPath))
                    continue;

                var url = GetUrl(mvcConfigPath);
                if (url == null)
                    continue;

                var latestVersion = await GetLatestAsync(url);
                if (latestVersion == null)
                    continue;

                var vCurrent =  new Version(version);
                var vLatest = new Version(latestVersion);

                if (vCurrent.CompareTo(vLatest) < 0)
                {
                    updates += $"{metadata.Name} {version} → {latestVersion}\n";
                    Plugin.logger.LogInfo($"*Update Available*  {metadata.Name} {version} → {latestVersion}");
                    continue;
                }

                Plugin.logger.LogInfo($"{metadata.Name} is up to date");
            }
            
            if (!updates.Equals("") && Plugin.enableNotification.Value)
                UpdatesUI.ui.SetActive(true);
            UpdatesUI.textMesh.text += updates;
            UpdatesUI.websites = websites;
        }

        internal static async Task<string> GetLatestAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {                    
                    // GitHub API requires a user agent
                    if (url.Contains("github"))
                        client.DefaultRequestHeaders.Add("User-Agent", "C# GitHub API Client");
                    
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject releaseInfo = JObject.Parse(jsonResponse);
                    
                    if (url.Contains("github"))
                        return Regex.Match((string)releaseInfo["tag_name"], @"(\d+\.\d+\.\d+)").Groups[1].Value;
                    if (url.Contains("thunderstore"))
                        return (string)releaseInfo["latest"]["version_number"];
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Plugin.logger.LogError($"Error accessing website API: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                Plugin.logger.LogError($"Error parsing JSON response: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"An unexpected error occurred: {e.Message}");
                return null;
            }
        }

        internal static string GetUrl(string mvcConfigPath)
        {
            try
            {
                JObject mvcConfig = JObject.Parse(File.ReadAllText(mvcConfigPath));
                if (!ValidateSchema(mvcConfig))
                    return null;
                var website = mvcConfig["website"].ToString().ToLower();
                if (website.Equals("github")) 
                {
                    var repoLatestRelease = $"{(string)mvcConfig["repo"]}/releases/latest";
                    websites.Add($"{githubWebsite}{repoLatestRelease}");

                    return $"{githubAPI}{repoLatestRelease}";
                } 
                else if (website.Equals("thunderstore"))
                {
                    var repo = (string)mvcConfig["repo"];
                    websites.Add($"{thunderstoreWebsite}{repo}");
                    
                    return $"{thunderstoreAPI}{repo}";
                }
                return null;
            }
            catch (JsonException e)
            {
                Plugin.logger.LogError($"Error parsing mvc JSON: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Plugin.logger.LogError($"Error reading mvc config: {e.Message}");
                return null;
            }
        }

        internal static bool ValidateSchema(JObject mvcConfig)
        {
            JSchema schema = JSchema.Parse(@"{ 
                'description': 'A MVC config',
                'type': 'object',
                'properties': {
                    'repo':{'type': 'string'},
                    'website':{'type': 'string'}
                }
            }");

            return mvcConfig.IsValid(schema);
        }
    }
}
