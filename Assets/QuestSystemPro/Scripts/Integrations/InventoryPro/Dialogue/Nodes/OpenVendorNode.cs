#if INVENTORY_PRO

using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.QuestSystemPro.Dialogue;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    [System.Serializable]
    [Category("Devdog/Inventory Pro")]
    public class OpenVendorNode : Node
    {
        [NonSerialized]
        public new string message;

        protected OpenVendorNode()
            : base()
        {

        }

        public override void OnExecute()
        {
            if (DialogueManager.instance.currentDialogueOwner != null && DialogueManager.instance.currentDialogueOwner.transform != null)
            {
                var trigger = DialogueManager.instance.currentDialogueOwner.transform.GetComponent<ObjectTriggerer>();
                if (trigger != null)
                {
                    trigger.Use();
                }
            }

            Finish(true);
        }
    }
}

#endif