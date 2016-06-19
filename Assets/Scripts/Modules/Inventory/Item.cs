using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Item : MonoBehaviour
{
    [Tooltip("Type of item")]
    [SerializeField]
    private ItemType itemType;

    [Tooltip("Description of this item")]
    [SerializeField]
    private string description;

    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(LayerNames.Trigger);
    }

    /// <summary>
    /// Gets the Item's type
    /// </summary>
    /// <returns>Item's type</returns>
    public ItemType GetItemType()
    {
        return itemType;
    }

    /// <summary>
    /// Sets the item type for this item
    /// </summary>
    /// <param name="itemType">Type of item</param>
    public void SetItemType(ItemType itemType)
    {
        this.itemType = itemType;
    }

    /// <summary>
    /// Gets the description for this item
    /// </summary>
    /// <returns>Item description</returns>
    public string GetDescription()
    {
        return description;
    }
}