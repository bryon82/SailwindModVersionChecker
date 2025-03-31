using HarmonyLib;

namespace SailwindModVersionChecker
{
    internal class UIPatches
    {
        [HarmonyPatch(typeof(StartMenu))]
        private class StartMenuPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("Awake")]
            private static void AddContinueUI(StartMenu __instance)
            {
                UpdatesUI.SetupUpdatesUI(__instance);
                __instance.gameObject.AddComponent<UpdatesUI>();
            }            
        }
    }
}
