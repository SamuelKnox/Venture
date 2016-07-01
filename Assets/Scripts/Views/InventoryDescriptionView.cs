using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class InventoryDescriptionView : MonoBehaviour
{
    private const string EquippedItemSuffix = " is currently equipped.";
    private const string RuneSocketedElsewhereAffix = " currently has ";
    private const string RuneSocketedElsewhereSuffix = " socketed.";

    [Tooltip("Image used to show the item which is currently equipped")]
    [SerializeField]
    private Image equippedItemImage;

    [Tooltip("Text used to show the description for the currently equipped item")]
    [SerializeField]
    private Text equippedItemDescription;

    [Tooltip("Image used to show the item which is being browsed")]
    [SerializeField]
    private Image browserItemImage;

    [Tooltip("Text used to show the description for the item currently being browsed")]
    [SerializeField]
    private Text browserItemDescription;

    [Tooltip("Text used to display whether or not an item is already equipped for the browser description")]
    [SerializeField]
    private Text browserEquipmentStatus;

    private Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find Player!", gameObject);
            return;
        }
    }
    
    /// <summary>
    /// Updates the description for the currently selected item
    /// </summary>
    public void UpdateItemDescriptions(GameObject currentSelectedGameObject, Equipment activeEquipment)
    {
        if (!currentSelectedGameObject)
        {
            browserItemImage.enabled = false;
            browserItemDescription.enabled = false;
            equippedItemImage.enabled = false;
            equippedItemDescription.enabled = false;
            return;
        }
        else
        {
            browserItemImage.enabled = true;
            browserItemDescription.enabled = true;
        }
        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.LogError("Item Button is missing from the currently selected game object!", currentSelectedGameObject);
            return;
        }
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("The Item represented by the Item Button is null!", itemButton.gameObject);
            return;
        }
        var browserIcon = item.GetIcon();
        if (!browserIcon)
        {
            Debug.LogError(item.name + " does not have an icon.", item.gameObject);
            return;
        }
        var browserDescription = item.GetDescription();
        if (item.IsEquipped())
        {
            browserEquipmentStatus.enabled = true;
            var rune = item.GetComponent<Rune>();
            if (rune)
            {
                if (activeEquipment.GetRune(rune.GetRuneType()) != rune)
                {
                    var allEquipment = player.GetInventory().GetItems().Where(i => i.GetComponent<Equipment>()).Select(e => e.GetComponent<Equipment>());
                    var runeOwner = allEquipment.Where(e => e.GetRune(rune.GetRuneType()) == rune).FirstOrDefault();
                    if (!runeOwner)
                    {
                        Debug.LogError("Failed to find owner of Rune!", rune.gameObject);
                    }
                    browserEquipmentStatus.text = runeOwner.name + RuneSocketedElsewhereAffix + rune.name + RuneSocketedElsewhereSuffix;
                }
                else
                {
                    browserEquipmentStatus.text = rune.name + EquippedItemSuffix;
                }
            }
            else
            {
                browserEquipmentStatus.text = item.name + EquippedItemSuffix;
            }
        }
        else
        {
            browserEquipmentStatus.enabled = false;
        }
        if (string.IsNullOrEmpty(browserDescription))
        {
            Debug.LogError(item.name + " does not have a description!", item.gameObject);
            return;
        }
        browserItemImage.sprite = browserIcon;
        browserItemDescription.text = browserDescription;
        var equippedItem = player.GetInventory().GetItems(item.GetItemType()).Where(i => i.IsEquipped()).FirstOrDefault();
        if (equippedItem)
        {
            var rune = equippedItem.GetComponent<Rune>();
            if (rune)
            {
                equippedItem = activeEquipment.GetRune(rune.GetRuneType());
                if (!equippedItem)
                {
                    equippedItemImage.enabled = false;
                    equippedItemDescription.enabled = false;
                    return;
                }
            }
            equippedItemImage.enabled = true;
            equippedItemDescription.enabled = true;
            var equippedIcon = equippedItem.GetIcon();
            if (!equippedIcon)
            {
                Debug.LogError(equippedIcon.name + " does not have an icon!", item.gameObject);
                return;
            }
            var equippedDescription = equippedItem.GetDescription();
            if (string.IsNullOrEmpty(equippedDescription))
            {
                Debug.LogError(equippedIcon.name + " does not have a description!", item.gameObject);
                return;
            }
            equippedItemImage.sprite = equippedIcon;
            equippedItemDescription.text = equippedDescription;
        }
        else
        {
            equippedItemImage.enabled = false;
            equippedItemDescription.enabled = false;
            return;
        }
    }
}