using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Item : MonoBehaviour
{
    [Tooltip("Type of item")]
    [SerializeField]
    private ItemType itemType;

    [Tooltip("Sockets for runes")]
    [SerializeField]
    private RuneSocket[] runeSockets;

    [Tooltip("Description of this item")]
    [SerializeField]
    private string description;

    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(LayerNames.Trigger);
    }

    /// <summary>
    /// Gets the rune sockets for this item
    /// </summary>
    /// <returns>Rune sockets</returns>
    public RuneSocket[] GetRuneSockets()
    {
        return runeSockets;
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
    /// Gets the description for this item
    /// </summary>
    /// <returns>Item description</returns>
    public string GetDescription()
    {
        return description;
    }
}