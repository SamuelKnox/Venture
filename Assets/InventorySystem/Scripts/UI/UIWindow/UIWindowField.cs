using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem.UI
{
    [System.Serializable]
    public struct UIWindowField
    {
        [SerializeField]
        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (useDynamicSearch)
                {
                    _window = UIWindow.FindByName(name);
                }

                return _window;
            }
            set
            {
                _window = value;
                if (_window != null)
                {
                    useDynamicSearch = false;
                }
            }
        }

        public string name;
        public bool useDynamicSearch;
    }
}
