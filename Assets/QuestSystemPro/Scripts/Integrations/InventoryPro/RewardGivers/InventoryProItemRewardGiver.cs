#if INVENTORY_PRO

using System;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.QuestSystemPro;
using Devdog.QuestSystemPro.Dialogue;
using Devdog.QuestSystemPro.UI;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    public partial class InventoryProItemRewardGiver : IRewardGiver
    {
        public InventoryProQuestInventoryItemAmountRow reward;
        public InventoryItemAmountRow row
        {
            get { return new InventoryItemAmountRow(reward.item.val, reward.amount); }
        }

        public virtual RewardRowUI rewardUIPrefab
        {
            get { return QuestManager.instance.settingsDatabase.inventoryProItemRewardRowUI; }
        }

        public ConditionInfo CanGiveRewards(Quest quest)
        {
            var canAdd = InventoryManager.CanAddItem(row);
            if (canAdd == false)
            {
                return new ConditionInfo(false, QuestManager.instance.languageDatabase.canNotCompleteQuestInventoryIsFull);
            }

            return ConditionInfo.success;
        }

        public void GiveRewards(Quest quest)
        {
            var inst = UnityEngine.Object.Instantiate<InventoryItemBase>(row.item);
            inst.currentStackSize = row.amount;

            InventoryManager.AddItem(inst);
        }
    }
}

#endif