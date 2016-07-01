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
    [AddComponentMenu("InventorySystem/Triggers/Crafting layout triggerer")]
    [RequireComponent(typeof(ObjectTriggerer))]
    public class CraftingLayoutTriggerer : CraftingTriggererBase<CraftingWindowLayoutUI>
    {
        protected override void SetWindow()
        {
            if (InventoryManager.instance.craftingLayout == null)
            {
                Debug.LogWarning("Crafting triggerer in scene, but no crafting window found", transform);
                return;
            }

            craftingWindow = InventoryManager.instance.craftingLayout;
        }
    }
}