#if PLAYMAKER

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
using HutongGames.PlayMaker;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Show or hide a dialog")]
    public class UIShowHideUIWindowFsmObject : FsmStateAction
    {
        public FsmBool show;
        public FsmObject window;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            var w = window.Value as UIWindow;
            if (w != null)
            {
                if (show.Value)
                    w.Show();
                else
                    w.Hide();

            }

            Finish();
        }
    }
}

#endif