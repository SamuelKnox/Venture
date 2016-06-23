using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Item : MonoBehaviour
{
    [Tooltip("Type of item")]
    [SerializeField]
    private ItemType itemType;

    [Tooltip("Whether or not this item is equipped")]
    [SerializeField]
    private bool equipped;

    [Tooltip("Icon associated with this item")]
    [SerializeField]
    private Sprite icon;

    [Tooltip("Description of this item")]
    [SerializeField]
    private string description;

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
    /// Checks whether or not this item is equipped
    /// </summary>
    /// <returns>Whether or not this is an equipped item</returns>
    public bool IsEquipped()
    {
        return equipped;
    }

    /// <summary>
    /// Sets whether or not this is an equipped item
    /// </summary>
    /// <param name="equipped">Whether or not to set this item as equipped</param>
    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;
    }

    /// <summary>
    /// Gets the icon associated with this item
    /// </summary>
    /// <returns>Item's icon sprite</returns>
    public Sprite GetIcon()
    {
        return icon;
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