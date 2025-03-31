using UnityEngine;

namespace SailwindModVersionChecker
{
    internal class GPOkButton : GoPointerButton
    {
        public GameObject updatesUI;
        public override void OnActivate()
        {
            updatesUI.gameObject.SetActive(false);          
        }
    }
}
