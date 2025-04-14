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
        const string modReleaseVersionsListurl = "https://raw.githubusercontent.com/bryon82/SailwindModVersionChecker/main/release_versions.json";
        const string githubWebsite = "https://github.com/";
        const string thunderstoreWebsite = "https://thunderstore.io/c/sailwind/p/";

        internal static async Task<(string, List<string>)> Check(Dictionary<string, PluginInfo> pluginInfos)
        {
            if (pluginInfos == null || pluginInfos.Count == 0)
            {
                Plugin.logger.LogError("No plugins found to check for updates.");
                return (null, null);
            }

            var latestReleaseList = new List<ReleaseVersionResponse>();
            var modList = await GetModVersionsList();
            if (modList == null)
            {
                Plugin.logger.LogError("GetModVersionsList returned null");
                return (null, null);
            }

            foreach (JToken mod in modList)
            {
                if (mod == null) continue;

                var guidProperty = mod["guid"];
                var versionProperty = mod["version"];
                var repoProperty = mod["repo"];

                if (guidProperty == null || versionProperty == null || repoProperty == null)
                {
                    Plugin.logger.LogWarning("Skipping mod with missing properties");
                    continue;
                }

                var versionString = versionProperty.ToString();
                var versionMatch = Regex.Match(versionString, @"(\d+\.\d+\.\d+)");
                if (!versionMatch.Success)
                {
                    Plugin.logger.LogWarning($"Skipping mod with invalid version format: {guidProperty} {versionString}");
                    continue;
                }

                latestReleaseList.Add(new ReleaseVersionResponse
                {
                    guid = guidProperty.ToString(),
                    version = versionMatch.Groups[1].Value,
                    repo = repoProperty.ToString()
                });                
            }

            var updates = "";
            var websites = new List<string>();
            foreach (var plugin in pluginInfos)
            {
                var metadata = plugin.Value?.Metadata;
                if (metadata == null) continue;

                var guid = metadata.GUID;
                var version = metadata.Version.ToString();

                var latestRelease = latestReleaseList.FirstOrDefault(m => m.guid == guid);
                if (latestRelease == null ||
                    latestRelease.version.IsNullOrWhiteSpace() ||
                    latestRelease.repo.IsNullOrWhiteSpace())
                {
                    continue;
                }                    

                Version vCurrent;
                Version vLatest;

                try
                {
                    vCurrent = new Version(version);
                    vLatest = new Version(latestRelease.version);
                }
                catch (ArgumentException e)
                {
                    Plugin.logger.LogWarning($"{guid}: {e.Message}");
                    continue;
                }

                if (vCurrent.CompareTo(vLatest) < 0)
                {
                    updates += $"{metadata.Name} {version} → {latestRelease.version}\n";
                    Plugin.logger.LogInfo($"*Update Available*  {metadata.Name} {version} → {latestRelease.version}");
                    if (latestRelease.repo.StartsWith(githubWebsite))
                    {
                        websites.Add($"{latestRelease.repo}/releases/latest");
                    }
                    else if (latestRelease.repo.StartsWith(thunderstoreWebsite))
                    {
                        websites.Add(latestRelease.repo);
                    }

                    continue;
                }

                Plugin.logger.LogInfo($"{metadata.Name} is up to date");
            }            

            return (updates, websites);
        }

        internal static async Task<JArray> GetModVersionsList()
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "C# GitHub Content Fetcher");

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
