using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Crafting standard")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowStandardUI : CraftingWindowBase
    {
        /// <summary>
        /// Crafting category title
        /// </summary>
        [Header("General UI references")]
        public Text currentCategoryTitle;

        /// <summary>
        /// Crafting category description
        /// </summary>
        public Text currentCategoryDescription;

        [InventoryRequired]
        public RectTransform blueprintsContainer;


        [Header("Blueprint prefabs")]
        public InventoryCraftingCategoryUI blueprintCategoryPrefab;
        
        /// <summary>
        /// The button used to select the prefab the user wishes to craft.
        /// </summary>
        [InventoryRequired] public InventoryCraftingBlueprintUI blueprintButtonPrefab;

        /// <summary>
        /// A single required item to be shown in the UI.
        /// </summary>
        [InventoryRequired] public InventoryUIItemWrapper blueprintRequiredItemPrefab;


        #region Crafting item page

        [Header("Craft blueprint UI References")]

        [InventoryRequired]
        public RectTransform blueprintRequiredItemsContainer;
        public InputField blueprintCraftAmountInput;

        #endregion

        [Header("UI window pages")]
        public UIWindowPage noBlueprintSelectedPage;
        public UIWindowPage blueprintCraftPage;

        [Header("Audio & Visuals")]
        public Color itemsAvailableColor = Color.white;
        public Color itemsNotAvailableColor = Color.red;

        #region Pools


        [NonSerialized]
        protected InventoryPool<InventoryCraftingCategoryUI> categoryPool;
        
        [NonSerialized]
        protected InventoryPool<InventoryCraftingBlueprintUI> blueprintPool;

        [NonSerialized]
        protected InventoryPool<InventoryUIItemWrapper> blueprintRequiredItemsPool;

        #endregion

        [NonSerialized]
        protected ItemCollectionBase[] subscribedToCollectionCurrency = new ItemCollectionBase[0];

        protected override void Awake()
        {
            if (blueprintCategoryPrefab != null)
            {
                categoryPool = new InventoryPool<InventoryCraftingCategoryUI>(blueprintCategoryPrefab, 16);
            }

            blueprintPool = new InventoryPool<InventoryCraftingBlueprintUI>(blueprintButtonPrefab, 128);
            blueprintRequiredItemsPool = new InventoryPool<InventoryUIItemWrapper>(blueprintRequiredItemPrefab, 8);
            InventoryPlayerManager.instance.OnPlayerChanged += InstanceOnPlayerChanged;
            
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            InstanceOnPlayerChanged(null, InventoryPlayerManager.instance.currentPlayer);
        }

        protected override void InstanceOnPlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
        {
            base.InstanceOnPlayerChanged(oldPlayer, newPlayer);

            if (oldPlayer != null)
            {
                foreach (var col in subscribedToCollectionCurrency)
                {
                    col.OnAddedItem -= OnAddedPlayerItem;
                    col.OnRemovedItem -= OnRemovedPlayerItem;
                }
            }

            subscribedToCollectionCurrency = InventoryManager.GetLootToCollections();
            foreach (var col in subscribedToCollectionCurrency)
            {
                col.OnAddedItem += OnAddedPlayerItem;
                col.OnRemovedItem += OnRemovedPlayerItem;
            }
        }

        private void OnRemovedPlayerItem(InventoryItemBase item, uint itemid, uint slot, uint amount)
        {
            if(currentBlueprint != null)
                SetCraftingBlueprint(currentBlueprint);
        }

        private void OnAddedPlayerItem(IEnumerable<InventoryItemBase> items, uint amount, bool camefromcollection)
        {
            if (currentBlueprint != null)
                SetCraftingBlueprint(currentBlueprint);
        }

        protected override void OnCraftButtonClicked()
        {
            CraftCurrentlySelectedBlueprint(GetCraftInputFieldAmount());
        }

        protected virtual int GetCraftInputFieldAmount()
        {
            if(blueprintCraftAmountInput != null)
                return int.Parse(blueprintCraftAmountInput.text);

            return 1;
        }

        protected virtual void ValidateCraftInputFieldAmount()
        {
            int amount = GetCraftInputFieldAmount();
            amount = Mathf.Clamp(amount, 1, 999);

            blueprintCraftAmountInput.text = amount.ToString();
        }

        public override InventoryCraftingBlueprint[] GetBlueprints(InventoryCraftingCategory category)
        {
            return currentCategory.blueprints.Where(o => o.playerLearnedBlueprint).ToArray();
        }

        public override void SetCraftingCategory(InventoryCraftingCategory category)
        {
//            if (currentCategory == category)
//            {
//                return;
//            }
            
            base.SetCraftingCategory(category);

            categoryPool.DestroyAll();
            blueprintPool.DestroyAll();
            if (blueprintCraftAmountInput != null)
                blueprintCraftAmountInput.text = "1"; // Reset
            
            if(currentCategoryTitle != null)
                currentCategoryTitle.text = category.name;
        
            if (currentCategoryDescription != null)
                currentCategoryDescription.text = category.description;

            if (noBlueprintSelectedPage != null)
                noBlueprintSelectedPage.Show();

//            var blueprints = GetBlueprints(category);
//            if (blueprintCraftPage != null && blueprints.Length > 0)
//            {
//                SetBlueprint(blueprints[0]); // Select first blueprint
//                blueprintCraftPage.Show();
//            }

            int lastItemCategory = -1;
            Button firstButton = null;
            foreach (var b in GetBlueprints(category))
            {
                if (b.playerLearnedBlueprint == false)
                    continue;

                var blueprintObj = blueprintPool.Get();
                blueprintObj.transform.SetParent(blueprintsContainer);
                InventoryUtility.ResetTransform(blueprintObj.transform);
                blueprintObj.Repaint(b);

                if (blueprintCategoryPrefab != null)
                {
                    Assert.IsTrue(b.resultItems.Length > 0, "No reward items set");
                    var item = b.resultItems.First().item;
                    Assert.IsNotNull(item, "Empty reward row on blueprint!");

                    if (lastItemCategory != item._category)
                    {
                        lastItemCategory = (int)item._category;

                        var uiCategory = categoryPool.Get();
                        uiCategory.Repaint(category, item.category);

                        uiCategory.transform.SetParent(blueprintsContainer);
                        blueprintObj.transform.SetParent(uiCategory.container);

                        InventoryUtility.ResetTransform(uiCategory.transform);
                        InventoryUtility.ResetTransform(blueprintObj.transform);
                    }
                }

                if (firstButton == null)
                {
                    firstButton = blueprintObj.button;
                }

                var bTemp = b; // Store capture list, etc.
                blueprintObj.button.onClick.AddListener(() =>
                {
                    currentBlueprint = bTemp;
                    SetCraftingBlueprint(currentBlueprint);

                    if (blueprintCraftPage != null && blueprintCraftPage.isVisible == false)
                    {
                        blueprintCraftPage.Show();
                    }
                });
            }

            if (firstButton != null)
            {
                firstButton.Select();
            }
        }


        public override void SetCraftingBlueprint(InventoryCraftingBlueprint blueprint)
        {
            base.SetCraftingBlueprint(blueprint);

            if (window.isVisible == false)
            {
                return;
            }

            blueprintRequiredItemsPool.DestroyAll();
            foreach (var item in blueprint.requiredItems)
            {
                var ui = blueprintRequiredItemsPool.Get();
                item.item.currentStackSize = (uint)item.amount;
                ui.transform.SetParent(blueprintRequiredItemsContainer);
                InventoryUtility.ResetTransform(ui.transform);

                ui.item = item.item;
                if (InventoryManager.GetItemCount(item.item.ID, currentCategory.alsoScanBankForRequiredItems) >= item.amount)
                    ui.icon.color = itemsAvailableColor;
                else
                    ui.icon.color = itemsNotAvailableColor;

                ui.Repaint();
                item.item.currentStackSize = 1; // Reset
            }
        }
    }
}