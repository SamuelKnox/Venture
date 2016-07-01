#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Use a triggerer.")]
    public class UseTriggererFsmObject : FsmStateAction
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
                if (useOrUnUse.Value)
                {
                    t.Use();
                }
                else
                {
                    t.UnUse();
                }
            }

            Finish();
        }
    }
}

#endif