#if REWIRED

using UnityEngine;
using System.Collections;
using System.Linq;
using Devdog.InventorySystem.UI;
using Rewired;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/Integration/Rewired/Rewired UI Window")]
    public class RewiredUIWindow : RewiredMonoBehaviour
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