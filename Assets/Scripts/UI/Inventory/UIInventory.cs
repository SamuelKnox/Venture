using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private const float LongPressDuration = 1.0f;
    private const float HorizontalTabSpacing = 10.0f;

    [Tooltip("Panel which stores all of the category tabs")]
    [SerializeField]
    private RectTransform categoryTabPanel;

    [Tooltip("All of the item tabs which may be used")]
    [SerializeField]
    private EquipmentTab[] equipmentTabs;

    [Tooltip("All of the rune tabs which may be used")]
    [SerializeField]
    private RuneTab[] runeTabs;

    [Tooltip("Content panel which stores the items for the current category")]
    [SerializeField]
    private RectTransform itemsContentPanel;

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
    private Equipment activeEquipmentForRunes;
    private float longPressTimer;
    private InventoryButtonsView buttonsView;
    private InventoryDescriptionView descriptionView;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Player was not found!", gameObject);
            return;
        }
        if (equipmentTabs.Length == 0)
        {
            Debug.LogError("There must be at least one item tab!", gameObject);
            return;
        }
        if (runeTabs.Length == 0)
        {
            Debug.LogError("There must be at least one rune tab!", gameObject);
            return;
        }
        buttonsView = FindObjectOfType<InventoryButtonsView>();
        if (!buttonsView)
        {
            Debug.LogError(gameObject + " could not find InventoryButtonsView!", gameObject);
            return;
        }
        descriptionView = FindObjectOfType<InventoryDescriptionView>();
        if (!descriptionView)
        {
            Debug.LogError(gameObject + " could not find InventoryDescriptionView!", gameObject);
            return;
        }
    }

    void Start()
    {
        inventoryMode = InventoryMode.EquipmentBrowser;
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
            descriptionView.UpdateItemDescriptions(currentSelectedGameObject, activeEquipmentForRunes);
        }
        switch (inventoryMode)
        {
            case InventoryMode.EquipmentBrowser:
                if (Input.GetButtonDown(InputNames.Inventory))
                {
                    buttonsView.PressButton(InputNames.Inventory);
                    Time.timeScale = 1.0f;
                    var playerController = player.GetComponent<PlayerController>();
                    if (!playerController)
                    {
                        Debug.LogError("Could not find PlayerController on Player!", gameObject);
                        return;
                    }
                    playerController.enabled = true;
                    SceneManager.UnloadScene(SceneNames.Inventory);
                }
                if (Input.GetButtonDown(InputNames.EditRunes))
                {
                    buttonsView.PressButton(InputNames.EditRunes);
                    if (currentSelectedGameObject)
                    {
                        inventoryMode = InventoryMode.RuneEditor;
                        CreateTabs();
                    }
                }
                if (!currentSelectedGameObject)
                {
                    return;
                }
                var equipmentItemButton = currentSelectedGameObject.GetComponent<ItemButton>();
                if (!equipmentItemButton)
                {
                    return;
                }
                var equipmentItem = equipmentItemButton.GetItem();
                if (!equipmentItem)
                {
                    Debug.LogError(equipmentItemButton + " does not have an item!", equipmentItemButton.gameObject);
                    return;
                }
                var equipment = equipmentItem.GetComponent<Equipment>();
                if (!equipment)
                {
                    Debug.LogError("Expecting equipment, but received " + equipmentItem + "!", equipmentItem.gameObject);
                    return;
                }
                if (Input.GetButtonDown(InputNames.EquipEquipment))
                {
                    buttonsView.PressButton(InputNames.EquipEquipment);
                    Equip(equipment);
                }
                if (Input.GetButtonDown(InputNames.ClearRunes))
                {
                    buttonsView.PressButton(InputNames.ClearRunes);
                    ClearRunes(equipment);
                }
                if (Input.GetButtonDown(InputNames.ClearRunes))
                {
                    longPressTimer = 0.0f;
                }
                if (Input.GetButton(InputNames.ClearRunes))
                {
                    longPressTimer += Time.unscaledDeltaTime;
                    if (longPressTimer > 0.0f)
                    {
                        buttonsView.HoldButton(InputNames.ClearRunes, longPressTimer / LongPressDuration);
                    }
                    if (longPressTimer >= LongPressDuration)
                    {
                        ClearAllRunes(equipment);
                        longPressTimer = -Mathf.Infinity;
                        buttonsView.HoldButton(InputNames.ClearRunes, 1.0f);
                    }
                }
                if (Input.GetButtonUp(InputNames.ClearRunes))
                {
                    buttonsView.HoldButton(InputNames.ClearRunes, 1.0f);
                }
                break;
            case InventoryMode.RuneEditor:
                if (Input.GetButtonDown(InputNames.Inventory))
                {
                    buttonsView.PressButton(InputNames.Inventory);
                    inventoryMode = InventoryMode.EquipmentBrowser;
                    CreateTabs();
                }
                if (!currentSelectedGameObject)
                {
                    return;
                }
                var runeItemButton = currentSelectedGameObject.GetComponent<ItemButton>();
                if (!runeItemButton)
                {
                    return;
                }
                var runeItem = runeItemButton.GetItem();
                if (!runeItem)
                {
                    Debug.LogError(runeItemButton + " does not have an item!", runeItemButton.gameObject);
                    return;
                }
                var rune = runeItem.GetComponent<Rune>();
                if (!rune)
                {
                    Debug.LogError("Expecting rune, but received " + runeItem + "!", runeItem.gameObject);
                    return;
                }
                if (Input.GetButtonDown(InputNames.EquipRune))
                {
                    buttonsView.PressButton(InputNames.EquipRune);
                    AttachRune(rune);
                }
                if (Input.GetButtonDown(InputNames.ClearRunes))
                {
                    buttonsView.PressButton(InputNames.ClearRunes);
                    DetachRune(rune);
                }
                if (Input.GetButtonDown(InputNames.LevelUpRune))
                {
                    longPressTimer = 0.0f;
                }
                if (Input.GetButton(InputNames.LevelUpRune))
                {
                    longPressTimer += Time.unscaledDeltaTime;
                    if (longPressTimer > 0.0f)
                    {
                        buttonsView.HoldButton(InputNames.LevelUpRune, longPressTimer / LongPressDuration);
                    }
                    if (longPressTimer >= LongPressDuration)
                    {
                        LevelUpRune(rune);
                        longPressTimer = -Mathf.Infinity;
                        buttonsView.HoldButton(InputNames.LevelUpRune, 1.0f);
                    }
                }
                if (Input.GetButtonUp(InputNames.LevelUpRune))
                {
                    buttonsView.HoldButton(InputNames.LevelUpRune, 1.0f);
                }
                break;
            default:
                Debug.LogError("Invalid inventory mode!", gameObject);
                return;
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
            case InventoryMode.EquipmentBrowser:
                CreateEquipmentTabs();
                break;
            case InventoryMode.RuneEditor:
                bool success = CreateRuneTabs();
                if (!success)
                {
                    inventoryMode = InventoryMode.EquipmentBrowser;
                    CreateTabs();
                }
                break;
            default:
                Debug.LogError("An invalid InventoryMode (" + inventoryMode.ToString() + ") was used!", gameObject);
                return;
        }
        MoveTab(0);
    }

    /// <summary>
    /// Creates the tabs for equipment browsing
    /// </summary>
    private void CreateEquipmentTabs()
    {
        tabs = new List<Tab>();
        for (int i = 0; i < equipmentTabs.Length; i++)
        {
            var equipmentTabInstance = Instantiate(equipmentTabs[i]);
            var equipmentTabTransform = equipmentTabInstance.GetComponent<RectTransform>();
            if (!equipmentTabTransform)
            {
                Debug.LogError(equipmentTabInstance.name + " is missing a RectTransform!", equipmentTabInstance.gameObject);
                return;
            }
            equipmentTabTransform.SetParent(categoryTabPanel);
            float xPosition = i * (equipmentTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
            float yPosition = 0.0f;
            equipmentTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            tabs.Add(equipmentTabInstance);
        }
    }

    /// <summary>
    /// Creates the tabs for rune browsing
    /// </summary>
    /// <returns>Whether or not the tabs were successfully created</returns>
    private bool CreateRuneTabs()
    {
        if (!currentSelectedGameObject)
        {
            return false;
        }
        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
        tabs = new List<Tab>();
        if (!itemButton)
        {
            Debug.LogError("Item Button is missing from the currently selected game object!", currentSelectedGameObject);
            return false;
        }
        var activeItem = itemButton.GetItem();
        if (!activeItem)
        {
            Debug.LogError("The Item represented by the Item Button is null!", itemButton.gameObject);
            return false;
        }
        activeEquipmentForRunes = activeItem.GetComponent<Equipment>();
        if (!activeEquipmentForRunes)
        {
            Debug.LogError(activeEquipmentForRunes + " is not Equipment, and it needs to be to access Runes!", activeEquipmentForRunes.gameObject);
            return false;
        }
        var runeTypes = activeEquipmentForRunes.GetRuneTypes();
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
                return false;
            }
            runeTabTransform.SetParent(categoryTabPanel);
            float xPosition = (i - skippedRunes) * (runeTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
            float yPosition = 0.0f;
            runeTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
            tabs.Add(runeTabInstance);
        }
        return true;
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
        EventSystem.current.SetSelectedGameObject(null);
        currentSelectedGameObject = null;
        PopulateContentPanel();
        descriptionView.UpdateItemDescriptions(currentSelectedGameObject, activeEquipmentForRunes);
    }

    /// <summary>
    /// Fills the content panel with item buttons based on the category
    /// </summary>
    private void PopulateContentPanel()
    {
        switch (inventoryMode)
        {
            case InventoryMode.EquipmentBrowser:
                var equipment = player.GetInventory().GetItems(equipmentTabs[currentIndex].GetItemType());
                PopulateItemBrowser(equipment);
                break;
            case InventoryMode.RuneEditor:
                var allRunes = player.GetInventory().GetItems(ItemType.Rune).Select(r => r.GetComponent<Rune>());
                var runes = allRunes.Where(r => r.GetRuneType() == runeTabs[currentIndex].GetRuneType()).ToArray();
                PopulateItemBrowser(runes);
                break;
            default:
                Debug.LogError("An invalid InventoryMode (" + inventoryMode.ToString() + ") was used!", gameObject);
                return;
        }
    }

    /// <summary>
    /// Fills the content panel with the items of the currently selected category type
    /// </summary>
    private void PopulateItemBrowser(Item[] items)
    {
        foreach (Transform child in itemsContentPanel)
        {
            Destroy(child.gameObject);
        }
        var buttonPrefabTransform = itemButton.GetComponent<RectTransform>();
        if (!buttonPrefabTransform)
        {
            Debug.LogError(itemButton.name + " needs a RectTransform, but does not have one!", gameObject);
            return;
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
                return;
            }
            var itemButtonInstance = Instantiate(itemButton);
            var buttonTransform = itemButtonInstance.GetComponent<RectTransform>();
            if (!buttonTransform)
            {
                Debug.LogError(itemButtonInstance.name + " is missing a RectTransform!", itemButtonInstance.gameObject);
                return;
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
                return;
            }
            buttonImage.sprite = itemSprite.sprite;
            var button = itemButtonInstance.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(button.name = " is missing a Button component!", button.gameObject);
                return;
            }
            if (button.IsInteractable())
            {
                if (items[i].IsEquipped())
                {
                    button.image.color = Color.gray;
                }
                if (i == 0)
                {
                    EventSystem.current.SetSelectedGameObject(itemButtonInstance.gameObject);
                }
                var rune = items[i].GetComponent<Rune>();
                if (rune)
                {
                    if (activeEquipmentForRunes.GetRune(rune.GetRuneType()) == rune)
                    {
                        EventSystem.current.SetSelectedGameObject(itemButtonInstance.gameObject);
                    }
                }
                else if (items[i].IsEquipped())
                {
                    EventSystem.current.SetSelectedGameObject(itemButtonInstance.gameObject);
                }
            }
            var item = itemButtonInstance.GetItem();
        }
    }

    /// <summary>
    /// Levels up the rune
    /// </summary>
    /// <param name="rune">Rune to level up</param>
    private void LevelUpRune(Rune rune)
    {
        rune.SetLevel(rune.GetLevel() + 1);
    }

    /// <summary>
    /// Tells the player to equip the item
    /// </summary>
    /// <param name="item">Item to equip</param>
    private void Equip(Item item)
    {
        if (!item)
        {
            Debug.LogError("Cannot equip a null item!", gameObject);
            return;
        }
        if (item.IsEquipped())
        {
            return;
        }
        var equipment = item.GetComponent<Equipment>();
        if (!equipment)
        {
            Debug.LogError(item + " is not Equipment!", item.gameObject);
            return;
        }
        player.Equip(equipment);
        descriptionView.UpdateItemDescriptions(currentSelectedGameObject, activeEquipmentForRunes);
        PopulateContentPanel();
    }

    /// <summary>
    /// Attaches a rune to the currently selected equipment
    /// </summary>
    /// <param name="rune">Rune to attach</param>
    private void AttachRune(Rune rune)
    {
        if (!rune)
        {
            Debug.LogError("Cannot attach a null rune!", gameObject);
            return;
        }
        if (rune.IsEquipped())
        {
            return;
        }
        var equipment = activeEquipmentForRunes.GetComponent<Equipment>();
        if (!equipment)
        {
            Debug.LogError(activeEquipmentForRunes + " must be an Equipment to have Runes!", activeEquipmentForRunes.gameObject);
            return;
        }
        equipment.SetRune(rune);
        PopulateContentPanel();
    }

    /// <summary>
    /// Detaches a rune from the currently selected equipment
    /// </summary>
    /// <param name="rune">Rune to detach</param>
    private void DetachRune(Rune rune)
    {
        var equipment = activeEquipmentForRunes.GetComponent<Equipment>();
        if (!equipment)
        {
            Debug.LogError(activeEquipmentForRunes + " must be an Equipment to have Runes!", activeEquipmentForRunes.gameObject);
            return;
        }
        equipment.DetachRune(rune);
        PopulateContentPanel();
    }

    /// <summary>
    /// Detaches all runes from specified equipment
    /// </summary>
    /// <param name="equipment">Equipment to detach runes from</param>
    private void ClearRunes(Equipment equipment)
    {
        equipment.DetachAllRunes();
        PopulateContentPanel();
    }

    /// <summary>
    /// Detaches all runes from equipment of the specified type
    /// </summary>
    /// <param name="equipment">Piece of equipment of the type that all runes will be detached from</param>
    private void ClearAllRunes(Equipment equipment)
    {
        var allEquipment = player.GetInventory().GetItems(equipment.GetItemType()).Select(e => e.GetComponent<Equipment>());
        foreach (var equipmentPiece in allEquipment)
        {
            equipmentPiece.DetachAllRunes();
        }
        PopulateContentPanel();
    }

    /// <summary>
    /// Modes for inventory UI
    /// </summary>
    private enum InventoryMode
    {
        EquipmentBrowser,
        RuneEditor
    }
}