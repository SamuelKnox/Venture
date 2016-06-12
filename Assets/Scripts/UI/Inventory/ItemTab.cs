using UnityEngine;
using UnityEngine.UI;

public class ItemTab : Tab
{
    [Tooltip("Type of item this tab contains")]
    [SerializeField]
    private ItemType itemType;

    /// <summary>
    /// Gets the item type for this category
    /// </summary>
    /// <returns>Item type</returns>
    public ItemType GetItemType()
    {
        return itemType;
    }
}