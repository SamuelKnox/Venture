using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryEquipmentView : MonoBehaviour
{
    [Tooltip("Button prefab to show items")]
    [SerializeField]
    private ItemButton itemButton;

    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform equipmentContainer;

    [Tooltip("Container for displaying equipment description")]
    [SerializeField]
    private Transform descriptionContainer;

    [Tooltip("Container for dispalying description when there is no equipment to describe")]
    [SerializeField]
    private Transform noDescriptionContainer;

    [Tooltip("Short description for the equipment selected")]
    [SerializeField]
    private TextMeshProUGUI description;

    [Tooltip("Rune sockets used to display equipment runes")]
    [SerializeField]
    private RuneSocket[] runeSockets;

    private Inventory inventory;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (!inventory)
        {
            Debug.LogError(gameObject + " could not find Inventory!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Updates the UI based on the newly selected GameObject
    /// </summary>
    /// <param name="selection">New selection</param>
    public void UpdateSelection(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting an Item, but none was found!", gameObject);
            return;
        }
        var itemType = item.GetItemType();
        CreateButtons(itemType);
    }

    /// <summary>
    /// Creates the item buttons for the equippable equipment of the type specified
    /// </summary>
    /// <param name="itemType">Type of equipment to be equipped</param>
    /// <returns>Whether or not any of that equipment type was found to create buttons for</returns>
    public bool CreateButtons(ItemType itemType)
    {
        foreach (var button in equipmentContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        var equipment = inventory.GetItems(itemType);
        if (equipment.Length == 0)
        {
            return false;
        }
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var equipmentPiece in equipment)
        {
            var button = Instantiate(itemButton);
            button.transform.SetParent(equipmentContainer);
            button.SetItemType(itemType);
            button.SetItem(equipmentPiece);
            if (!EventSystem.current.currentSelectedGameObject || equipmentPiece.IsEquipped())
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
        return true;
    }

    /// <summary>
    /// Updates the long description for the piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment to get description for</param>
    public void UpdateDescription(Equipment equipment)
    {
        if (equipment)
        {
            descriptionContainer.gameObject.SetActive(true);
            noDescriptionContainer.gameObject.SetActive(false);
            description.text = equipment.GetDescription();
            foreach (var runeSocket in runeSockets)
            {
                bool containsSocket = Array.IndexOf(equipment.GetRuneSocketTypes(), runeSocket.GetRuneType()) >= 0;
                if (containsSocket)
                {
                    var runeIcon = runeSocket.GetIcon();
                    var rune = equipment.GetRune(runeSocket.GetRuneType());
                    runeIcon.sprite = rune ? rune.GetIcon() : null;
                    runeIcon.enabled = runeIcon.sprite;
                }
                runeSocket.gameObject.SetActive(containsSocket);
            }
        }
        else
        {
            descriptionContainer.gameObject.SetActive(false);
            noDescriptionContainer.gameObject.SetActive(true);
        }
    }
}