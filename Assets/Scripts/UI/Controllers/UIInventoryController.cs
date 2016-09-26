using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIInventoryController : MonoBehaviour
{
    private const string CharacterAButton = "Select Weapon";
    private const string CharacterBButton = "Exit";
    private const string CharacterXButton = "Change Roons";
    private const string CharacterYButton = "Remove Roons";
    private const string WeaponAButton = "Equip";
    private const string WeaponBButton = "Back";
    private const string WeaponXButton = null;
    private const string WeaponYButton = null;
    private const string RoonsAButton = "Attach";
    private const string RoonsBButton = "Back";
    private const string RoonsXButton = null;
    private const string RoonsYButton = null;

    [Tooltip("View used to display the title of the current inventory panel")]
    [SerializeField]
    private InventoryTitleView titleView;

    [Tooltip("View used to display the instructions for the current inventory panel")]
    [SerializeField]
    private InventoryInstructionsView instructionsView;

    [Tooltip("View used to display the UI for currently equipped weapon")]
    [SerializeField]
    private InventoryCharacterView characterView;

    [Tooltip("View used to display the weapon that can potentially be equipped")]
    [SerializeField]
    private InventoryWeaponView weaponView;

    [Tooltip("View used to display the long description of the currently chosen weapon")]
    [SerializeField]
    private InventoryWeaponDescriptionView weaponDescriptionView;

    [Tooltip("View used to display the roons to be selected from")]
    [SerializeField]
    private InventoryRoonsView roonsView;

    [Tooltip("View used to display the long description of the currently chosen roon")]
    [SerializeField]
    private InventoryRoonDescriptionView roonDescriptionView;

    [Tooltip("GameObject from the character panel which defaults as being selected")]
    [SerializeField]
    private ItemButton initialSelection;

    private InventoryMode inventoryMode = InventoryMode.Character;
    private GameObject previousSelectedGameObject;
    private ItemButton roonableWeaponItemButton;
    private bool dirty;
    private Dictionary<GamePadInputs, string> gamePadInstructions = new Dictionary<GamePadInputs, string>();

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
            EventSystem.current.SetSelectedGameObject(roonableWeaponItemButton.gameObject);
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
            case InventoryMode.Weapon:
                UpdateForWeaponMode(itemButton);
                break;
            case InventoryMode.Roons:
                UpdateForRoonsMode(itemButton);
                break;
            default:
                Debug.LogError("A valid InventoryMode could not be found!", gameObject);
                return;
        }
    }

    /// <summary>
    /// Update loop for character screen
    /// </summary>
    /// <param name="changedSelectedGameObject">If the selected character weapon has changed since last frame</param>
    /// <param name="itemButton">Currently selected character weapon</param>
    private void UpdateForCharacterMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        var weapon = item ? item.GetComponent<Weapon>() : null;
        if (dirty)
        {
            dirty = false;
            characterView.UpdateDescription(weapon);
            weaponDescriptionView.UpdateDescription(weapon);
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            Time.timeScale = 1.0f;
            var blacksmith = FindObjectOfType<Blacksmith>();
            if (!blacksmith)
            {
                Debug.LogError("Could not find Blacksmith!", gameObject);
                return;
            }
            blacksmith.EndInteraction();
            SceneManager.UnloadScene(SceneNames.Inventory);
        }
        if (Input.GetButtonDown(InputNames.EquipWeapon))
        {
            ChangeMode(InventoryMode.Weapon, itemButton);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.EditRoons))
        {
            ChangeMode(InventoryMode.Roons, itemButton);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.ClearRoons))
        {
            weapon.DetachAllRoons();
            dirty = true;
        }
    }

    /// <summary>
    /// Update loop for when selecting weapon
    /// </summary>
    /// <param name="changedSelectedGameObject">Whether or not the selected weapon has chanegd since last frame</param>
    /// <param name="itemButton">Currently selected item button</param>
    private void UpdateForWeaponMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting item, but there wasn't one!", gameObject);
            return;
        }
        var weapon = item.GetComponent<Weapon>();
        if (!weapon)
        {
            Debug.LogError("Expecting weapon, but there wasn't any!", gameObject);
            return;
        }
        if (dirty)
        {
            dirty = false;
            weaponView.UpdateDescription(weapon);
        }
        if (Input.GetButtonDown(InputNames.EquipWeapon))
        {
            var weaponOfType = PlayerManager.Player.GetInventory().GetItems(weapon.GetItemType()).Select(i => i.GetComponent<Weapon>());
            foreach (var weaponInstance in weaponOfType)
            {
                if (weaponInstance == weapon)
                {
                    continue;
                }
                weaponInstance.DetachAllRoons();
            }
            PlayerManager.Player.Equip(weapon);
            ChangeMode(InventoryMode.Character, itemButton);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            ChangeMode(InventoryMode.Character, itemButton);
            dirty = true;
        }
    }

    /// <summary>
    /// Update loop for when in roon selection
    /// </summary>
    /// <param name="changedSelectedGameObject">Whether or not the currently selected roon has been changed</param>
    /// <param name="itemButton">Currently selected item button</param>
    private void UpdateForRoonsMode(ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting item in ItemButton!", itemButton.gameObject);
            return;
        }
        var roon = item.GetComponent<Roon>();
        Weapon weaponWithRoonAttached = null;
        if (roon)
        {
            var weapon = PlayerManager.Player.GetInventory().GetItems().Where(i => i.GetComponent<Weapon>()).Select(e => e.GetComponent<Weapon>());
            var socketedWeapon = weapon.Where(e => e.GetRoonSocketTypes().Contains(roon.GetRoonType()));
            weaponWithRoonAttached = socketedWeapon.Where(e => e.GetRoon(roon.GetRoonType()) == roon).FirstOrDefault();
        }
        if (dirty)
        {
            dirty = false;
            roonsView.UpdateDescription(roon);
            roonDescriptionView.SetDescription(roon, weaponWithRoonAttached);
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            var weaponItemButton = characterView.GetItemButton(roonableWeaponItemButton.GetItemType());
            ChangeMode(InventoryMode.Character, weaponItemButton);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.TabRight))
        {
            roonsView.MoveTab(1);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.TabLeft))
        {
            roonsView.MoveTab(-1);
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.EquipRoon))
        {
            if (roon && !roon.IsEquipped())
            {
                var weaponItem = roonableWeaponItemButton.GetItem();
                if (!weaponItem)
                {
                    Debug.LogError("Could not find item!", gameObject);
                    return;
                }
                var weapon = weaponItem.GetComponent<Weapon>();
                if (!weapon)
                {
                    Debug.LogError("Could not find weapon!", gameObject);
                    return;
                }
                weapon.SetRoon(roon);
            }
            dirty = true;
        }
        if (Input.GetButtonDown(InputNames.ClearRoons) && weaponWithRoonAttached)
        {
            weaponWithRoonAttached.DetachRoon(roon);
            dirty = true;
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
        var weapon = item ? item.GetComponent<Weapon>() : null;
        roonableWeaponItemButton = null;
        switch (this.inventoryMode)
        {
            case InventoryMode.Character:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(true);
                weaponView.gameObject.SetActive(false);
                weaponDescriptionView.gameObject.SetActive(true);
                roonsView.gameObject.SetActive(false);
                roonDescriptionView.gameObject.SetActive(false);
                characterView.UpdateWeapon(itemType);
                characterView.EnableNavigation(true);
                break;
            case InventoryMode.Weapon:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(true);
                weaponView.gameObject.SetActive(true);
                weaponDescriptionView.gameObject.SetActive(false);
                roonsView.gameObject.SetActive(false);
                roonDescriptionView.gameObject.SetActive(false);
                characterView.EnableNavigation(false);
                bool weaponButtonSuccess = weaponView.CreateButtons(itemType);
                if (!weaponButtonSuccess)
                {
                    ChangeMode(InventoryMode.Character, itemButton);
                }
                break;
            case InventoryMode.Roons:
                titleView.gameObject.SetActive(true);
                instructionsView.gameObject.SetActive(true);
                characterView.gameObject.SetActive(false);
                weaponView.gameObject.SetActive(false);
                weaponDescriptionView.gameObject.SetActive(false);
                roonsView.gameObject.SetActive(true);
                roonDescriptionView.gameObject.SetActive(true);
                roonableWeaponItemButton = itemButton;
                roonsView.CreateButtons(weapon);
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
                gamePadInstructions[GamePadInputs.A] = CharacterAButton;
                gamePadInstructions[GamePadInputs.B] = CharacterBButton;
                gamePadInstructions[GamePadInputs.X] = CharacterXButton;
                gamePadInstructions[GamePadInputs.Y] = CharacterYButton;
                break;
            case InventoryMode.Weapon:
                gamePadInstructions[GamePadInputs.A] = WeaponAButton;
                gamePadInstructions[GamePadInputs.B] = WeaponBButton;
                gamePadInstructions[GamePadInputs.X] = WeaponXButton;
                gamePadInstructions[GamePadInputs.Y] = WeaponYButton;
                break;
            case InventoryMode.Roons:
                gamePadInstructions[GamePadInputs.A] = RoonsAButton;
                gamePadInstructions[GamePadInputs.B] = RoonsBButton;
                gamePadInstructions[GamePadInputs.X] = RoonsXButton;
                gamePadInstructions[GamePadInputs.Y] = RoonsYButton;
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
        Weapon,
        Roons
    }
}