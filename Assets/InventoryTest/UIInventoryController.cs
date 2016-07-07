using CustomUnityLibrary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIInventoryController : MonoBehaviour
{
    private const string characterAButton = "Select Equipment";
    private const string characterBButton = "Exit";
    private const string characterXButton = "Change Runes";
    private const string characterYButton = "Remove Runes";
    private const string equipmentAButton = "Equip";
    private const string equipmentBButton = "Back";
    private const string equipmentXButton = null;
    private const string equipmentYButton = null;
    private const string runesAButton = "Attach";
    private const string runesBButton = "Back";
    private const string runesXButton = null;
    private const string runesYButton = null;

    [Tooltip("View used to display the title of the current inventory panel")]
    [SerializeField]
    private InventoryTitleView titleView;

    [Tooltip("View used to display the instructions for the current inventory panel")]
    [SerializeField]
    private InventoryInstructionsView instructionsView;

    [Tooltip("View used to display the UI for currently equipped equipment")]
    [SerializeField]
    private InventoryCharacterView characterView;

    [Tooltip("View used to display the equipment that can potentially be equipped")]
    [SerializeField]
    private InventoryEquipmentView equipmentView;

    [Tooltip("View used to display the long description of the currently chosen equipment")]
    [SerializeField]
    private InventoryEquipmentDescriptionView equipmentDescriptionView;

    [Tooltip("View used to display the runes to be selected from")]
    [SerializeField]
    private InventoryRunesView runesView;

    [Tooltip("View used to display the long description of the currently chosen rune")]
    [SerializeField]
    private InventoryRuneDescriptionView runeDescriptionView;

    [Tooltip("GameObject from the character panel which defaults as being selected")]
    [SerializeField]
    private ItemButton initialSelection;

    private InventoryMode inventoryMode = InventoryMode.Character;
    private GameObject previousSelectedGameObject;
    private Player player;
    private ItemButton runeableEquipmentItemButton;
    private bool dirty;
    private Dictionary<GamePadInputs, string> gamePadInstructions = new Dictionary<GamePadInputs, string>();

    void Awake()
    {
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.Log(gameObject + " could not find Player!", gameObject);
            return;
        }
    }

    void Start()
    {
        UpdateInstructions();
        ChangeMode(InventoryMode.Character, initialSelection);
    }

    void Update()
    {
        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (!currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(runeableEquipmentItemButton.gameObject);
            currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }
        dirty = dirty || currentSelectedGameObject != previousSelectedGameObject;
        previousSelectedGameObject = currentSelectedGameObject;
        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.Log(currentSelectedGameObject + " could not find ItemButton!", currentSelectedGameObject);
            return;
        }
        switch (inventoryMode)
        {
            case InventoryMode.Character:
                UpdateForCharacterMode(itemButton);
                break;
            case InventoryMode.Equipment:
                UpdateForEquipmentMode(itemButton);
                break;
            case InventoryMode.Runes:
                UpdateForRunesMode(itemButton);
                break;
            default:
                Debug.LogError("A valid InventoryMode could not be found!", gameObject);
                return;
        }
    }

    /// <summary>
    /// Update loop for character screen
    /// </summary>
    /// <param name="changedSelectedGameObject">If the selected character equipment has changed since last frame</param>
    /// <param name="itemButton">Currently selected character equipment</param>
    private void UpdateForCharacterMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        var equipment = item ? item.GetComponent<Equipment>() : null;
        if (dirty)
        {
            dirty = false;
            characterView.UpdateDescription(equipment);
            equipmentDescriptionView.UpdateDescription(equipment);
        }
        if (Input.GetButtonDown(InputNames.Inventory))
        {
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
        if (Input.GetButtonDown(InputNames.EquipEquipment))
        {
            ChangeMode(InventoryMode.Equipment, itemButton);
        }
        if (Input.GetButtonDown(InputNames.EditRunes))
        {
            ChangeMode(InventoryMode.Runes, itemButton);
        }
        if (Input.GetButtonDown(InputNames.ClearRunes))
        {
            equipment.DetachAllRunes();
        }
    }

    /// <summary>
    /// Update loop for when selecting equipment
    /// </summary>
    /// <param name="changedSelectedGameObject">Whether or not the selected equipment has chanegd since last frame</param>
    /// <param name="itemButton">Currently selected item button</param>
    private void UpdateForEquipmentMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting item, but there wasn't one!", gameObject);
            return;
        }
        var equipment = item.GetComponent<Equipment>();
        if (!equipment)
        {
            Debug.LogError("Expecting equipment, but there wasn't any!", gameObject);
            return;
        }
        if (dirty)
        {
            dirty = false;
            equipmentView.UpdateDescription(equipment);
        }
        if (Input.GetButtonDown(InputNames.EquipEquipment))
        {
            var equipmentOfType = player.GetInventory().GetItems(equipment.GetItemType()).Select(i => i.GetComponent<Equipment>());
            foreach (var equipmentPiece in equipmentOfType)
            {
                if (equipmentPiece == equipment)
                {
                    continue;
                }
                equipmentPiece.DetachAllRunes();
            }
            player.Equip(equipment);
            ChangeMode(InventoryMode.Character, itemButton);
        }
        if (Input.GetButtonDown(InputNames.Inventory))
        {
            ChangeMode(InventoryMode.Character, itemButton);
        }
    }

    /// <summary>
    /// Update loop for when in rune selection
    /// </summary>
    /// <param name="changedSelectedGameObject">Whether or not the currently selected rune has been changed</param>
    /// <param name="itemButton">Currently selected item button</param>
    private void UpdateForRunesMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting item in ItemButton!", itemButton.gameObject);
            return;
        }
        var rune = item.GetComponent<Rune>();
        if (dirty)
        {
            dirty = false;
            runesView.UpdateDescription(rune);
            runeDescriptionView.SetDescription(rune);
        }
        if (Input.GetButtonDown(InputNames.Inventory))
        {
            var equipmentItemButton = characterView.GetItemButton(runeableEquipmentItemButton.GetItemType());
            ChangeMode(InventoryMode.Character, equipmentItemButton);
        }
        if (Input.GetButtonDown(InputNames.TabRight))
        {
            runesView.MoveTab(1);
        }
        if (Input.GetButtonDown(InputNames.TabLeft))
        {
            runesView.MoveTab(-1);
        }
        if (Input.GetButtonDown(InputNames.EquipRune))
        {
            if (rune && !rune.IsEquipped())
            {
                var equipmentItem = runeableEquipmentItemButton.GetItem();
                if (!equipmentItem)
                {
                    Debug.LogError("Could not find equipment!", gameObject);
                    return;
                }
                var equipment = equipmentItem.GetComponent<Equipment>();
                if (!equipment)
                {
                    Debug.LogError("Could not find equipment!", gameObject);
                    return;
                }
                equipment.SetRune(rune);
            }
        }
    }

    /// <summary>
    /// Changes the inventory mode
    /// </summary>
    /// <param name="inventoryMode">New inventory mode</param>
    /// <param name="itemButton">Item button selected when inventory mode is changing</param>
    private void ChangeMode(InventoryMode inventoryMode, ItemButton itemButton)
    {
        dirty = true;
        this.inventoryMode = inventoryMode;
        UpdateInstructions();
        var itemType = itemButton.GetItemType();
        var item = itemButton.GetItem();
        var equipment = item ? item.GetComponent<Equipment>() : null;
        runeableEquipmentItemButton = null;
        switch (this.inventoryMode)
        {
            case InventoryMode.Character:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(true);
                equipmentView.gameObject.SetActive(false);
                equipmentDescriptionView.gameObject.SetActive(true);
                runesView.gameObject.SetActive(false);
                runeDescriptionView.gameObject.SetActive(false);
                characterView.UpdateEquipment(itemType);
                characterView.EnableNavigation(true);
                break;
            case InventoryMode.Equipment:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(true);
                equipmentView.gameObject.SetActive(true);
                equipmentDescriptionView.gameObject.SetActive(false);
                runesView.gameObject.SetActive(false);
                runeDescriptionView.gameObject.SetActive(false);
                characterView.EnableNavigation(false);
                bool equipmentButtonSuccess = equipmentView.CreateButtons(itemType);
                if (!equipmentButtonSuccess)
                {
                    ChangeMode(InventoryMode.Character, itemButton);
                }
                break;
            case InventoryMode.Runes:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(false);
                equipmentView.gameObject.SetActive(false);
                equipmentDescriptionView.gameObject.SetActive(false);
                runesView.gameObject.SetActive(true);
                runeDescriptionView.gameObject.SetActive(true);
                runeableEquipmentItemButton = itemButton;
                runesView.CreateButtons(equipment);
                break;
            default:
                Debug.LogError("Invalid InventoryMode used!", gameObject);
                return;
        }
    }

    /// <summary>
    /// Adds all the keys for the gamepad input instruction dictionary, and updates the display on screen
    /// </summary>
    private void UpdateInstructions()
    {
        switch (inventoryMode)
        {
            case InventoryMode.Character:
                gamePadInstructions[GamePadInputs.A] = characterAButton;
                gamePadInstructions[GamePadInputs.B] = characterBButton;
                gamePadInstructions[GamePadInputs.X] = characterXButton;
                gamePadInstructions[GamePadInputs.Y] = characterYButton;
                break;
            case InventoryMode.Equipment:
                gamePadInstructions[GamePadInputs.A] = equipmentAButton;
                gamePadInstructions[GamePadInputs.B] = equipmentBButton;
                gamePadInstructions[GamePadInputs.X] = equipmentXButton;
                gamePadInstructions[GamePadInputs.Y] = equipmentYButton;
                break;
            case InventoryMode.Runes:
                gamePadInstructions[GamePadInputs.A] = runesAButton;
                gamePadInstructions[GamePadInputs.B] = runesBButton;
                gamePadInstructions[GamePadInputs.X] = runesXButton;
                gamePadInstructions[GamePadInputs.Y] = runesYButton;
                break;
            default:
                Debug.LogError("Invalid InventoryMode supplied!", gameObject);
                return;
        }
        instructionsView.UpdateInstructions(gamePadInstructions);
    }

    /// <summary>
    /// Sections of inventory event system can be in
    /// </summary>
    private enum InventoryMode
    {
        Character,
        Equipment,
        Runes
    }
}