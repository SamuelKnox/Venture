using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class CraftingWindowLayoutCollectionUI : ItemCollectionBase
    {
        //[Header("Behavior")] // Moved to custom editor

        [SerializeField]
        protected uint _initialCollectionSize = 9;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        private CraftingWindowLayoutUI craftingWindow { get; set; }
        private bool canDragInCollectionDefault { get; set; }
        public override void Awake()
        {
            base.Awake();

            this.craftingWindow = GetComponent<CraftingWindowLayoutUI>();
            canDragInCollectionDefault = canDragInCollection;
        }

        public override void Start()
        {
            base.Start();

//            if (useReferences)
//            {
//                foreach (var col in InventoryManager.GetLootToCollections())
//                {
//                    col.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
//                    {
//                        if (window.isVisible == false)
//                            return;
//
//                        ValidateReferences();
//                        GetBlueprintFromCurrentLayout();
//                    };
//                    col.OnAddedItem += (itemArr, amount, cameFromCollection) =>
//                    {
//                        if (window.isVisible == false)
//                            return;
//
//                        foreach (var i in items)
//                        {
//                            i.Repaint();
//                        }
//
//                        GetBlueprintFromCurrentLayout();
//                    };
//                    col.OnUsedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
//                    {
//                        if (window.isVisible == false)
//                            return;
//
//                        foreach (var i in items)
//                        {
//                            if (i.item != null && i.item.ID == itemID)
//                                i.Repaint();
//                        }
//
//                        if (currentBlueprintItemsDict.ContainsKey(itemID))
//                        {
//                            CancelActiveCraftAndClearQueue(); // Used an item that we're using in crafting.
//                            GetBlueprintFromCurrentLayout();
//                        }
//                    };
//                }
//            }
        }
        
    }
}