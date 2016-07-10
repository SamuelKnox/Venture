using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemButton : MonoBehaviour
{
    [Tooltip("Item which this button represents")]
    [SerializeField]
    private Item item;

    [Tooltip("Type of item which can fit in the item button")]
    [SerializeField]
    private ItemType itemType;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        SetButtonIcon();
    }

    /// <summary>
    /// Gets the type of item this item button holds
    /// </summary>
    /// <returns>Type of item</returns>
    public ItemType GetItemType()
    {
        return itemType;
    }

    /// <summary>
    /// Changes the type of item this item button can hold
    /// </summary>
    /// <param name="itemType">New item type</param>
    public void SetItemType(ItemType itemType)
    {
        this.itemType = itemType;
    }

    /// <summary>
    /// Gets the item represented by this Item Button
    /// </summary>
    /// <returns>Item being represented</returns>
    public Item GetItem()
    {
        return item;
    }

    /// <summary>
    /// Sets the item associated with this item button
    /// </summary>
    /// <param name="item">Item which Item Button represents</param>
    /// <returns>Whether or not the item was successfully set</returns>
    public bool SetItem(Item item)
    {
        if(item && GetItemType() != item.GetItemType())
        {
            return false;
        }
        this.item = item;
        SetButtonIcon();
        return true;
    }

    /// <summary>
    /// Sets the icon for the item
    /// </summary>
    private void SetButtonIcon()
    {
        var item = GetItem();
        if (item)
        {
            var image = button.GetComponentsInChildren<Image>().Where(i => i.gameObject != gameObject).FirstOrDefault();
            image.sprite = item.GetIcon();
        }
    }
}