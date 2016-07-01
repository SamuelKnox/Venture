#if INVENTORY_PRO

using System;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.QuestSystemPro;
using Devdog.QuestSystemPro.UI;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    public partial class InventoryProInventorySlotsRewardGiver : INamedRewardGiver
    {
        public uint extraSlots = 1;

        [Required]
        public string collectionName;

        public string name
        {
            get { return "Extra slots"; }
        }

        public virtual RewardRowUI rewardUIPrefab
        {
            get { return QuestManager.instance.settingsDatabase.inventoryProInventorySlotsRewardRowUI; }
        }

        public ConditionInfo CanGiveRewards(Quest quest)
        {
            return ConditionInfo.success;
        }

        public void GiveRewards(Quest quest)
        {
            var col = ItemCollectionBase.FindByName(collectionName);
            if (col == null)
            {
                QuestLogger.LogWarning("Collection with name " + collectionName + " not found.");
                return;
            }

            col.AddSlots(extraSlots);
        }

        public override string ToString()
        {
            return extraSlots + " slots";
        }
    }
}

#endif