using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomUnityLibrary;
using TMPro;
using System;

public class InventoryRunesView : MonoBehaviour
{
    [Tooltip("Rune tab prefabs to move between different types of runes")]
    [SerializeField]
    private RuneTab[] runeTabs;

    [Tooltip("Container to put item buttons for rune categories")]
    [SerializeField]
    private RectTransform categoryContainer;

    [Tooltip("Button prefab to show items")]
    [SerializeField]
    private ItemButton itemButton;

    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform runesContainer;

    [Tooltip("Description for when there are no runes available to display")]
    [SerializeField]
    private RectTransform noRunesDescription;

    [Tooltip("Text to display in no runes description, when applicable")]
    [SerializeField]
    private string noRunesDescriptionText;

    private Inventory inventory;
    private int tabIndex = 0;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (!inventory)
        {
            Debug.LogError(gameObject + " could not find Inventory!", gameObject);
            return;
        }
        if (string.IsNullOrEmpty(noRunesDescriptionText))
        {
            Debug.LogWarning("There is no description for when runes are missing.", gameObject);
        }
    }

    void Start()
    {
        SetUpNoDescriptionText();
    }

    /// <summary>
    /// Creates the rune buttons for the specified piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment whose runes can be changed</param>
    /// <returns>Whether or not any of that equipment type was found to create buttons for</returns>
    public bool CreateButtons(Equipment equipment)
    {
        foreach (var button in categoryContainer.GetComponentsInChildren<RuneTab>())
        {
            Destroy(button.gameObject);
        }
        foreach (var button in runesContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        var runeSocketTypes = equipment.GetRuneSocketTypes();
        if (runeSocketTypes.Length == 0)
        {
            Debug.LogError(equipment + " does not have any rune sockets!", equipment.gameObject);
            return false;
        }
        foreach (var runeSocketType in runeSocketTypes)
        {
            var runeTabPrefab = runeTabs.Where(t => t.GetRuneType() == runeSocketType).FirstOrDefault();
            if (!runeTabPrefab)
            {
                Debug.LogError("Missing a rune tab for " + runeSocketType + "!", gameObject);
                return false;
            }
            var tab = Instantiate(runeTabPrefab);
            tab.transform.SetParent(categoryContainer);
            var button = tab.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(tab + " could not find button component!", tab.gameObject);
                return false;
            }
            button.SetNavigation(Navigation.Mode.None);
        }
        MoveTab(0);
        return true;
    }

    /// <summary>
    /// Sets up the text game object for when there are no runes
    /// </summary>
    private void SetUpNoDescriptionText()
    {
        var description = noRunesDescription.GetComponent<TextMeshProUGUI>();
        if (!description)
        {
            Debug.LogError("Could not find TextMeshProUGUI!", gameObject);
            return;
        }
        description.text = noRunesDescriptionText;
    }

    /// <summary>
    /// Moves the rune tab by the index passed to the new tab
    /// </summary>
    /// <param name="movement">Amount to move the tab to the left or right</param>
    public void MoveTab(int movement)
    {
        tabIndex = (tabIndex + movement) % runeTabs.Length;
        if (tabIndex < 0)
        {
            tabIndex = 0;
        }
        var buttons = categoryContainer.GetComponentsInChildren<RuneTab>().Select(t => t.GetComponent<Button>()).ToArray();
        for (int i = 0; i < buttons.Count(); i++)
        {
            buttons[i].interactable = i == tabIndex;
        }
        var runes = inventory.GetItems(ItemType.Rune).Select(r => r.GetComponent<Rune>()).Where(r => r.GetRuneType() == runeTabs[tabIndex].GetRuneType());
        if (runes.Count() > 0)
        {
            noRunesDescription.gameObject.SetActive(false);
            foreach (var rune in runes)
            {
                var button = Instantiate(itemButton);
                button.transform.SetParent(runesContainer);
                button.SetItemType(ItemType.Rune);
                button.SetItem(rune);
                if (!EventSystem.current.currentSelectedGameObject || rune.IsEquipped())
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                }
            }
        }
        else
        {
            noRunesDescription.gameObject.SetActive(true);
        }
    }
}