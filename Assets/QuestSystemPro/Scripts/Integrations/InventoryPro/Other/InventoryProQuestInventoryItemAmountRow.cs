#if INVENTORY_PRO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    [System.Serializable]
    public struct InventoryProQuestInventoryItemAmountRow
    {
        public Asset<InventoryItemBase> item;
        public uint amount;
    }
}

#endif