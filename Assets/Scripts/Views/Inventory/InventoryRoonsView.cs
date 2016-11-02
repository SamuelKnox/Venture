using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomUnityLibrary;
using TMPro;

public class InventoryRoonsView : MonoBehaviour
{
    private const string EquippedDescriptionPrefix = "Equipped: ";
    private const string NoEquippedDescriptionSuffix = "None";

    [Tooltip("Roon tab prefabs to move between different types of roons")]
    [SerializeField]
    private RoonTab[] roonTabs;

    [Tooltip("Container to put item buttons for roon categories")]
    [SerializeField]
    private RectTransform categoryContainer;

    [Tooltip("Button prefab to show items")]
    [SerializeField]
    private ItemButton itemButton;

    [Tooltip("Container used to store item selection buttons")]
    [SerializeField]
    private RectTransform roonsContainer;

    [Tooltip("Description for when there are no roons available to display")]
    [SerializeField]
    private RectTransform noRoonsDescription;

    [Tooltip("Text to display in no roons description, when applicable")]
    [SerializeField]
    private string noRoonsDescriptionText;

    [Tooltip("Short description for currently selected roon")]
    [SerializeField]
    private TextMeshProUGUI description;

    private Inventory inventory;
    private int tabIndex = 0;
    private Weapon weaponViewed;
    private bool dirty = false;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (!inventory)
        {
            Debug.LogError(gameObject + " could not find Inventory!", gameObject);
            return;
        }
        if (string.IsNullOrEmpty(noRoonsDescriptionText))
        {
            Debug.LogWarning("There is no description for when roons are missing.", gameObject);
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
    /// Creates the roon buttons for the specified piece of weapon
    /// </summary>
    /// <param name="weapon">Weapon whose roons can be changed</param>
    /// <returns>Whether or not any of that weapon type was found to create buttons for</returns>
    public void CreateButtons(Weapon weapon)
    {
        weaponViewed = weapon;
        foreach (var roonTab in categoryContainer.GetComponentsInChildren<RoonTab>())
        {
            Destroy(roonTab.gameObject);
        }
        foreach (var button in roonsContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(button.gameObject);
        }
        var roonSocketTypes = weapon.GetRoonSocketTypes();
        if (roonSocketTypes.Length == 0)
        {
            Debug.LogError(weapon + " does not have any roon sockets!", weapon.gameObject);
            return;
        }
        foreach (var roonSocketType in roonSocketTypes)
        {
            var roonTabPrefab = roonTabs.Where(t => t.GetRoonType() == roonSocketType).FirstOrDefault();
            if (!roonTabPrefab)
            {
                Debug.LogError("Missing a roon tab for " + roonSocketType + "!", gameObject);
                return;
            }
            var tab = Instantiate(roonTabPrefab);
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
    /// Moves the roon tab by the index passed to the new tab
    /// </summary>
    /// <param name="movement">Amount to move the tab to the left or right</param>
    public void MoveTab(int movement)
    {
        foreach (var itemButton in roonsContainer.GetComponentsInChildren<ItemButton>())
        {
            Destroy(itemButton.gameObject);
        }
        var roonTabInstances = categoryContainer.GetComponentsInChildren<RoonTab>();
        tabIndex = (tabIndex + movement) % roonTabInstances.Length;
        if (tabIndex < 0)
        {
            tabIndex = roonTabInstances.Length - 1;
        }
        var buttons = roonTabInstances.Select(t => t.GetComponent<Button>()).ToArray();
        for (int i = 0; i < buttons.Count(); i++)
        {
            buttons[i].interactable = i == tabIndex;
        }
        var roonType = roonTabInstances[tabIndex].GetRoonType();
        var roons = inventory.GetItems(ItemType.Roon).Select(r => r.GetComponent<Roon>()).Where(r => r.GetRoonType() == roonType).ToArray();
        if (roons.Length > 0)
        {
            roonsContainer.gameObject.SetActive(true);
            noRoonsDescription.gameObject.SetActive(false);
            for (int i = 0; i < roons.Length; i++)
            {
                var button = Instantiate(itemButton);
                button.transform.SetParent(roonsContainer);
                button.SetItemType(ItemType.Roon);
                button.SetItem(roons[i]);
                if (i == 0 || weaponViewed.GetRoon(roonType) == roons[i])
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                }
            }
        }
        else
        {
            roonsContainer.gameObject.SetActive(false);
            noRoonsDescription.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the description for the roon
    /// </summary>
    /// <param name="roon">Roon to describe</param>
    public void UpdateDescription(Roon roon)
    {
        var descriptionSuffix = roon ? roon.name : NoEquippedDescriptionSuffix;
        var descriptionText = EquippedDescriptionPrefix + descriptionSuffix;
        description.text = descriptionText;
    }
}