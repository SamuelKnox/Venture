using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// A physical representation of a crafting station.
    /// </summary>
    [AddComponentMenu("InventorySystem/Triggers/Crafting triggerer")]
    [RequireComponent(typeof(ObjectTriggerer))]
    public class CraftingStandardTriggerer : CraftingTriggererBase<CraftingWindowStandardUI>
    {
        protected override void SetWindow()
        {
            if (InventoryManager.instance.craftingStandard == null)
            {
                Debug.LogWarning("Crafting triggerer in scene, but no crafting window found", transform);
                return;
            }

            craftingWindow = InventoryManager.instance.craftingStandard;
        }
    }
}