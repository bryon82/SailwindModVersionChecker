using UnityEngine;
using System;
using static SailwindModVersionChecker.MVC_Plugin;

namespace SailwindModVersionChecker
{
    internal class VisitWebsiteButton : GoPointerButton
    {
        internal GameObject UIGameObject { get; set; }

        public override void OnActivate()
        {
            foreach(string website in UpdatesUI.Websites)
            {
                try
                {
                    Application.OpenURL(website);
                }
                catch (Exception e)
                {
                    LogError($"{e}");
                }
            }

            UIGameObject.gameObject.SetActive(false);
        }
    }
}
