using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomUnityLibrary;
using TMPro;

public class InventoryRunesView : MonoBehaviour
{
    private const string EquippedDescriptionPrefix = "Equipped: ";
    private const string NoEquippedDescriptionSuffix = "None";

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

    [Tooltip("Short description for currently selected rune")]
    [SerializeField]
    private TextMeshProUGUI description;

    private Inventory inventory;
    private int tabIndex = 0;
    private bool dirty = false;

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
        UpdateDescription(null);
    }

    void Update()
    {
        if (dirty)
        {
            dirty = false;
            MoveTab(0);
        }
    }

    /// <summary>
    /// Creates the rune buttons for the specified piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment whose runes can be changed</param>
    /// <returns>Whether or not any of that equipment type was found to create buttons for</returns>
    public void CreateButtons(Equipment equipment)
    {
        foreach (var runeTab in categoryContainer.GetComponentsInChildren<RuneTab>())
        {
            Destroy(runeTab.gameObject);
        }
        foreach (var button in runesContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        var runeSocketTypes = equipment.GetRuneSocketTypes();
        if (runeSocketTypes.Length == 0)
        {
            Debug.LogError(equipment + " does not have any rune sockets!", equipment.gameObject);
            return;
        }
        foreach (var runeSocketType in runeSocketTypes)
        {
            var runeTabPrefab = runeTabs.Where(t => t.GetRuneType() == runeSocketType).FirstOrDefault();
            if (!runeTabPrefab)
            {
                Debug.LogError("Missing a rune tab for " + runeSocketType + "!", gameObject);
                return;
            }
            var tab = Instantiate(runeTabPrefab);
            tab.transform.SetParent(categoryContainer);
            var button = tab.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(tab + " could not find button component!", tab.gameObject);
                return;
            }
            button.SetNavigation(Navigation.Mode.None);
        }
        dirty = true;
    }

    /// <summary>
    /// Moves the rune tab by the index passed to the new tab
    /// </summary>
    /// <param name="movement">Amount to move the tab to the left or right</param>
    public void MoveTab(int movement)
    {
        foreach (var itemButton in runesContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(itemButton.gameObject);
        }
        var runeTabInstances = categoryContainer.GetComponentsInChildren<RuneTab>();
        tabIndex = (tabIndex + movement) % runeTabInstances.Length;
        if (tabIndex < 0)
        {
            tabIndex = runeTabInstances.Length - 1;
        }
        var buttons = runeTabInstances.Select(t => t.GetComponent<Button>()).ToArray();
        for (int i = 0; i < buttons.Count(); i++)
        {
            buttons[i].interactable = i == tabIndex;
        }
        var runes = inventory.GetItems(ItemType.Rune).Select(r => r.GetComponent<Rune>()).Where(r => r.GetRuneType() == runeTabs[tabIndex].GetRuneType()).ToArray();
        if (runes.Length > 0)
        {
            runesContainer.gameObject.SetActive(true);
            noRunesDescription.gameObject.SetActive(false);
            for (int i = 0; i < runes.Length; i++)
            {
                var button = Instantiate(itemButton);
                button.transform.SetParent(runesContainer);
                button.SetItemType(ItemType.Rune);
                button.SetItem(runes[i]);
                if (i == 0 || runes[i].IsEquipped())
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                }
            }
        }
        else
        {
            runesContainer.gameObject.SetActive(false);
            noRunesDescription.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the description for the rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void UpdateDescription(Rune rune)
    {
        var descriptionSuffix = rune ? rune.name : NoEquippedDescriptionSuffix;
        var descriptionText = EquippedDescriptionPrefix + descriptionSuffix;
        description.text = descriptionText;
    }
}