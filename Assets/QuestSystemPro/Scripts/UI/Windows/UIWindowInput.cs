using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.QuestSystemPro.UI
{
    using UnityEngine.Serialization;

    [RequireComponent(typeof(UIWindow))]
    public partial class UIWindowInput : MonoBehaviour
    {
        public KeyCode keyCode = KeyCode.None;

        protected UIWindow window;
        protected virtual void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                window.Toggle();
            }
        }
    }
}
