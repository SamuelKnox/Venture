using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIRoonShopController : MonoBehaviour
{
    [Tooltip("View used to display the title of the current inventory panel")]
    [SerializeField]
    private RoonShopTitleView titleView;

    [Tooltip("View used to display the instructions for the current inventory panel")]
    [SerializeField]
    private RoonShopInstructionsView instructionsView;

    [Tooltip("View used to display the roons to be selected from")]
    [SerializeField]
    private RoonShopRoonsView roonsView;

    [Tooltip("View used to display the long description of the currently chosen roon")]
    [SerializeField]
    private RoonShopRoonDescriptionView roonDescriptionView;

    [Tooltip("Number of roons for sale by shopkeeper")]
    [SerializeField]
    [Range(1, 5)]
    private int roonsForSaleCount = 4;

    [Tooltip("List of Roons which can be sold")]
    [SerializeField]
    private List<Roon> roonsForSale = new List<Roon>();
    
    private GameObject previousSelectedGameObject;

    void Start()
    {
        SetRoonsForSale();
        roonsView.CreateRoonButtons(roonsForSale.ToArray());
        roonsView.ResetShopkeeperDialog();
    }

    void Update()
    {
        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedGameObject != previousSelectedGameObject)
        {
            previousSelectedGameObject = currentSelectedGameObject;
            roonsView.ResetShopkeeperDialog();
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
            var roon = item.GetComponent<Roon>();
            roonDescriptionView.SetDescription(roon);
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            Time.timeScale = 1.0f;
            var roonShop = FindObjectOfType<RoonShop>();
            if (!roonShop)
            {
                Debug.LogError("Could not find Roon Shop!", gameObject);
                return;
            }
            roonShop.EndInteraction();
            SceneManager.UnloadScene(SceneNames.RoonShop);
        }
        if (Input.GetButtonDown(InputNames.BuyRoon))
        {
            var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
            if (!itemButton)
            {
                Debug.LogError("Could not find Item Button!", gameObject);
                return;
            }
            var item = itemButton.GetItem();
            BuyRoon(item);
        }
    }

    /// <summary>
    /// Selects the roons to be sold by the shop
    /// </summary>
    private void SetRoonsForSale()
    {
        while (roonsForSale.Count > roonsForSaleCount)
        {
            roonsForSale.Remove(roonsForSale[Random.Range(0, roonsForSale.Count)]);
        }
    }

    /// <summary>
    /// Purchases a roon if affordable
    /// </summary>
    /// <param name="item">Roon to purchase</param>
    private void BuyRoon(Item item)
    {
        if (!item)
        {
            Debug.LogError("Item could not be found!", gameObject);
            return;
        }
        if (PlayerManager.Player.GetGold() < item.GetCost())
        {
            roonsView.AttemptToPurchaseUnaffordableItem();
            return;
        }
        PlayerManager.Player.SpendGold(item.GetCost());
        var itemInstance = Instantiate(item);
        var collectable = itemInstance.GetComponent<Collectable>();
        if (!collectable)
        {
            Debug.LogError("Could not find Collectable on Roon!", gameObject);
            return;
        }
        PlayerManager.Player.Collect(collectable);
    }
}