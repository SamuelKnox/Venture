using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
    public partial class InventoryPlayerEquipHelper
    {
        public enum EquipHandlerType
        {
            /// <summary>
            /// Replace the equip location's mesh with the new model.
            /// </summary>
            Replace,

            /// <summary>
            /// Make the model a child of the equip location.
            /// </summary>
            MakeChild
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="item"></param>
        public delegate void EquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item);
        public event EquippedVisually OnEquippedVisually;
        public event EquippedVisually OnUnEquippedVisually;


        protected InventoryPlayer player;

        public InventoryPlayerEquipHelper(InventoryPlayer player)
        {
            this.player = player;


            if (player.characterCollection == null)
            {
                Debug.Log("Visual equipment disabled, no character collection assigned to player.", player.gameObject);
                return;
            }

            player.characterCollection.OnAddedItem += (items, amount, collection) =>
            {
                var item = items.FirstOrDefault();
                if (item is EquippableInventoryItem)
                {
                    var equippableField = player.characterCollection.equipSlotFields[item.index];
                    if (player.characterCollection.useReferences)
                    {
                        foreach (var wrapper in player.characterCollection)
                        {
                            if (wrapper.item == item)
                            {
                                equippableField = player.characterCollection.equipSlotFields[wrapper.index];
                            }
                        }
                    }

                    EquipItemVisually((EquippableInventoryItem)item, equippableField);
                }
            };
            player.characterCollection.OnRemovedItem += (item, id, slot, amount) =>
            {
                if (item is EquippableInventoryItem)
                {
                    var equippableField = player.characterCollection.equipSlotFields[slot];
                    if (player.characterCollection.useReferences)
                    {
                        foreach (var wrapper in player.characterCollection)
                        {
                            if (wrapper.item == item)
                            {
                                equippableField = player.characterCollection.equipSlotFields[wrapper.index];
                            }
                        }
                    }

                    UnEquipItemVisually((EquippableInventoryItem)item, equippableField);
                }
            };
            player.characterCollection.OnSwappedItems += (fromCollection, fromSlot, toCollection, toSlot) =>
            {
                if (fromCollection is CharacterUI == false || toCollection is CharacterUI == false)
                    return;

                // Items are already swapped here...

                var fromItem = (EquippableInventoryItem)fromCollection[fromSlot].item;
                var fromCol = (CharacterUI)fromCollection;

                var toItem = (EquippableInventoryItem)toCollection[toSlot].item;
                var toCol = (CharacterUI)toCollection;

                UnEquipItemVisually(toItem, fromCol.equipSlotFields[fromSlot]);

                // Remove from old position
                if (fromItem != null)
                {
                    UnEquipItemVisually(fromItem, toCol.equipSlotFields[toSlot]);
                }

                EquipItemVisually(toItem, toCol.equipSlotFields[toSlot]);

                if (fromItem != null)
                {
                    EquipItemVisually(fromItem, fromCol.equipSlotFields[fromSlot]);
                }
            };
//            if (player.characterCollection.useReferences)
//            {
//                player.characterCollection.OnSetItem += (slot, item) =>
//                {
//                    
//                };
//            }
        }


        private void NotifyItemEquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);
            Assert.IsNotNull(binder);

            if (OnEquippedVisually != null)
                OnEquippedVisually(binder, item);

           item.NotifyItemEquippedVisually(binder);
        }

        private void NotifyItemUnEquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item)
        {
            Assert.IsNotNull(item);
            Assert.IsNotNull(binder);

            if (OnUnEquippedVisually != null)
                OnUnEquippedVisually(binder, item);

           item.NotifyItemUnEquippedVisually(binder);
        }

        public virtual InventoryPlayerEquipTypeBinder FindEquipLocation(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            return player.equipLocations.FirstOrDefault(o => o.equipTransform != null && o.associatedField == equippableField);
        }

        public virtual void EquipItemVisually(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            if (item.equipVisually == false)
                return;

            var equipLocation = FindEquipLocation(item, equippableField);

            if (equipLocation != null)
            {
                var copy = UnityEngine.Object.Instantiate<EquippableInventoryItem>(item);

                // Remove the default components, to prevent the user from looting an equipped item.
                Object.Destroy(copy.gameObject.GetComponent<ObjectTriggererItem>());
                Object.Destroy(copy.gameObject.GetComponent<InventoryItemBase>());

                var rigid = copy.gameObject.GetComponent<Rigidbody>();
                if (rigid != null)
                    rigid.isKinematic = true;

                var rigid2D = copy.gameObject.GetComponent<Rigidbody2D>();
                if (rigid2D != null)
                    rigid2D.isKinematic = true;

                var cols = copy.gameObject.GetComponentsInChildren<Collider>(true);
                foreach (var col in cols)
                    col.isTrigger = true;

                var cols2D = copy.gameObject.GetComponentsInChildren<Collider2D>(true);
                foreach (var col2D in cols2D)
                    col2D.isTrigger = true;

                copy.gameObject.SetActive(true);
                InventoryUtility.SetLayerRecursive(copy.gameObject, InventorySettingsManager.instance.equipmentLayer);
                HandleEquipType(copy, equipLocation);
                return;
            }

            //Debug.LogWarning("No suitable visual equip location found", item.transform);
        }


        public virtual void HandleEquipType(EquippableInventoryItem copy, InventoryPlayerEquipTypeBinder binder)
        {
            switch (binder.equipHandlerType)
            {
                case EquipHandlerType.MakeChild:

                    copy.transform.SetParent(binder.equipTransform);
                    copy.transform.localPosition = copy.equipPosition;
                    copy.transform.localRotation = copy.equipRotation;

                    break;
                case EquipHandlerType.Replace:

                    copy.transform.SetParent(binder.equipTransform.parent); // Same level
                    copy.transform.SetSiblingIndex(binder.equipTransform.GetSiblingIndex());
                    binder.equipTransform.gameObject.SetActive(false); // Swap the item by disabling the original

                    copy.transform.localPosition = copy.equipPosition;
                    copy.transform.localRotation = copy.equipRotation;

                    break;
                default:
                    Debug.LogWarning("No visual equip handler found for item " + copy.name, copy);
                    return; // Return to avoid the notify event handler
            }

            binder.currentItem = copy.gameObject;
            NotifyItemEquippedVisually(binder, copy);
        }

        public void UnEquipItemVisually(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            var equipLocation = FindEquipLocation(item, equippableField);
            UnEquipItemVisually(item, equipLocation);
        }

        public virtual void UnEquipItemVisually(EquippableInventoryItem item, InventoryPlayerEquipTypeBinder binder)
        {
            if (binder != null && binder.currentItem != null)
            {
                switch (binder.equipHandlerType)
                {
                    case EquipHandlerType.MakeChild:

                        UnityEngine.Object.Destroy(binder.currentItem);
                        break;
                    case EquipHandlerType.Replace:

                        binder.equipTransform.gameObject.SetActive(true); // Re-enable the original
                        break;
                    default:
                        Debug.LogWarning("No visual unequip handler found for item");
                        return; // Return to avoid the notify event handler
                }

                NotifyItemUnEquippedVisually(binder, item);
            }
        }


    }
}
