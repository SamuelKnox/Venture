using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIInventoryController : MonoBehaviour
{
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
        ChangeMode(InventoryMode.Character, initialSelection);
    }

    void Update()
    {
        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.Log(currentSelectedGameObject + " could not find ItemButton!", currentSelectedGameObject);
            return;
        }
        bool changedSelectedGameObject = currentSelectedGameObject != previousSelectedGameObject;
        if (changedSelectedGameObject)
        {
            previousSelectedGameObject = currentSelectedGameObject;
            changedSelectedGameObject = true;
        }
        switch (inventoryMode)
        {
            case InventoryMode.Character:
                UpdateForCharacterMode(changedSelectedGameObject, itemButton);
                break;
            case InventoryMode.Equipment:
                UpdateForEquipmentMode(changedSelectedGameObject, itemButton);
                break;
            case InventoryMode.Runes:
                UpdateForRunesMode(changedSelectedGameObject, itemButton);
                break;
            default:
                Debug.LogError("A valid InventoryMode could not be found!", gameObject);
                return;
        }
    }

    private void UpdateForCharacterMode(bool changedSelectedGameObject, ItemButton itemButton)
    {
        var item = itemButton.GetItem();
        var equipment = item ? item.GetComponent<Equipment>() : null;
        if (changedSelectedGameObject)
        {
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

    private void UpdateForEquipmentMode(bool changedSelectedGameObject, ItemButton itemButton)
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
        if (changedSelectedGameObject)
        {
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

    private void UpdateForRunesMode(bool changedSelectedGameObject, ItemButton itemButton)
    {
        if (Input.GetButtonDown(InputNames.Inventory))
        {
            ChangeMode(InventoryMode.Character, itemButton);
        }
        if (Input.GetButtonDown(InputNames.TabRight))
        {
            runesView.MoveTab(1);
        }
        if (Input.GetButtonDown(InputNames.TabLeft)){
            runesView.MoveTab(-1);
        }
    }

    private void ChangeMode(InventoryMode inventoryMode, ItemButton itemButton)
    {
        this.inventoryMode = inventoryMode;
        var itemType = itemButton.GetItemType();
        var item = itemButton.GetItem();
        var equipment = item ? item.GetComponent<Equipment>() : null;
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
                bool runesButtonSuccess = runesView.CreateButtons(equipment);
                if (!runesButtonSuccess)
                {
                    ChangeMode(InventoryMode.Character, itemButton);
                }
                break;
            default:
                Debug.LogError("Invalid InventoryMode used!", gameObject);
                return;
        }
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