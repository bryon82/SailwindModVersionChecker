using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ModVersionChecker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MVC_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "zzz.raddude.modversionchecker";
        public const string PLUGIN_NAME = "ModVersionChecker";
        public const string PLUGIN_VERSION = "1.3.3";

        internal static MVC_Plugin Instance { get; private set; }
        private static ManualLogSource _logger;

        internal static void LogDebug(string message) => _logger.LogDebug(message);
        internal static void LogInfo(string message) => _logger.LogInfo(message);
        internal static void LogWarning(string message) => _logger.LogWarning(message);
        internal static void LogError(string message) => _logger.LogError(message);

        internal static ConfigEntry<bool> enableNotification;
        internal static ConfigEntry<bool> enableVersionChecks;

        private async void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _logger = Logger;

            enableNotification = Config.Bind("Settings", "Enable Notifications", true, "Enables the notification that shows at the start menu if there are updates available. Enable Version Checks must also be true.");
            enableVersionChecks = Config.Bind("Settings", "Enable Version Checks", true, "Enables checking for updates to mods.");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);

            if (!enableVersionChecks.Value)
                return;

            var updates = await VersionChecker.Check(Chainloader.PluginInfos);

            if (!enableNotification.Value)
                return;

            StartCoroutine(ShowUpdates(updates));
        }

        private IEnumerator ShowUpdates((string, List<string>) updates)
        {
            yield return new WaitUntil(() => UpdatesUI.Instance != null);
            UpdatesUI.Instance.ShowUpdatesUI(updates);
        }
    }
}
