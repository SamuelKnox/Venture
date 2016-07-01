#if REWIRED

using Devdog.QuestSystemPro.UI;
using UnityEngine;
using Rewired;

namespace Devdog.QuestSystemPro.Integration.RewiredIntegration
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu(QuestSystemPro.ProductName + "/Integration/Rewired/UI Window Rewired input")]
    public class UIWindowRewiredInput : RewiredMonoBehaviour
    {
        private UIWindow _window;

        protected override void Awake()
        {
            base.Awake();
            _window = GetComponent<UIWindow>();
        }

        protected void Update()
        {
            if (player.GetButtonDown(rewiredActionName))
            {
                _window.Toggle();
            }
        }
    }
}

#endif