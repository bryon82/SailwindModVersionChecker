﻿using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SailwindModVersionChecker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "zzz.raddude82.modversionchecker";
        public const string PLUGIN_NAME = "ModVersionChecker";
        public const string PLUGIN_VERSION = "1.2.2";

        internal static ManualLogSource logger;

        internal static ConfigEntry<bool> enableNotification;
        internal static ConfigEntry<bool> enableVersionChecks;

        private async void Awake()
        {
            logger = Logger;

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
            yield return new WaitUntil(() => UpdatesUI.instance != null);
            UpdatesUI.instance.ShowUpdatesUI(updates);
        }
    }
}
