#if INVENTORY_PRO

using System;
using Devdog.InventorySystem;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro.Integration.InventoryPro.UI
{
    public class InventoryProItemTaskProgressRowUI : TaskProgressRowUI
    {
        [Header("Inventory Pro UI Reference")]
        public InventoryUIItemWrapperBase wrapper;

        public override void Repaint(Task task)
        {
            base.Repaint(task);

            var inventoryTask = (IInventoryProTask)task;
            var item = inventoryTask.item.val;

            if (wrapper != null)
            {
                wrapper.item = item;
                wrapper.Repaint();
            }
        }
    }
}

#endif