using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemButton : MonoBehaviour {
    [Tooltip("Item which this button represents")]
    [SerializeField]
    private Item item;

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
    public void SetItem(Item item)
    {
        this.item = item;
    }
}