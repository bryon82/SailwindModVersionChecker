using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SailwindModVersionChecker.MVC_Plugin;

namespace SailwindModVersionChecker
{
    internal class VersionChecker
    {
        private const string RELEASE_VERSIONS_LIST_URL =
            "https://cdn.jsdelivr.net/gh/bryon82/SailwindModVersionChecker@main/release_versions.json";
        private const string GITHUB_URL = "https://github.com/";
        private const string GITLAB_URL = "https://gitlab.com/";
        private const string THUNDERSTORE_URL = "https://thunderstore.io/c/sailwind/p/";
        private const string CACHE_FILE = "mvc_cache.json";
        private const int CHECK_INTERVAL_HOURS = 2;

        internal static async Task<(string, List<string>)> Check(Dictionary<string, PluginInfo> pluginInfos)
        {
            if (pluginInfos == null || pluginInfos.Count == 0)
            {
                LogError("No plugins found to check for updates.");
                return (null, null);
            }

            var latestReleaseList = new List<ReleaseVersionResponse>();
            var modList = await GetModVersionsList();
            if (modList == null)
            {
                LogError("GetModVersionsList returned null");
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
                    LogWarning("Skipping mod with missing properties");
                    continue;
                }

                var versionString = versionProperty.ToString();
                var versionMatch = Regex.Match(versionString, @"\d+(?:\.\d+){1,3}");
                if (!versionMatch.Success)
                {
                    LogWarning($"Skipping mod with invalid version format: {guidProperty} {versionString}");
                    continue;
                }

                latestReleaseList.Add(new ReleaseVersionResponse
                {
                    Guid = guidProperty.ToString(),
                    Version = versionMatch.Value,
                    Repo = repoProperty.ToString()
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

                var latestRelease = latestReleaseList.FirstOrDefault(m => m.Guid == guid);
                if (latestRelease == null ||
                    latestRelease.Version.IsNullOrWhiteSpace() ||
                    latestRelease.Repo.IsNullOrWhiteSpace())
                {
                    continue;
                }

                Version vCurrent;
                Version vLatest;

                try
                {
                    var normalizedCurrent = NormalizeVersion(version);
                    var normalizedLatest = NormalizeVersion(latestRelease.Version);

                    if (normalizedCurrent == null || normalizedLatest == null)
                    {
                        LogWarning($"{guid}: could not parse version string(s) '{version}' / '{latestRelease.Version}'");
                        continue;
                    }

                    vCurrent = new Version(normalizedCurrent);
                    vLatest = new Version(normalizedLatest);
                }
                catch (ArgumentException e)
                {
                    LogWarning($"{guid}: {e.Message}");
                    continue;
                }

                if (vCurrent.CompareTo(vLatest) < 0)
                {
                    updates += $"{metadata.Name} {version} → {latestRelease.Version}\n";
                    LogInfo($"*Update Available*  {metadata.Name} {version} → {latestRelease.Version}");
                    if (latestRelease.Repo.StartsWith(GITHUB_URL))
                    {
                        websites.Add($"{latestRelease.Repo}/releases/latest");
                    }
                    else if (latestRelease.Repo.StartsWith(THUNDERSTORE_URL))
                    {
                        websites.Add(latestRelease.Repo);
                    }
                    else if (latestRelease.Repo.StartsWith(GITLAB_URL))
                    {
                        websites.Add($"{latestRelease.Repo}/-/releases");
                    }
                    continue;
                }

                LogInfo($"{metadata.Name} is up to date");
            }

            return (updates, websites);
        }

        internal static async Task<JArray> GetModVersionsList()
        {
            var cachePath = Path.Combine(Path.GetDirectoryName(SaveSlots.GetCurrentSavePath()), CACHE_FILE);
            try
            {
                if (File.Exists(cachePath))
                {
                    var cache = JObject.Parse(File.ReadAllText(cachePath));
                    var lastChecked = cache["lastChecked"]?.ToObject<DateTime>() ?? DateTime.MinValue;
                    if (DateTime.UtcNow - lastChecked < TimeSpan.FromHours(CHECK_INTERVAL_HOURS))
                    {
                        LogDebug("Using cached release versions (checked recently)");
                        return (JArray)cache["data"];
                    }
                }

                var _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "SailwindModVersionChecker");

                var response = await _httpClient.GetAsync(RELEASE_VERSIONS_LIST_URL);
                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(jsonContent);

                File.WriteAllText(cachePath, new JObject
                {
                    ["lastChecked"] = DateTime.UtcNow,
                    ["data"] = data
                }.ToString());

                return data;
            }
            catch (HttpRequestException e)
            {
                LogError($"Error accessing website API: {e.Message}");
                // fall back to stale cache
                if (File.Exists(cachePath))
                {
                    LogWarning("Falling back to stale cache due to network error");
                    return (JArray)JObject.Parse(File.ReadAllText(cachePath))["data"];
                }
                return null;
            }
            catch (JsonException e)
            {
                LogError($"Error parsing JSON response: {e.Message}");
                return null;
            }
        }

        private static string NormalizeVersion(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // grab the leading numeric dotted sequence, ignore 'v' prefixes, -beta suffixes, etc.
            var match = Regex.Match(input, @"\d+(\.\d+){0,3}");
            if (!match.Success) return null;

            var parts = match.Value.Split('.').ToList();
            while (parts.Count < 4)
                parts.Add("0");   // pad up to major.minor.build.revision

            return string.Join(".", parts);
        }
    }
}
