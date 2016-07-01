#if INVENTORY_PRO

using System;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.QuestSystemPro;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    public partial class InventoryProStatRewardGiver : IRewardGiver
    {
        public InventoryItemPropertyLookup property;

        public virtual RewardRowUI rewardUIPrefab {
            get { return QuestManager.instance.settingsDatabase.inventoryProStatRewardRowUI; }
        }

        public InventoryPlayer currentPlayer
        {
            get { return InventoryPlayerManager.instance.currentPlayer; }
        }

        public ConditionInfo CanGiveRewards(Quest quest)
        {
            return ConditionInfo.success;
        }

        public void GiveRewards(Quest quest)
        {
            var p = property.property;
            var stat = currentPlayer.characterCollection.stats.Get(p.category, p.name);
            if (stat == null)
            {
                QuestLogger.LogWarning("Property with ID " + property._propertyID + " not found.");
                return;
            }

            InventoryItemUtility.SetItemProperty(currentPlayer, property, InventoryItemUtility.SetItemPropertiesAction.Use);
        }
    }
}

#endif