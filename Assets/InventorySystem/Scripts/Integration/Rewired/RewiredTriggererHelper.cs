#if REWIRED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using Devdog.InventorySystem.UI;
using UnityEngine;


namespace Devdog.InventorySystem.Integration.RewiredIntegration
{
    [RequireComponent(typeof(ObjectTriggererBase))]
    [AddComponentMenu("InventorySystem/Integration/Rewired triggerer helper")]
    public partial class RewiredTriggererHelper : RewiredMonoBehaviour, IObjectTriggerInputOverrider
    {
//        private ObjectTriggererBase _triggerer;
        protected override void Awake()
        {
            base.Awake();
//            _triggerer = GetComponent<ObjectTriggererBase>();
        }

        public bool AreKeysDown(ObjectTriggererBase triggerer)
        {
            return player.GetButtonDown(rewiredActionName);
        }
    }    
}

#endif