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
    /// Gets all items, regardless of type
    /// </summary>
    /// <returns>All items</returns>
    public Item[] GetItems()
    {
        var items = new List<Item>();
        foreach (var itemContainer in itemContainers)
        {
            items.AddRange(itemContainer.GetItems());
        }
        return items.ToArray();
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
            return false;
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
    }
}