using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SailwindModVersionChecker
{
    internal class VersionChecker
    {
        internal static List<string> websites = new List<string>();
        private static HttpClient _httpClient;

        const string modReleaseVersionsListurl = "https://raw.githubusercontent.com/bryon82/SailwindModVersionChecker/main/release_versions.json";
        const string githubWebsite = "https://github.com/";
        const string thunderstoreWebsite = "https://thunderstore.io/c/sailwind/p/";

        internal static async void Check(Dictionary<string, PluginInfo> pluginInfos)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# GitHub Content Fetcher");

            var modDict = new Dictionary<string, string>();
            var modList = await GetModVersionsList();
            foreach (JToken mod in modList)
            {
                modDict.Add(mod["guid"].ToString(), Regex.Match(mod["version"].ToString(), @"(\d+\.\d+\.\d+)").Groups[1].Value);
            }

            var updates = "";
            foreach (var plugin in pluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                var guid = metadata.GUID;
                var version = metadata.Version.ToString();

                if (!modDict.ContainsKey(guid) || modDict[guid] == null)
                    continue;

                var latestVersion = modDict[guid];
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
                    var website = modList.FirstOrDefault(m => m["guid"].ToString() == guid)?["repo"]?.ToString();
                    if (website.StartsWith(githubWebsite))
                    {
                        websites.Add($"{website}/releases/latest");
                    }
                    else if (website.StartsWith(thunderstoreWebsite))
                    {
                        websites.Add(website);
                    }

                    continue;
                }

                Plugin.logger.LogInfo($"{metadata.Name} is up to date");
            }

            if (!updates.Equals("") && Plugin.enableNotification.Value)
                UpdatesUI.ui.SetActive(true);
            UpdatesUI.textMesh.text += updates;
            UpdatesUI.websites = websites;
        }

        internal static async Task<JArray> GetModVersionsList()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(modReleaseVersionsListurl);
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
    }
}
