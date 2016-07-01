#if INVENTORY_PRO

using UnityEngine;
using System.Collections;
using System;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.QuestSystemPro;

namespace Devdog.QuestSystemPro.Integration.InventoryPro
{
    public interface IInventoryProTask
    {
        Asset<InventoryItemBase> item { get; }
    }
}

#endif