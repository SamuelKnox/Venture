using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper reference sum")]
    public partial class InventoryUIItemWrapperKeyTrigger : InventoryUIItemWrapper, IInventoryUIItemWrapperKeyTrigger
    {
        public Text keyCombinationText;
        public string keyCombination
        {
            get { return keyCombinationText != null ? keyCombinationText.text : ""; }
            set
            {
                if (keyCombinationText != null)
                    keyCombinationText.text = value;
            }
        }

        public override void TriggerUse()
        {
            if (item == null)
                return;

            if (itemCollection.canUseFromCollection == false)
                return;

            if (item != null)
            {
                var i = item;
                var used = i.Use();
                if (used >= 0 && i.currentStackSize <= 0)
                {
                    item = null;
                }

                if (itemCollection.useReferences)
                    itemCollection.NotifyReferenceUsed(i, i.ID, index, 1);

                Repaint();
            }
        }
    }
}