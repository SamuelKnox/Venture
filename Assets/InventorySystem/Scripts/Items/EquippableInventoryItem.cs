using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.InventorySystem.Models;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Used to represent items that can be equipped by the user, this includes armor, weapons, etc.
    /// </summary>
    public partial class EquippableInventoryItem : InventoryItemBase
    {
        #region Events

        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equippedTo"></param>
        public delegate void Equipped(InventoryEquippableField equippedTo, uint amountEquipped);

//        /// <summary>
//        /// Called when the item is equipped visually / bound to a mesh.
//        /// Useful if you wish to remove a custom component whenever the item is equipped.
//        /// </summary>
//        /// <param name="binder"></param>
//        public delegate void EquippedVisually(InventoryPlayerEquipTypeBinder binder, InventoryPlayer player);

        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public delegate void UnEquipped(uint amountUnEquipped);



        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equippedTo"></param>
        public event Equipped OnEquipped;


//        /// <summary>
//        /// Called when the item is equipped visually / bound to a mesh.
//        /// Useful if you wish to remove a custom component whenever the item is equipped.
//        /// </summary>
//        public event EquippedVisually OnEquippedVisually;
//
//
//        /// <summary>
//        /// Called when the item is unequipped visually / removed from the equipment mesh
//        /// </summary>
//        public event EquippedVisually OnUnEquippedVisually;


        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public event UnEquipped OnUnEquipped;

        #endregion




        /// <summary>
        /// EquippedItem type ID's, Manage in InventoryManager/Item Editor/EquippedItem types
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public int _equipType;
        public InventoryEquipType equipType
        {
            get
            {
                return ItemManager.database.equipTypes.FirstOrDefault(o => o.ID == _equipType);
            }
        }


        public InventoryAudioClip playOnEquip;

        /// <summary>
        /// Consider this item for visual equipment?
        /// </summary>
        public bool equipVisually = true;

        /// <summary>
        /// The position / offset used when equipping the item visually
        /// </summary>
        public Vector3 equipPosition;

        /// <summary>
        /// The rotation of the item when equipping the item visually
        /// </summary>
        public Quaternion equipRotation;


        /// <summary>
        /// Is the item currently equipped or not?
        /// </summary>
        public bool isEquipped {
            get
            {
                return equippedToField != null;
            }
        }
        public InventoryPlayer equippedToPlayer
        {
            get
            {
                if(equippedToField != null)
                {
                    return equippedToField.characterCollection.player;
                }

                return null;
            }
        }
        
        public InventoryEquippableField equippedToField { get; protected set; }


        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equipSlot"></param>
        public virtual void NotifyItemEquipped(InventoryEquippableField equipSlot, uint amountEquipped)
        {
            equippedToField = equipSlot;
            Assert.IsNotNull(equippedToPlayer, "CharacterUI's player reference not set! Forgot to assign to player?");
            Assert.IsTrue(isEquipped, "Item not considered equipped even though NotifyItemEquipped was called!!");

            SetItemProperties(equipSlot.characterCollection.player, InventoryItemUtility.SetItemPropertiesAction.Use);

            if (OnEquipped != null)
                OnEquipped(equipSlot, amountEquipped);

            InventoryAudioManager.AudioPlayOneShot(playOnEquip);
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        public virtual void NotifyItemEquippedVisually(InventoryPlayerEquipTypeBinder binder)
        {
            equippedToField = binder.associatedField;
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        public virtual void NotifyItemUnEquippedVisually(InventoryPlayerEquipTypeBinder binder)
        {
//            Assert.IsNotNull(equippedToPlayer, "Item visual unEquip failed, wasn't 'equipped' to player.");
//            Assert.IsTrue(binder.associatedField.characterCollection.player == equippedToPlayer);
        }

        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public virtual void NotifyItemUnEquipped(CharacterUI equipTo, uint amountUnEquipped)
        {
            equippedToField = null;
            SetItemProperties(equipTo.player, InventoryItemUtility.SetItemPropertiesAction.UnUse);

            if (OnUnEquipped != null)
                OnUnEquipped(amountUnEquipped);
        }

        

        protected virtual void SetItemProperties(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            // Just to be safe...
            foreach (var property in properties)
            {
                property.actionEffect = InventoryItemPropertyLookup.ActionEffect.Add;
            }

            InventoryItemUtility.SetItemProperties(player, properties, action);

            SetItemAttributeStats(player, action);
        }

        protected virtual void SetItemAttributeStats(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            // Also set the item properties for [InventoryStat] attributes.
            var fields = new List<FieldInfo>();
            InventoryUtility.GetAllFieldsInherited(GetType(), fields);
            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(InventoryStatAttribute), true).Length == 0)
                {
                    continue;
                }

                float finalValue = GetFloatValueFromObject(field.GetValue(this));
                var row = ItemManager.database.equipStats.FirstOrDefault(o => o.fieldInfoName == field.Name);
                if (row != null && row.show)
                {
                    var stat = player.characterCollection.stats.Get(row.category, row.name);
                    if (action == InventoryItemUtility.SetItemPropertiesAction.Use)
                    {
                        stat.ChangeCurrentValueRaw(finalValue);
                    }
                    else if (action == InventoryItemUtility.SetItemPropertiesAction.UnUse)
                    {
                        stat.ChangeCurrentValueRaw(-finalValue);
                    }
                }
            }
        }

        private float GetFloatValueFromObject(object val)
        {
            var floatVal = val as float?;
            // var stringVal = val as string;
            var intVal = val as int?;
            var uintVal = val as uint?;

            if (floatVal != null)
                return floatVal.Value;
            else if (intVal != null)
                return intVal.Value;
            else if (uintVal != null)
                return uintVal.Value;

            return 0f;
        }


        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
            // Currently in a equip to collection?
            if (isEquipped)
            {
                bool unEquipped = equippedToPlayer.characterCollection.UnEquipItem(this, true);
                if (unEquipped == false)
                {
                    return null;
                }
            }

            return base.Drop(location, rotation);
        }


        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;


            var prevCollection = itemCollection;
            var prevIndex = index;

            if (isEquipped)
            {
                bool unequipped = equippedToPlayer.characterCollection.UnEquipItem(this, true);
                if (unequipped)
                {
                    NotifyItemUsed(currentStackSize, false);
                    if (prevCollection != null)
                    {
                        prevCollection.NotifyItemUsed(this, ID, prevIndex, currentStackSize);
                    }

                    return 1; // Used from inside the character, move back to inventory.
                }

                return -1; // Couldn't un-unequip
            }

            var equipTo = GetBestEquipToCollection();
            if (equipTo == null)
            {
//                Debug.LogWarning("No equip collection found for item " + name + ", forgot to assign the character collection to the player?", transform);
                return -2; // No equip to collections found
            }

            var equipSlot = GetBestEquipSlot(equipTo);
            if (equipSlot == null)
            {
                Debug.LogWarning("No suitable equip slot found for item " + name, transform);    
                return -2; // No slot found
            }

            // This actually equips the item.
            var equipped = equipTo.EquipItem(equipSlot, this);
            if (equipped)
            {
                NotifyItemUsed(currentStackSize, false);
                if (prevCollection != null)
                {
                    prevCollection.NotifyItemUsed(this, ID, prevIndex, currentStackSize);
                }

                return 1;
            }

            return -1;
        }

        protected virtual CharacterUI GetBestEquipToCollection()
        {
            CharacterUI last = null;
            foreach (var col in InventoryManager.GetEquipToCollections())
            {
                var best = GetBestEquipSlot(col);
                if (best != null && (best.itemWrapper.item == null || last == null))
                {
                    last = col;
                }
            }

            return last;
        }

        /// <summary>
        /// Verifies if the item can be equipped or not.
        /// This is validated after CanSetItem, so the item can be rejected before it gets here, if it doesn't match onlyAllowTypes.
        /// </summary>
        /// <returns>True if the item can be equipped, false if not.</returns>
        public virtual bool CanEquip(CharacterUI equipTo)
        {
            if(CanUse() == false)
            {
                return false;
            }
            
            return GetBestEquipSlot(equipTo) != null;
        }

        public virtual bool CanUnEquip(bool addToInventory)
        {
            if (addToInventory)
            {
                return InventoryManager.CanAddItem(this);
            }

            return true;
        }

        public bool Equip(InventoryEquippableField equipSlot)
        {
            return equipSlot.characterCollection.EquipItem(equipSlot, this);
        }

        public bool UnEquip(bool addItemToInventory)
        {
            return equippedToPlayer.characterCollection.UnEquipItem(this, addItemToInventory);
        }

        public virtual InventoryEquippableField GetBestEquipSlot(CharacterUI equipCollection)
        {
            Assert.IsNotNull(equipCollection);

            var equipSlots = equipCollection.GetEquippableSlots(this);
            if (equipSlots.Length == 0)
            {
//                Debug.LogWarning("No suitable equip slot found for item " + name, gameObject);
                return null;
            }

            InventoryEquippableField equipSlot = equipSlots[0];
            foreach (var e in equipSlots)
            {
                if (equipCollection[e.index].item == null)
                {
                    equipSlot = e; // Prefer an empty slot over swapping a filled one.
                    break;
                }
            }

            return equipSlot;
        }
    }
}