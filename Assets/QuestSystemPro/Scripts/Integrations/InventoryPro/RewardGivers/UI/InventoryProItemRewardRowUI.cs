#if INVENTORY_PRO

using UnityEngine;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.QuestSystemPro.Integration.InventoryPro;
using Devdog.QuestSystemPro.UI;
using UnityEngine.UI;

namespace Devdog.QuestSystemPro.Integration.InventoryPro.UI
{
    public class InventoryProItemRewardRowUI : RewardRowUI
    {
        [Header("UI Elements")]
        public InventoryUIItemWrapperStatic wrapper;


        public override void Repaint(IRewardGiver rewardGiver, Quest quest)
        {
            var r = (InventoryProItemRewardGiver) rewardGiver;

            r.reward.item.val.currentStackSize = r.reward.amount;
            wrapper.item = r.reward.item.val;
            wrapper.Repaint();
            r.reward.item.val.currentStackSize = 1; // Restore
        }
    }
}

#endif