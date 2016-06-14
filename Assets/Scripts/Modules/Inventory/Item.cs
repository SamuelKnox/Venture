using System;
using System.Linq;
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

    void OnValidate()
    {
        CheckForDuplicateSockets();
    }

    /// <summary>
    /// Sets the rune for this item's respective rune socket
    /// </summary>
    /// <param name="rune"></param>
    public void SetRune(Rune rune)
    {
        var runeSocket = runeSockets.Where(s => s.GetRuneType() == rune.GetRuneType()).FirstOrDefault();
        if (runeSocket == null)
        {
            Debug.LogError("There is no rune socket that can hold " + rune + " in " + gameObject + "!", gameObject);
        }
        runeSocket.SetRune(rune);
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
    /// Gets the types of runes this item can hold, as determined by its rune sockets
    /// </summary>
    /// <returns>Types of runes this item can hold</returns>
    public RuneType[] GetRuneTypes()
    {
        return runeSockets.Select(s => s.GetRuneType()).ToArray();
    }

    /// <summary>
    /// Gets the description for this item
    /// </summary>
    /// <returns>Item description</returns>
    public string GetDescription()
    {
        return description;
    }

    /// <summary>
    /// Asserts that there is only one type of each rune socket at most
    /// </summary>
    private void CheckForDuplicateSockets()
    {
        var runeTypes = runeSockets.Select(s => s.GetRuneType());
        if (runeTypes.Count() != runeTypes.Distinct().Count())
        {
            Debug.LogError("You cannot have more than one Rune Socket of the same type on one item!", gameObject);
        }
    }

    [Serializable]
    private class RuneSocket
    {
        [Tooltip("Type of rune this socket can hold")]
        [SerializeField]
        private RuneType runeType;

        [Tooltip("Rune in this socket")]
        [SerializeField]
        private Rune rune;

        /// <summary>
        /// Gets the type of rune this socket can hold
        /// </summary>
        /// <returns>Rune socket type</returns>
        public RuneType GetRuneType()
        {
            return runeType;
        }

        /// <summary>
        /// Sets the rune for this socket
        /// </summary>
        /// <param name="rune">Rune to set</param>
        public void SetRune(Rune rune)
        {
            this.rune = rune;
        }
    }
}