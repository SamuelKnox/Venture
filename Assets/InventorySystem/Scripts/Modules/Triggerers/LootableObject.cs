using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    using System.Linq;

    [HelpURL("http://devdog.nl/documentation/lootables-generators/")]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [AddComponentMenu("InventorySystem/Triggers/Lootable objet")]
    public partial class LootableObject : MonoBehaviour, IObjectTriggerUser, IInventoryItemContainer
    {
        public delegate void LootedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);
        public delegate void Empty();

        /// <summary>
        /// Called when an item was looted by a player from this lootable object.
        /// </summary>
        public event LootedItem OnLootedItem;
        public event Empty OnEmpty;


        [SerializeField]
        private string _uniqueName;
        public string uniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }



        [SerializeField]
        private InventoryItemBase[] _items = new InventoryItemBase[0];
        public InventoryItemBase[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value.Where(o => o is CurrencyInventoryItem == false).ToArray();

                var cs = value.Select(o => o as CurrencyInventoryItem).Where(o => o != null).ToArray();
                currencies = new InventoryCurrencyLookup[cs.Length];
                for (int i = 0; i < currencies.Length; i++)
                {
                    currencies[i] = new InventoryCurrencyLookup(cs[i].currencyType, cs[i].amount);
                }
            }
        }

        /// <summary>
        /// The currencies we're holding
        /// </summary>
        public InventoryCurrencyLookup[] currencies = new InventoryCurrencyLookup[0];


        [SerializeField]
        private bool _destroyWhenEmpty = false;


        public LootUI lootUI { get; protected set; }
        public UIWindow window { get; protected set; }

        protected Animator animator;
        public ObjectTriggerer triggerer { get; protected set; }


        protected virtual void Start()
        {
            lootUI = InventoryManager.instance.loot;
            if (lootUI == null)
            {
                Debug.LogWarning("No loot window set, yet there's a lootable object in the scene", transform);
                return;
            }

            if (GetComponent<IInventoryItemContainerGenerator>() == null)
            {
                // Items were not generated -> Instantiate them
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == null)
                    {
                        continue;
                    }

                    var newItem = Instantiate<InventoryItemBase>(items[i]);
                    newItem.currentStackSize = items[i].currentStackSize;
                    items[i] = newItem;

                    items[i].transform.SetParent(transform);
                    items[i].gameObject.SetActive(false);
                }
            }
            

            window = lootUI.window;
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = new UIWindowField() {window = window};
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            animator = GetComponent<Animator>();

            triggerer.OnTriggerUsed += Used;
            triggerer.OnTriggerUnUsed += UnUsed;
        }

        private void LootUIOnOnRemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            items[slot] = null;

            if (OnLootedItem != null)
                OnLootedItem(item, itemID, slot, amount);

            if (lootUI.isEmpty)
            {
                if (OnEmpty != null)
                    OnEmpty();

                if (_destroyWhenEmpty)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void LootUIOnOnCurrencyChanged(float before, InventoryCurrencyLookup after)
        {
            var lookup = currencies.FirstOrDefault(o => o._currencyID == after._currencyID);
            if (lookup != null)
            {
                lookup.amount = after.amount;
            }

            if (lootUI.isEmpty)
            {
                triggerer.UnUse();
                
                if (_destroyWhenEmpty)
                {
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void Used(InventoryPlayer player)
        {
            // Set items
            lootUI.Clear();
            lootUI.SetItems(items, false);
            foreach (var cur in currencies)
            {
                lootUI.AddCurrency(cur.amount, cur._currencyID);
            }

            lootUI.OnRemovedItem += LootUIOnOnRemovedItem;
            lootUI.OnCurrencyChanged += LootUIOnOnCurrencyChanged;
            lootUI.window.Show();
        }

        protected virtual void UnUsed(InventoryPlayer player)
        {
            lootUI.OnRemovedItem -= LootUIOnOnRemovedItem;
            lootUI.OnCurrencyChanged -= LootUIOnOnCurrencyChanged;
            lootUI.window.Hide();
        }
    }
}