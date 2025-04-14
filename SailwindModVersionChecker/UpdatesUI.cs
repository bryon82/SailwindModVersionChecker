using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SailwindModVersionChecker
{
    internal class UpdatesUI : MonoBehaviour
    {
        public static UpdatesUI instance;
        internal static TextMesh textMesh;
        internal static GameObject ui;
        internal static List<string> websites;

        private void Awake()
        {
            instance = this;
        }

        internal static void SetupUpdatesUI(StartMenu startMenu)
        {            
            GameObject confirmQuitUI = (GameObject)Traverse.Create(startMenu).Field("confirmQuitUI").GetValue();
            ui = Instantiate(confirmQuitUI, confirmQuitUI.transform.parent);
            ui.name = "updates UI";
            ui.SetActive(false);

            GameObject text = ui.transform.GetChild(4).gameObject;
            Destroy(ui.transform.GetChild(2).gameObject);
            text.transform.localPosition = new Vector3(0f, 0.97f, 0.002f);
            textMesh = text.GetComponent<TextMesh>();
            textMesh.text = "<b>Mod Updates Available</b>\n\n";
            textMesh.fontSize = 50;
            textMesh.lineSpacing = 1.1f;
            
            GameObject dismissButton = ui.transform.GetChild(1).gameObject;
            GameObject visitWebsiteButton = Instantiate(dismissButton, dismissButton.transform.parent);

            dismissButton.name = "button dismiss";
            dismissButton.transform.localPosition = new Vector3(0.5f, -0.6f, 0f);
            dismissButton.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            dismissButton.GetComponentInChildren<TextMesh>().text = "Dismiss";
            GameObject buttonGO = dismissButton.GetComponentInChildren<StartMenuButton>().gameObject;
            Destroy(buttonGO.GetComponent<StartMenuButton>());
            buttonGO.AddComponent<DismissButton>().updatesUI = ui;

            visitWebsiteButton.name = "button visit website";
            visitWebsiteButton.transform.localPosition = new Vector3(-0.5f, -0.6f, 0f);
            visitWebsiteButton.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            visitWebsiteButton.GetComponentInChildren<TextMesh>().text = "Visit Mod\nWebsites";
            buttonGO = visitWebsiteButton.GetComponentInChildren<StartMenuButton>().gameObject;
            Destroy(buttonGO.GetComponent<StartMenuButton>());
            buttonGO.AddComponent<VisitWebsiteButton>().updatesUI = ui;
        }

        internal void ShowUpdatesUI((string updates, List<string> websites) updateInfo)
        {
            if (!updateInfo.updates.IsNullOrWhiteSpace())
                ui.SetActive(true);
            textMesh.text += updateInfo.updates;
            websites = updateInfo.websites;
        }
    }
}
