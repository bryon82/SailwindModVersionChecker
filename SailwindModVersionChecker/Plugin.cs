using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace SailwindModVersionChecker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {        
        public const string PLUGIN_GUID = "zzz.raddude82.modversionchecker";
        public const string PLUGIN_NAME = "ModVersionChecker";
        public const string PLUGIN_VERSION = "1.1.0";

        internal static ManualLogSource logger;

        internal static ConfigEntry<bool> enableNotification;

        private void Awake()
        {
            logger = Logger;

            enableNotification = Config.Bind("Settings", "Enable Notifications", true, "Enables the notification that shows at the start menu if there are updates available.");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);

            VersionChecker.Check(Chainloader.PluginInfos);
        }
    }
}
