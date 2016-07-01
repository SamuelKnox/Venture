using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// A single row in the infobox.
    /// </summary>
    public partial class InventoryCraftingCategoryUI : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        protected UnityEngine.UI.Text title;

        [SerializeField]
        protected UnityEngine.UI.Image icon;

        [InventoryRequired]
        public RectTransform container;

        public virtual void Repaint(InventoryCraftingCategory category, InventoryItemCategory itemCategory)
        {
            title.text = category.name;

            if (icon != null)
                icon.sprite = itemCategory.icon;
        }

        public void Reset()
        {
            // Item has no specific states, no need to reset
        }
    }
}