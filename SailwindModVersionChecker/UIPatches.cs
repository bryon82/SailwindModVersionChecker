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
            public static void AddContinueUI(StartMenu __instance)
            {
                UpdatesUI.SetupUpdatesUI(__instance);
                __instance.gameObject.AddComponent<UpdatesUI>();
            }

            [HarmonyPrefix]
            [HarmonyPatch("ButtonClick", new System.Type[] { typeof(StartMenuButtonType) })]
            public static void HideUpdatesUI()
            {
                if(UpdatesUI.ui.activeInHierarchy)
                    UpdatesUI.ui.SetActive(false);
            }
        }
    }
}
