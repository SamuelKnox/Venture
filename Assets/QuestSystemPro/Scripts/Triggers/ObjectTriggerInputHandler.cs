using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.QuestSystemPro
{
    [RequireComponent(typeof(ObjectTriggerBase))]
    public class ObjectTriggerInputHandler : MonoBehaviour, IObjectTriggerInputHandler
    {
        public bool toggleWhenTriggered = true;

        public bool triggerMouseClick = true;
        public KeyCode triggerKeyCode = KeyCode.None;

        public CursorIcon cursorIcon; // TODO: probably move this to a global 'setting' manager or scriptable object.
//        {
//            get { return QuestSettingsManager.instance.useCursor; }
//            set { QuestSettingsManager.instance.useCursor = value; }
//        }

        private ObjectTriggerBase _trigger;

        protected virtual void Awake()
        {
            _trigger = GetComponent<ObjectTriggerBase>();
            Assert.IsNotNull(_trigger);
        }

        protected void Update()
        {
            if (AreKeysDown())
            {
                OnKeysDown();
            }
        }

        protected virtual bool AreKeysDown()
        {
            return Input.GetKeyDown(triggerKeyCode);
        }

        protected virtual void OnMouseDown()
        {
            if (triggerMouseClick && UIUtility.isHoveringUIElement == false)
            {
                Used();
            }
        }

        protected virtual void OnKeysDown()
        {
            Used();
        }

        protected virtual void Used()
        {
            if (enabled == false)
                return;

            if (toggleWhenTriggered)
            {
                _trigger.Toggle();
            }
            else
            {
                _trigger.Use();
            }
        }
    }
}
