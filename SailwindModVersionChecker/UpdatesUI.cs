using HarmonyLib;
using UnityEngine;

namespace SailwindModVersionChecker
{
    internal class UpdatesUI : MonoBehaviour
    {
        public static UpdatesUI instance;
        internal static TextMesh textMesh;
        internal static GameObject ui;

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
            textMesh.text = "<b>Updates Available</b>\n\n";
            textMesh.fontSize = 50;
            textMesh.lineSpacing = 1.1f;
            
            GameObject okButton = ui.transform.GetChild(1).gameObject;
            okButton.name = "button ok";
            okButton.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            okButton.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            okButton.GetComponentInChildren<TextMesh>().text = "OK";
            GameObject buttonGO = okButton.GetComponentInChildren<StartMenuButton>().gameObject;
            Destroy(buttonGO.GetComponent<StartMenuButton>());
            buttonGO.AddComponent<GPOkButton>().updatesUI = ui;
        }
    }
}
