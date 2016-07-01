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

namespace Devdog.InventorySystem
{
    [HelpURL("http://devdog.nl/documentation/lootui/")]
    [AddComponentMenu("InventorySystem/Windows/Loot")]
    [RequireComponent(typeof(UIWindow))]
    public partial class LootUI : ItemCollectionBase
    {
        public override uint initialCollectionSize
        {
            get { return 0; }
        }

        private UIWindow _window;
        public virtual UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        public override void Start()
        {
            base.Start();

            OnRemovedItem += (item, itemID, slot, amount) =>
            {
                HideWindowIfEmpty();
            };

            window.OnShow += WindowOnOnShow;
        }

        private void WindowOnOnShow()
        {
            if (items.Length > 0)
            {
                var selectable = items[0].GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.Select();
                }
            }
        }

        // <inheritcdoc />
        public override void SetItems(InventoryItemBase[] newItems, bool setParent, bool repaint = true)
        {
            bool canPutIn = canPutItemsInCollection;
            canPutItemsInCollection = true;

            Resize((uint)items.Length, true); // Force resize, SetItems() doesn't force, hence the extra call.
            base.SetItems(newItems, setParent, repaint);

            canPutItemsInCollection = canPutIn;
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            var c = item as CurrencyInventoryItem;
            if (c != null)
            {
                return AddCurrency(c.amount, c.currencyID);
            }

            return base.SetItem(slot, item);
        }

        public virtual void TakeCurrencies()
        {
            foreach (var c in currenciesContainer.lookups)
            {
                bool added = InventoryManager.AddCurrency(c);
                if (added)
                {
                    c.amount = 0f;
                }
            }

            HideWindowIfEmpty();
        }

        public virtual void TakeAll()
        {
            TakeCurrencies();
            foreach (var item in this.items)
            {
                if(item != null && item.item != null)
                {
                    ((InventoryUIItemWrapper)item).OnPointerUp(new PointerEventData(EventSystem.current));
                }
            }

            HideWindowIfEmpty();
        }

        protected virtual void HideWindowIfEmpty()
        {
            if (isEmpty)
            {
                window.Hide();
            }
        }

        public override IList<InventoryItemUsability> GetExtraItemUsabilities(IList<InventoryItemUsability> basic)
        {
            var l = base.GetExtraItemUsabilities(basic);
        
            l.Add(new InventoryItemUsability("Loot", (item) =>
            {
                InventoryManager.AddItemAndRemove(item);
            }));

            return l;
        }


        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;    
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return false;    
        }
    }
}