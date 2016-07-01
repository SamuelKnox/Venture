using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Player/Inventory player")]
    public partial class InventoryPlayer : InventoryPlayerBase
    {
        public delegate void PickedUpItem(uint itemID, uint itemAmount);
        public delegate void PlayerDied(LootableObject dropObject);

        public event PickedUpItem OnPickedUpItem;
        public event PlayerDied OnPlayerDied;
        


        [SerializeField]
        private bool isPlayerDynamicallyInstantiated = false;

        public bool dontDestroyOnLoad = true;

        [SerializeField]
        private LootableObject deathDropObjectPrefab;


        private InventoryPlayerRangeHelper _rangeHelper;
        public InventoryPlayerRangeHelper rangeHelper
        {
            get
            {
                if (_rangeHelper == null)
                {
                    var comps = GetComponentsInChildren<InventoryPlayerRangeHelper>(true); // GetComponentInChildren (single) doesn't grab in-active objects.
                    _rangeHelper = comps.Length > 0 ? comps[0] : null;
                }

                return _rangeHelper;
            }
            protected set { _rangeHelper = value; }
        }

        public InventoryPlayerEquipHelper equipHelper { get; set; }
        public bool isInitialized { get; protected set; }

        /// <summary>
        /// Initialize this player. The player will be added to the players list ( assigned to the InventoryPlayerManager )
        /// </summary>
        public void Init()
        {
            Assert.IsFalse(isInitialized, "Tried to initialize player - Player was already initialized!");
            isInitialized = true;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this);
            }

            if (dynamicallyFindUIElements)
            {
                FindUIElements();
            }

            if (characterCollection != null)
            {
                characterCollection.player = this;
            }

            UpdateEquipLocations();
            equipHelper = new InventoryPlayerEquipHelper(this);
            InventoryPlayerManager.AddPlayer(this);
        }
        
        protected override void Awake()
        {
            base.Awake();

            if (isPlayerDynamicallyInstantiated == false)
            {
                Init();
            }
        }

        public void NotifyPickedUpItem(uint itemID, uint itemAmount)
        {
            if (OnPickedUpItem != null)
                OnPickedUpItem(itemID, itemAmount);
        }

        public LootableObject NotifyPlayerDied(bool dropAll)
        {
            return NotifyPlayerDied(dropAll, dropAll, dropAll, transform.position);
        }

        public LootableObject NotifyPlayerDied(bool clearInventories, bool clearCharacter, bool putAllItemsInDropObject, Vector3 dropPosition)
        {
            var playerCollections = new List<ItemCollectionBase>();
            if (clearInventories)
            {
                playerCollections.AddRange(inventoryCollections);
            }

            if (clearCharacter)
            {
                playerCollections.Add(characterCollection);
            }

            LootableObject dropObj = null;
            if (putAllItemsInDropObject)
            {
                var itemsInCols = new List<InventoryItemBase>();
                var currencies = new List<InventoryCurrencyLookup>();
                foreach (var col in playerCollections)
                {
                    itemsInCols.AddRange(col.items.Select(o => o.item).Where(o => o != null));
                    currencies.AddRange(col.currenciesContainer.lookups);
                }

                if (itemsInCols.Count > 0 || currencies.Count > 0)
                {
                    Assert.IsNotNull(deathDropObjectPrefab, "Player died, trying to drop object with all the player's items, but drop object prefab is not set.");
                    dropObj = Instantiate<LootableObject>(deathDropObjectPrefab);
                    dropObj.items = itemsInCols.ToArray();
                    dropObj.currencies = currencies.ToArray();

                    dropObj.transform.position = transform.position;
                    dropObj.transform.rotation = Quaternion.identity;
                }
            }

            foreach (var col in playerCollections)
            {
                col.Clear();
            }

            if (OnPlayerDied != null)
                OnPlayerDied(dropObj);

            return dropObj;
        }
        
//        public void SetActive(bool active)
//        {
//            this.enabled = active;
//            this.rangeHelper.enabled = active;
//
//            var userControl = gameObject.GetComponent<IInventoryPlayerController>();
//            if (userControl == null)
//            {
//                Debug.LogWarning("No component found on player that implements IInventoryPlayerController. If you implement your own controller, be sure to implement IInventoryPlayerController.", transform);
//                return;
//            }
//
//            userControl.SetActive(active);
//        }


        /// <summary>
        /// For collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter(Collider col)
        {
            TryPickup(col.gameObject);
        }


        /// <summary>
        /// For 2D collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            TryPickup(col.gameObject);
        }

        /// <summary>
        /// Collision pickup attempts
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void TryPickup(GameObject obj)
        {
            // Just for safety in-case the collision matrix isn't set up correctly..
            if (obj.layer == InventorySettingsManager.instance.equipmentLayer)
                return;

            if (InventorySettingsManager.instance.itemTriggerOnPlayerCollision || CanPickupGold(obj))
            {
                var item = obj.GetComponent<ObjectTriggererItem>();
                if (item != null)
                    item.Use(this);
            }
        }

        protected virtual bool CanPickupGold(GameObject obj)
        {
            return InventorySettingsManager.instance.alwaysTriggerGoldItemPickupOnPlayerCollision && obj.GetComponent<CurrencyInventoryItem>() != null;
        }

        /// <summary>
        /// Add the range helper this object depends on.
        /// </summary>
        public void AddRangeHelper()
        {
            var col = new GameObject("_Col");
            col.transform.SetParent(transform);
            InventoryUtility.ResetTransform(col.transform);

            col.gameObject.AddComponent<InventoryPlayerRangeHelper>();
        }
    }
}
