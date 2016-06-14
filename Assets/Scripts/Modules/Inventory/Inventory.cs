using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Tooltip("Item Containers store items of a specific Item Type")]
    [SerializeField]
    private ItemContainer[] itemContainers;

    void Awake()
    {
        foreach (var itemContainer in itemContainers)
        {
            var activeItem = itemContainer.GetActiveItem();
            if (!activeItem)
            {
                continue;
            }
            if (!itemContainer.Contains(activeItem))
            {
                Debug.LogWarning(itemContainer.GetName() + " does not contain its active item, " + activeItem.name + ", so it is being added.", gameObject);
                itemContainer.Add(activeItem);
            }
        }
    }

    void Reset()
    {
        itemContainers = new ItemContainer[EnumUtility.Count<ItemType>()];
        for (int i = 0; i < itemContainers.Length; i++)
        {
            itemContainers[i] = new ItemContainer((ItemType)i);
        }
    }

    void OnValidate()
    {
        foreach (var itemContainer in itemContainers)
        {
            var itemType = itemContainer.GetItemType();
            var activeItem = itemContainer.GetActiveItem();
            if (activeItem && activeItem.GetItemType() != itemType)
            {
                Debug.LogWarning(activeItem.name + " is not of the correct type, " + itemType + ", for " + itemContainer.GetName());
                itemContainer.ClearActiveItem();
            }
            foreach (var item in itemContainer.GetItems())
            {
                if (item.GetItemType() != itemType)
                {
                    Debug.LogWarning(item.name + " is not of the correct type, " + itemType + ", for " + itemContainer.GetName());
                    itemContainer.Remove(item);
                }
            }
        }
    }

    /// <summary>
    /// Gets the items of the specified type
    /// </summary>
    /// <param name="itemType">Item type</param>
    /// <returns>Items</returns>
    public Item[] GetItems(ItemType itemType)
    {
        return GetItemContainer(itemType).GetItems();
    }

    /// <summary>
    /// Returns the count of items in the item container of the type specified
    /// </summary>
    /// <param name="itemType">Type of item container</param>
    /// <returns>Number of items</returns>
    public int Count(ItemType itemType)
    {
        var itemContainer = GetItemContainer(itemType);
        if (itemContainer == null)
        {
            return 0;
        }
        return itemContainer.Count();
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns>Whether or not the add was successful</returns>
    public bool Add(Item item)
    {
        if (!item)
        {
            Debug.LogError("You cannot add a null Item to inventory!", gameObject);
            return false;
        }
        var itemContainer = GetItemContainer(item.GetItemType());
        if (itemContainer == null)
        {
            Debug.LogError("The Item Container for " + item.GetItemType() + " is missing!", gameObject);
        }
        bool success = itemContainer.Add(item);
        if (success)
        {
            item.tag = gameObject.tag;
        }
        return success;
    }

    /// <summary>
    /// Checks if the inventory contains an item
    /// </summary>
    /// <param name="item">Item to check for</param>
    /// <returns>Whether or not inventory contains item</returns>
    public bool Contains(Item item)
    {
        if (!item)
        {
            Debug.LogWarning("You are checking if the inventory contains a null Item.", gameObject);
            return false;
        }
        var itemContainer = GetItemContainer(item.GetItemType());
        if (itemContainer == null)
        {
            return false;
        }
        return itemContainer.Contains(item);
    }

    /// <summary>
    /// Gets the active item for a specified item type
    /// </summary>
    /// <param name="itemType">Type of item to get</param>
    /// <returns>The active item</returns>
    public Item GetActiveItem(ItemType itemType)
    {
        var itemContainer = GetItemContainer(itemType);
        if (itemContainer != null)
        {
            return itemContainer.GetActiveItem();
        }
        return null;
    }


    /// <summary>
    /// Sets the active item for the category
    /// </summary>
    /// <param name="item">Item to set</param>
    /// <returns>Whether or not the item was successfully set as the active item</returns>
    public bool SetActiveItem(Item item)
    {
        if (!item)
        {
            Debug.LogError("Attempted to set null item as the active item!", gameObject);
        }
        if (!Contains(item))
        {
            Debug.LogError("Attempting to set " + item.name + " as the active item, but it is not contained in " + name + "!", gameObject);
        }
        return GetItemContainer(item.GetItemType()).SetActiveItem(item);
    }

    /// <summary>
    /// Gets the item container for the specified item type
    /// </summary>
    /// <param name="itemType">Type of container to get</param>
    /// <returns>Item Container</returns>
    private ItemContainer GetItemContainer(ItemType itemType)
    {
        return itemContainers.Where(c => c.GetItemType() == itemType).FirstOrDefault();
    }

    [Serializable]
    private class ItemContainer
    {
        [HideInInspector]
        [SerializeField]
        private string name;

        [Tooltip("Currently active item")]
        [SerializeField]
        private Item activeItem;

        [Tooltip("All items in the container")]
        [SerializeField]
        private List<Item> items;

        [HideInInspector]
        [SerializeField]
        private ItemType itemType;

        /// <summary>
        /// Creates a new item container
        /// </summary>
        /// <param name="itemType">Type of item container</param>
        public ItemContainer(ItemType itemType)
        {
            this.itemType = itemType;
            name = itemType.ToString().AddSpacing();
        }

        /// <summary>
        /// Gets the items in this item container
        /// </summary>
        /// <returns>Items</returns>
        public Item[] GetItems()
        {
            return items.ToArray();
        }

        /// <summary>
        /// Sets the active item for this item container
        /// </summary>
        /// <param name="item">Item to set</param>
        /// <returns>Whether or not</returns>
        public bool SetActiveItem(Item item)
        {
            if (!item)
            {
                Debug.LogError("You cannot set the active item to null!");
                Debug.Log("Use ClearActiveItem(Item) instead.");
            }
            if (!Contains(item) || item.GetItemType() != GetItemType())
            {
                return false;
            }
            activeItem = item;
            return true;
        }

        /// <summary>
        /// Removes the active item, setting it to null
        /// </summary>
        public void ClearActiveItem()
        {
            activeItem = null;
        }

        /// <summary>
        /// Gets the name of this item container
        /// </summary>
        /// <returns>The name</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Adds an item to the item container
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Whether or not the add was successful</returns>
        public bool Add(Item item)
        {
            if (Contains(item))
            {
                return false;
            }
            items.Add(item);
            return true;
        }

        /// <summary>
        /// Removes an item from the item container
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>Whether or not the removal was successful</returns>
        public bool Remove(Item item)
        {
            if (!Contains(item))
            {
                return false;
            }
            return items.Remove(item);
        }

        /// <summary>
        /// Gets the count of the items in this item container
        /// </summary>
        /// <returns>Number of items</returns>
        public int Count()
        {
            return items.Count;
        }

        /// <summary>
        /// Checks if the item container contains the specified item
        /// </summary>
        /// <param name="item">Item to check for</param>
        /// <returns>Whether or not the item container contains the item</returns>
        public bool Contains(Item item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Gets the type of item this item container holds
        /// </summary>
        /// <returns>Type of Item Container</returns>
        public ItemType GetItemType()
        {
            return itemType;
        }

        /// <summary>
        /// Gets the active item for this container
        /// </summary>
        /// <returns>Active item</returns>
        public Item GetActiveItem()
        {
            return activeItem;
        }
    }
}