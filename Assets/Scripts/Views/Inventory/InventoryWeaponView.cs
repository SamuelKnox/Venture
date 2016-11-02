using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryWeaponView : MonoBehaviour
{
    [Tooltip("Button prefab to show items")]
    [SerializeField]
    private ItemButton itemButton;

    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform weaponContainer;

    [Tooltip("Container for displaying weapon description")]
    [SerializeField]
    private Transform descriptionContainer;

    [Tooltip("Container for dispalying description when there is no weapon to describe")]
    [SerializeField]
    private Transform noDescriptionContainer;

    [Tooltip("Short description for the weapon selected")]
    [SerializeField]
    private TextMeshProUGUI description;

    [Tooltip("Roon sockets used to display weapon roons")]
    [SerializeField]
    private RoonSocket[] roonSockets;

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
    /// Creates the item buttons for the equippable weapon of the type specified
    /// </summary>
    /// <param name="itemType">Type of weapon to be equipped</param>
    /// <returns>Whether or not any of that weapon type was found to create buttons for</returns>
    public bool CreateButtons(ItemType itemType)
    {
        foreach (var button in weaponContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        var weapons = inventory.GetItems(itemType);
        if (weapons.Length == 0)
        {
            return false;
        }
        EventSystem.current.SetSelectedGameObject(null);
        foreach (var weapon in weapons)
        {
            var button = Instantiate(itemButton);
            button.transform.SetParent(weaponContainer);
            button.SetItemType(itemType);
            button.SetItem(weapon);
            if (!EventSystem.current.currentSelectedGameObject || weapon.IsEquipped())
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
        return true;
    }

    /// <summary>
    /// Updates the long description for the piece of weapon
    /// </summary>
    /// <param name="weapon">Weapon to get description for</param>
    public void UpdateDescription(Weapon weapon)
    {
        if (weapon)
        {
            descriptionContainer.gameObject.SetActive(true);
            noDescriptionContainer.gameObject.SetActive(false);
            description.text = weapon.GetDescription();
            foreach (var roonSocket in roonSockets)
            {
                bool containsSocket = Array.IndexOf(weapon.GetRoonSocketTypes(), roonSocket.GetRoonType()) >= 0;
                if (containsSocket)
                {
                    var roonIcon = roonSocket.GetIcon();
                    var roon = weapon.GetRoon(roonSocket.GetRoonType());
                    roonIcon.sprite = roon ? roon.GetIcon() : null;
                    roonIcon.enabled = roonIcon.sprite;
                }
                roonSocket.gameObject.SetActive(containsSocket);
            }
        }
        else
        {
            descriptionContainer.gameObject.SetActive(false);
            noDescriptionContainer.gameObject.SetActive(true);
        }
    }
}