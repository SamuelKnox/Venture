using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private const float HorizontalTabSpacing = 10.0f;

    [Tooltip("Panel which stores all of the category tabs")]
    [SerializeField]
    private RectTransform categoryTabPanel;

    [Tooltip("All of the item tabs which may be used")]
    [SerializeField]
    private ItemTab[] itemTabs;

    [Tooltip("All of the rune tabs which may be used")]
    [SerializeField]
    private RuneTab[] runeTabs;

    [Tooltip("Content panel which stores the items for the current category")]
    [SerializeField]
    private RectTransform itemsContentPanel;

    [Tooltip("Text used to show the description for the currently selected item")]
    [SerializeField]
    private Text itemDescription;

    [Tooltip("Button prefab used for item selection")]
    [SerializeField]
    private ItemButton itemButton;

    [Tooltip("Number of items to fit on the screen horizontally")]
    [SerializeField]
    [Range(1, 10)]
    private int horizontalItems = 5;

    [Tooltip("How much to space the item buttons apart")]
    [SerializeField]
    private Vector2 itemSpacing = Vector2.one;

    private int currentIndex = 0;
    private Player player;
    private GameObject currentSelectedGameObject;
    private InventoryMode inventoryMode;
    private List<Tab> tabs;
    private Item editableItem;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Player was not found!", gameObject);
        }
        if (itemTabs.Length == 0)
        {
            Debug.LogError("There must be at least one item tab!", gameObject);
        }
        if (runeTabs.Length == 0)
        {
            Debug.LogError("There must be at least one rune tab!", gameObject);
        }
    }

    void Start()
    {
        inventoryMode = InventoryMode.ItemBrowser;
        CreateTabs();
    }

    void Update()
    {
        if (Input.GetButtonDown(InputNames.TabLeft))
        {
            MoveTab(-1);
        }
        if (Input.GetButtonDown(InputNames.TabRight))
        {
            MoveTab(1);
        }
        if (EventSystem.current.currentSelectedGameObject != currentSelectedGameObject)
        {
            currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            UpdateDescription();
        }
        if (Input.GetButtonDown(InputNames.EditRunes))
        {
            switch (inventoryMode)
            {
                case InventoryMode.ItemBrowser:
                    inventoryMode = InventoryMode.RuneEditor;
                    CreateTabs();
                    break;
                case InventoryMode.RuneEditor:
                    inventoryMode = InventoryMode.ItemBrowser;
                    CreateTabs();
                    break;
                default:
                    Debug.LogError("An invalid InventoryMode (" + inventoryMode + ") is being used!", gameObject);
                    break;
            }
        }
        if (Input.GetButtonDown(InputNames.ItemMenu))
        {
            Time.timeScale = 1.0f;
            SceneManager.UnloadScene(SceneNames.Inventory);
        }
    }

    /// <summary>
    /// Creates the tabs for the respective inventory mode
    /// </summary>
    private void CreateTabs()
    {
        currentIndex = 0;
        foreach (Transform child in categoryTabPanel)
        {
            Destroy(child.gameObject);
        }
        switch (inventoryMode)
        {
            case InventoryMode.ItemBrowser:
                CreateItemTabs();
                break;
            case InventoryMode.RuneEditor:
                CreateRuneTabs();
                break;
            default:
                Debug.LogError("An invalid InventoryMode (" + inventoryMode.ToString() + ") was used!", gameObject);
                break;
        }
        MoveTab(0);
    }

    /// <summary>
    /// Creates the tabs for item browsing
    /// </summary>
    private void CreateItemTabs()
    {
        tabs = new List<Tab>();
        int skippedItems = 0;
        for (int i = 0; i < itemTabs.Length; i++)
        {
            int numItems = player.GetInventory().Count(itemTabs[i].GetItemType());
            if (numItems == 0)
            {
                skippedItems++;
                continue;
            }
            var itemTabInstance = Instantiate(itemTabs[i]);
            var itemTabTransform = itemTabInstance.GetComponent<RectTransform>();
            if (!itemTabTransform)
            {
                Debug.LogError(itemTabInstance.name + " is missing a RectTransform!", itemTabInstance.gameObject);
            }
            itemTabTransform.SetParent(categoryTabPanel);
            float xPosition = (i - skippedItems) * (itemTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
            float yPosition = 0.0f;
            itemTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            tabs.Add(itemTabInstance);
        }
    }

    /// <summary>
    /// Creates the tabs for rune browsing
    /// </summary>
    private void CreateRuneTabs()
    {
        tabs = new List<Tab>();
        var itemButton = EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.LogError("Item Button is missing from the currently selected game object!", EventSystem.current.currentSelectedGameObject);
        }
        editableItem = itemButton.GetItem();
        if (!editableItem)
        {
            Debug.LogError("The Item represented by the Item Button is null!", itemButton.gameObject);
        }
        var runeTypes = editableItem.GetRuneTypes();
        int skippedRunes = 0;
        for (int i = 0; i < runeTabs.Length; i++)
        {
            if (!runeTypes.Contains(runeTabs[i].GetRuneType()))
            {
                skippedRunes++;
                continue;
            }
            var runeTabInstance = Instantiate(runeTabs[i]);
            var runeTabTransform = runeTabInstance.GetComponent<RectTransform>();
            if (!runeTabTransform)
            {
                Debug.LogError(runeTabInstance.name + " is missing a RectTransform!", runeTabInstance.gameObject);
            }
            runeTabTransform.SetParent(categoryTabPanel);
            float xPosition = (i - skippedRunes) * (runeTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
            float yPosition = 0.0f;
            runeTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            tabs.Add(runeTabInstance);
        }
    }

    /// <summary>
    /// Moves the tab left or right by the specified number
    /// </summary>
    /// <param name="movement">Number of tabs to move</param>
    private void MoveTab(int movement)
    {
        tabs[currentIndex].GetImage().color = Color.white;
        currentIndex = (currentIndex + movement) % tabs.Count;
        if (currentIndex < 0)
        {
            currentIndex = tabs.Count - 1;
        }
        tabs[currentIndex].GetImage().color = Color.red;
        itemDescription.text = "";
        PopulateContentPanel();
    }

    /// <summary>
    /// Fills the content panel with item buttons based on the category
    /// </summary>
    private void PopulateContentPanel()
    {
        switch (inventoryMode)
        {
            case InventoryMode.ItemBrowser:
                var items = player.GetInventory().GetItems(itemTabs[currentIndex].GetItemType());
                PopulateItemBrowser(items, EquipItem);
                break;
            case InventoryMode.RuneEditor:
                var allRunes = player.GetInventory().GetItems(ItemType.Rune).Select(r => r.GetComponent<Rune>());
                var runes = allRunes.Where(r => r.GetRuneType() == runeTabs[currentIndex].GetRuneType()).ToArray();
                Debug.Log(runes.Length);
                PopulateItemBrowser(runes, AttachRune);
                break;
            default:
                Debug.LogError("An invalid InventoryMode (" + inventoryMode.ToString() + ") was used!", gameObject);
                break;
        }
    }

    /// <summary>
    /// Fills the content panel with the items of the currently selected category type
    /// </summary>
    private void PopulateItemBrowser(Item[] items, Action<Item> actionToTakeWithItems)
    {
        foreach (Transform child in itemsContentPanel)
        {
            Destroy(child.gameObject);
        }
        var buttonPrefabTransform = itemButton.GetComponent<RectTransform>();
        if (!buttonPrefabTransform)
        {
            Debug.LogError(itemButton.name + " needs a RectTransform, but does not have one!", gameObject);
        }
        var buttonSize = new Vector2(buttonPrefabTransform.sizeDelta.x, buttonPrefabTransform.sizeDelta.y);
        float contentWidth = horizontalItems * (buttonSize.x + itemSpacing.x) + itemSpacing.x;
        float contentHeight = (items.Length / horizontalItems + 1) * (buttonSize.y + itemSpacing.y) + itemSpacing.y;
        itemsContentPanel.sizeDelta = new Vector2(contentWidth, contentHeight);
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i])
            {
                Debug.LogError("There is a null Item in the inventory!", player.GetInventory().gameObject);
            }
            var itemButtonInstance = Instantiate(itemButton);
            var buttonTransform = itemButtonInstance.GetComponent<RectTransform>();
            if (!buttonTransform)
            {
                Debug.LogError(itemButtonInstance.name + " is missing a RectTransform!", itemButtonInstance.gameObject);
            }
            buttonTransform.SetParent(itemsContentPanel);
            float xPosition = (-itemsContentPanel.sizeDelta.x + buttonTransform.sizeDelta.x) / 2.0f + (i % horizontalItems) * (buttonTransform.sizeDelta.x + itemSpacing.x) + itemSpacing.x;
            float yPosition = (itemsContentPanel.sizeDelta.y - buttonTransform.sizeDelta.y) / 2.0f - (i / horizontalItems) * (buttonTransform.sizeDelta.y + itemSpacing.y) - itemSpacing.y;
            buttonTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            itemButtonInstance.SetItem(items[i]);
            var buttonImage = itemButtonInstance.GetComponentsInChildren<Image>().Where(s => s.gameObject != itemButtonInstance.gameObject).FirstOrDefault();
            var itemSprite = itemButtonInstance.GetItem().GetComponent<SpriteRenderer>();
            if (!itemSprite)
            {
                Debug.LogError(items[i] + " is missing a child Sprite Renderer!", items[i].gameObject);
            }
            buttonImage.sprite = itemSprite.sprite;
            bool activeItem = player.GetInventory().GetActiveItem(items[i].GetItemType()) == items[i];
            if (i == 0 || activeItem)
            {
                EventSystem.current.SetSelectedGameObject(itemButtonInstance.gameObject);
            }
            var button = itemButtonInstance.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(button.name = " is missing a Button component!", button.gameObject);
            }
            var item = itemButtonInstance.GetItem();
            button.onClick.AddListener(() => actionToTakeWithItems(item));
        }
    }

    /// <summary>
    /// Updates the description for the currently selected item
    /// </summary>
    private void UpdateDescription()
    {
        var itemButton = EventSystem.current.currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.LogError("Item Button is missing from the currently selected game object!", EventSystem.current.currentSelectedGameObject);
        }
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("The Item represented by the Item Button is null!", itemButton.gameObject);
        }
        var description = item.GetDescription();
        if (string.IsNullOrEmpty(description))
        {
            Debug.LogWarning(item.name + " does not have a description.", item.gameObject);
        }
        itemDescription.text = item.GetDescription();
    }

    /// <summary>
    /// Tells the player to equip the item
    /// </summary>
    /// <param name="item">Item to equip</param>
    private void EquipItem(Item item)
    {
        player.Equip(item);
    }

    /// <summary>
    /// Attaches a rune to the currently selected item
    /// </summary>
    /// <param name="item">Rune to attach</param>
    private void AttachRune(Item item)
    {
        var rune = item.GetComponent<Rune>();
        if (!rune)
        {
            Debug.LogError("Expecting a Rune, but received " + item + "!", item.gameObject);
        }
        editableItem.SetRune(rune);
    }

    /// <summary>
    /// Modes for inventory UI
    /// </summary>
    private enum InventoryMode
    {
        ItemBrowser,
        RuneEditor
    }
}