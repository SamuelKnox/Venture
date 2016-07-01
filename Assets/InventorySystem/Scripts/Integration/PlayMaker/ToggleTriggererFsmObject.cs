#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Toggle a triggerer.")]
    public class ToggleTriggererFsmObject : FsmStateAction
    {
        public FsmObject trigger;
        public FsmBool useOrUnUse;
        
        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            var t = trigger.Value as ObjectTriggererBase;
            if (t != null)
            {
                t.Toggle();
            }

            Finish();
        }
    }
}

#endif