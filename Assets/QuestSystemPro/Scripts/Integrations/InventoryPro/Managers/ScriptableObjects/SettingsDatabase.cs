#if INVENTORY_PRO 

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public partial class SettingsDatabase
    {

        [Category("Inventory Pro Quest Prefabs")]
        public RewardRowUI inventoryProItemRewardRowUI;
        public RewardRowUI inventoryProStatRewardRowUI;
        public RewardRowUI inventoryProInventorySlotsRewardRowUI;

        [Header("Inventory Pro Task Prefabs")]
        public TaskProgressRowUI inventoryProGatherTaskRowUI;


    }
}

#endif