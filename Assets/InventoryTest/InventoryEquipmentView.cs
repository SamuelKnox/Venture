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

    [Tooltip("Description to give for item when there is no equipment selected")]
    [SerializeField]
    private string noEquipmentDescription;

    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform equipmentContainer;

    [Tooltip("Short description for the equipment selected")]
    [SerializeField]
    private TextMeshProUGUI description;

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
            if (!EventSystem.current.currentSelectedGameObject)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
        return true;
    }

    /// <summary>
    /// Updates the description for the equippable item
    /// </summary>
    /// <param name="item">equipment to describe</param>
    public void UpdateDescription(Equipment equipment)
    {
        string equipmentDescription;
        if (equipment)
        {
            equipmentDescription = equipment.GetDescription();
            if (string.IsNullOrEmpty(equipmentDescription))
            {
                Debug.LogError(equipment + " is missing a description!", equipment.gameObject);
                return;
            }
        }
        else
        {
            equipmentDescription = noEquipmentDescription;
            if (string.IsNullOrEmpty(equipmentDescription))
            {
                Debug.LogError(this + " is missing a description for when there is no item!", gameObject);
                return;
            }
        }
        description.text = equipmentDescription;
    }
}