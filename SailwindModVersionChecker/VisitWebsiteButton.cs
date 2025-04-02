using UnityEngine;
using System;

namespace SailwindModVersionChecker
{
    internal class VisitWebsiteButton : GoPointerButton
    {
        public GameObject updatesUI;

        public override void OnActivate()
        {
            foreach(string website in UpdatesUI.websites)
            {
                try
                {
                    Application.OpenURL(website);
                }
                catch (Exception e)
                {
                    Plugin.logger.LogError(e);
                }
            }

            updatesUI.gameObject.SetActive(false);
        }
    }
}
