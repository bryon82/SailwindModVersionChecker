using UnityEngine;

namespace SailwindModVersionChecker
{
    internal class DismissButton : GoPointerButton
    {
        public GameObject updatesUI;
        public override void OnActivate()
        {
            updatesUI.gameObject.SetActive(false);          
        }
    }
}
