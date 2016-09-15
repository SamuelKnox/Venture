using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIRuneShopController : MonoBehaviour
{
    [Tooltip("View used to display the title of the current inventory panel")]
    [SerializeField]
    private RuneShopTitleView titleView;

    [Tooltip("View used to display the instructions for the current inventory panel")]
    [SerializeField]
    private RuneShopInstructionsView instructionsView;

    [Tooltip("View used to display the runes to be selected from")]
    [SerializeField]
    private RuneShopRunesView runesView;

    [Tooltip("View used to display the long description of the currently chosen rune")]
    [SerializeField]
    private RuneShopRuneDescriptionView runeDescriptionView;

    [Tooltip("Number of runes for sale by shopkeeper")]
    [SerializeField]
    [Range(1, 5)]
    private int runesForSaleCount = 4;

    [Tooltip("List of Runes which can be sold")]
    [SerializeField]
    private List<Rune> runesForSale = new List<Rune>();
    
    private GameObject previousSelectedGameObject;

    void Start()
    {
        SetRunesForSale();
        runesView.CreateRuneButtons(runesForSale.ToArray());
        runesView.ResetShopkeeperDialog();
    }

    void Update()
    {
        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedGameObject != previousSelectedGameObject)
        {
            previousSelectedGameObject = currentSelectedGameObject;
            runesView.ResetShopkeeperDialog();
            var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
            if (!itemButton)
            {
                Debug.LogError("Could not find Item Button!", gameObject);
                return;
            }
            var item = itemButton.GetItem();
            if (!item)
            {
                Debug.LogError("Could not find item!", gameObject);
                return;
            }
            var rune = item.GetComponent<Rune>();
            runeDescriptionView.SetDescription(rune);
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            Time.timeScale = 1.0f;
            var runeShop = FindObjectOfType<RuneShop>();
            if (!runeShop)
            {
                Debug.LogError("Could not find Rune Shop!", gameObject);
                return;
            }
            runeShop.EndInteraction();
            SceneManager.UnloadScene(SceneNames.RuneShop);
        }
        if (Input.GetButtonDown(InputNames.BuyRune))
        {
            var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
            if (!itemButton)
            {
                Debug.LogError("Could not find Item Button!", gameObject);
                return;
            }
            var item = itemButton.GetItem();
            BuyRune(item);
        }
    }

    /// <summary>
    /// Selects the runes to be sold by the shop
    /// </summary>
    private void SetRunesForSale()
    {
        while (runesForSale.Count > runesForSaleCount)
        {
            runesForSale.Remove(runesForSale[Random.Range(0, runesForSale.Count)]);
        }
    }

    /// <summary>
    /// Purchases a rune if affordable
    /// </summary>
    /// <param name="item">Rune to purchase</param>
    private void BuyRune(Item item)
    {
        if (!item)
        {
            Debug.LogError("Item could not be found!", gameObject);
            return;
        }
        if (PlayerManager.Player.GetGold() < item.GetCost())
        {
            runesView.AttemptToPurchaseUnaffordableItem();
            return;
        }
        PlayerManager.Player.SpendGold(item.GetCost());
        var itemInstance = Instantiate(item);
        var collectable = itemInstance.GetComponent<Collectable>();
        if (!collectable)
        {
            Debug.LogError("Could not find Collectable on Rune!", gameObject);
            return;
        }
        PlayerManager.Player.Collect(collectable);
    }
}