#if INVENTORY_PRO 

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.QuestSystemPro.UI;
using UnityEngine;

namespace Devdog.QuestSystemPro
{
    public partial class LanguageDatabase
    {

        [Category("Inventory Pro")]
        public MultiLangString canNotCompleteQuestInventoryIsFull = new MultiLangString("", "Can not give quest reward, inventory is full.");
        public MultiLangString inventoryIsFull = new MultiLangString("", "Inventory is full.");

    }
}

#endif