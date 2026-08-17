using UnityEngine;

namespace ModVersionChecker
{
    internal class DismissButton : GoPointerButton
    {
        internal GameObject UIGameObject { get; set; }
        public override void OnActivate()
        {
            UIGameObject.gameObject.SetActive(false);
        }
    }
}
