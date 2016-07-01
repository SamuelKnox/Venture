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
    public class GiveItemNode : Node
    {
        /// <summary>
        /// The items the player receives when this node is executed.
        /// </summary>
        [ShowInNode]
        public InventoryProQuestInventoryItemAmountRow[] items;

        protected GiveItemNode()
            : base()
        {

        }

        protected InventoryItemAmountRow[] GetRows()
        {
            return items.Select(o => new InventoryItemAmountRow(o.item.val, o.amount)).ToArray();
        }

        public override void OnExecute()
        {
            var inventoryItems = GetRows();
            if (InventoryManager.CanAddItems(inventoryItems))
            {
                foreach (var item in inventoryItems)
                {
                    var i = UnityEngine.Object.Instantiate<InventoryItemBase>(item.item);
                    i.currentStackSize = item.amount;
                    InventoryManager.AddItem(i);
                }

                Finish(true);
                return;
            }

            Failed(QuestManager.instance.languageDatabase.inventoryIsFull); // Couldn't add items
        }
    }
}

#endif