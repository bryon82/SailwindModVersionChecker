using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SailwindModVersionChecker
{
    internal class UpdatesUI : MonoBehaviour
    {
        public static UpdatesUI Instance { get; private set; }        
        internal static GameObject UI { get; private set; }
        internal static List<string> Websites { get; private set; }

        private static TextMesh _textMesh;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        internal static void SetupUpdatesUI(StartMenu startMenu)
        {
            var confirmQuitUI = (GameObject)Traverse.Create(startMenu).Field("confirmQuitUI").GetValue();
            UI = Instantiate(confirmQuitUI, confirmQuitUI.transform.parent);
            UI.name = "updates UI";
            UI.SetActive(false);

            var text = UI.transform.GetChild(4).gameObject;
            Destroy(UI.transform.GetChild(2).gameObject);
            text.transform.localPosition = new Vector3(0f, 0.97f, 0.002f);
            _textMesh = text.GetComponent<TextMesh>();
            _textMesh.text = "<b>Mod Updates Available</b>\n\n";
            _textMesh.fontSize = 50;
            _textMesh.lineSpacing = 1.1f;

            var dismissButton = UI.transform.GetChild(1).gameObject;
            var visitWebsiteButton = Instantiate(dismissButton, dismissButton.transform.parent);

            dismissButton.name = "button dismiss";
            dismissButton.transform.localPosition = new Vector3(0.5f, -0.6f, 0f);
            dismissButton.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            dismissButton.GetComponentInChildren<TextMesh>().text = "Dismiss";

            var buttonGO = dismissButton.GetComponentInChildren<StartMenuButton>().gameObject;
            Destroy(buttonGO.GetComponent<StartMenuButton>());
            buttonGO.AddComponent<DismissButton>().UIGameObject = UI;

            visitWebsiteButton.name = "button visit website";
            visitWebsiteButton.transform.localPosition = new Vector3(-0.5f, -0.6f, 0f);
            visitWebsiteButton.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            visitWebsiteButton.GetComponentInChildren<TextMesh>().text = "Visit Mod\nWebsites";

            buttonGO = visitWebsiteButton.GetComponentInChildren<StartMenuButton>().gameObject;
            Destroy(buttonGO.GetComponent<StartMenuButton>());
            buttonGO.AddComponent<VisitWebsiteButton>().UIGameObject = UI;
        }

        internal void ShowUpdatesUI((string updates, List<string> websites) updateInfo)
        {
            if (!updateInfo.updates.IsNullOrWhiteSpace())
                UI.SetActive(true);
            _textMesh.text += updateInfo.updates;
            Websites = updateInfo.websites;
        }
    }
}
