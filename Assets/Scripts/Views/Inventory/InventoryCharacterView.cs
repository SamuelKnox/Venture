using CustomUnityLibrary;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryCharacterView : MonoBehaviour
{
    [Tooltip("Container storing the ItemButtons for the character")]
    [SerializeField]
    private RectTransform characterWeaponContainer;

    [Tooltip("Description to give for item when there is no weapon selected")]
    [SerializeField]
    private string noWeaponDescription;

    [Tooltip("Short description for the item type selected")]
    [SerializeField]
    private TextMeshProUGUI description;

    private Inventory inventory;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (!inventory)
        {
            Debug.Log(gameObject + " could not find Inventory!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Enables (or disables) navigation for the weapon view's item buttons
    /// </summary>
    /// <param name="enabled">Whether or not to enable navigation</param>
    public void EnableNavigation(bool enabled)
    {
        var navigationMode = enabled ? Navigation.Mode.Automatic : Navigation.Mode.None;
        var buttons = characterWeaponContainer.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.SetNavigation(navigationMode);
        }
    }

    /// <summary>
    /// Updates the description for the equipped item
    /// </summary>
    /// <param name="item">Item to describe</param>
    public void UpdateDescription(Weapon weapon)
    {
        string itemDescription;
        if (weapon)
        {
            itemDescription = weapon.GetDescription();
            if (string.IsNullOrEmpty(itemDescription))
            {
                Debug.LogError(weapon + " is missing a description!", weapon.gameObject);
                return;
            }
        }
        else
        {
            itemDescription = noWeaponDescription;
            if (string.IsNullOrEmpty(itemDescription))
            {
                Debug.LogError(this + " is missing a description for when there is no item!", gameObject);
                return;
            }
        }
        description.text = itemDescription;
    }

    /// <summary>
    /// Updates the item buttons for the character view
    /// </summary>
    /// <param name="itemType">Item type to be currently selected</param>
    public void UpdateWeapon(ItemType itemType = ItemType.MeleeWeapon)
    {
        var itemButtons = characterWeaponContainer.GetComponentsInChildren<ItemButton>();
        foreach (var itemButtonInstance in itemButtons)
        {
            var item = inventory.GetItems(itemButtonInstance.GetItemType()).Where(i => i.IsEquipped()).FirstOrDefault();
            itemButtonInstance.SetItem(item);
        }
        var equippedItemSelection = itemButtons.Where(b => b.GetItemType() == itemType).First();
        EventSystem.current.SetSelectedGameObject(equippedItemSelection.gameObject);
    }

    /// <summary>
    /// Gets the Item Button for the specified type
    /// </summary>
    /// <param name="itemType">Type of item to get button for</param>
    /// <returns>The item button</returns>
    public ItemButton GetItemButton(ItemType itemType)
    {
        var itemButtons = characterWeaponContainer.GetComponentsInChildren<ItemButton>();
        return itemButtons.Where(b => b.GetItemType() == itemType).FirstOrDefault();
    }
}