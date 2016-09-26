using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoonShopRoonsView : MonoBehaviour
{
    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform roonsContainer;

    [Tooltip("Image to display the shopkeeper")]
    [SerializeField]
    private Image shopkeeperImage;

    [Tooltip("Text to show what the shop keeper is saying")]
    [SerializeField]
    private TextMeshProUGUI shopkeeperDialog;

    [Tooltip("What the shopkeeper says when talked to")]
    [SerializeField]
    [TextArea(1, 5)]
    private string shopKeeperGenericDialog;

    [Tooltip("What the shopkeeper says when an item that cannot be afforded is attempted to be purchased")]
    [SerializeField]
    [TextArea(1, 5)]
    private string shopKeeperDialogForUnaffordableItem;


    [Tooltip("Item Button used to show the roons for sale")]
    [SerializeField]
    private ItemButton itemButton;

    /// <summary>
    /// Creates the buttons for the player to look through and purchase a roon
    /// </summary>
    /// <param name="roonsForSale">Roons to create buttons for</param>
    public void CreateRoonButtons(Roon[] roonsForSale)
    {
        foreach (var button in roonsContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        foreach (var roon in roonsForSale)
        {
            var itemButtonInstance = Instantiate(itemButton);
            itemButtonInstance.SetItemType(ItemType.Roon);
            itemButtonInstance.SetItem(roon);
            itemButtonInstance.transform.SetParent(roonsContainer);
            if (!EventSystem.current.currentSelectedGameObject)
            {
                EventSystem.current.SetSelectedGameObject(itemButtonInstance.gameObject);
            }
        }
    }

    /// <summary>
    /// A purchase attempt has failed, probably from not having enough gold
    /// </summary>
    public void AttemptToPurchaseUnaffordableItem()
    {
        shopkeeperDialog.text = shopKeeperDialogForUnaffordableItem;
    }

    /// <summary>
    /// Reset the shopkeeper's dialog to default
    /// </summary>
    public void ResetShopkeeperDialog()
    {
        shopkeeperDialog.text = shopKeeperGenericDialog;
    }
}