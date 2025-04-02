using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SailwindModVersionChecker
{
    internal class VersionChecker
    {
        internal static List<string> websites = new List<string>();
        private static HttpClient _httpClient;

        const string modListurl = "https://raw.githubusercontent.com/bryon82/SailwindModVersionChecker/main/ModList.json";
        const string githubAPI = "https://api.github.com/repos/";
        const string thunderstoreAPI = "https://thunderstore.io/api/experimental/package/";
        const string githubWebsite = "https://github.com/";
        const string thunderstoreWebsite = "https://thunderstore.io/c/sailwind/p/";

        internal static async void Check(Dictionary<string, PluginInfo> pluginInfos)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# GitHub Content Fetcher");

            var modDict = new Dictionary<string, string>();
            var modList = await GetModList();
            foreach (JToken mod in modList)
            {
                modDict.Add(mod["guid"].ToString(), mod["repo"].ToString());
            }

            _httpClient.DefaultRequestHeaders.Remove("User_Agent");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# GitHub API Client");

            var updates = "";
            foreach (var plugin in pluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                var guid = metadata.GUID;
                var version = metadata.Version.ToString();

                if (!modDict.ContainsKey(guid) || modDict[guid] == null)
                    continue;

                var url = GetUrl(modDict[guid]);
                if (url == null)
                    continue;

                var latestVersion = await GetLatestAsync(url);
                if (latestVersion == null)
                    continue;

                Version vCurrent;
                Version vLatest;

                try
                {
                    vCurrent = new Version(version);
                    vLatest = new Version(latestVersion);
                }
                catch (ArgumentException e)
                {
                    Plugin.logger.LogWarning($"{guid}: {e.Message}");
                    continue;
                }                

                if (vCurrent.CompareTo(vLatest) < 0)
                {
                    updates += $"{metadata.Name} {version} → {latestVersion}\n";
                    Plugin.logger.LogInfo($"*Update Available*  {metadata.Name} {version} → {latestVersion}");
                    var website = modDict[guid].Contains(githubWebsite) ? $"{modDict[guid]}/releases/latest" : modDict[guid];
                    websites.Add(website);
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
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject releaseInfo = JObject.Parse(jsonResponse);
                    
                if (url.Contains("github"))
                    return Regex.Match((string)releaseInfo["tag_name"], @"(\d+\.\d+\.\d+)").Groups[1].Value;
                if (url.Contains("thunderstore"))
                    return (string)releaseInfo["latest"]["version_number"];

                return null;                
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

        internal static async Task<JArray> GetModList()
        {
            try
            {                    
                HttpResponseMessage response = await _httpClient.GetAsync(modListurl);

                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();                    
                return JArray.Parse(jsonContent);
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

        internal static string GetUrl(string website)
        {            
            if (website.Contains(githubWebsite))
            {
                return $"{githubAPI}{website.Replace(githubWebsite, "")}/releases/latest";
            }
            else if (website.Contains(thunderstoreWebsite))
            {
                return $"{thunderstoreAPI}{website.Replace(thunderstoreWebsite, "")}";
            }
            
            return null;            
        }
    }
}
