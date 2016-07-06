//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class InventoryCategoriesView : MonoBehaviour
//{
//    private const float HorizontalTabSpacing = 10.0f;

//    [Tooltip("Panel which stores all of the category tabs")]
//    [SerializeField]
//    private RectTransform categoryTabPanel;

//    [Tooltip("All of the item tabs which may be used")]
//    [SerializeField]
//    private EquipmentTab[] equipmentTabs;

//    [Tooltip("All of the rune tabs which may be used")]
//    [SerializeField]
//    private RuneTab[] runeTabs;

//    private int currentIndex = 0;
//    private List<Tab> tabs;
//    private InventoryBrowserView browserView;

//    void Awake()
//    {
//        if (equipmentTabs.Length == 0)
//        {
//            Debug.LogError("There must be at least one item tab!", gameObject);
//            return;
//        }
//        if (runeTabs.Length == 0)
//        {
//            Debug.LogError("There must be at least one rune tab!", gameObject);
//            return;
//        }
//        browserView = FindObjectOfType<InventoryBrowserView>();
//        if (!browserView)
//        {
//            Debug.LogError(gameObject + " could not find InventoryBrowserView!", gameObject);
//            return;
//        }
//    }

//    void Update()
//    {
//        if (Input.GetButtonDown(InputNames.TabLeft))
//        {
//            MoveTab(-1);
//        }
//        if (Input.GetButtonDown(InputNames.TabRight))
//        {
//            MoveTab(1);
//        }
//    }

//    /// <summary>
//    /// Gets the equipment type for the current index of equipment tabs
//    /// </summary>
//    /// <returns>Equipment type</returns>
//    public ItemType GetEquipmentType()
//    {
//        return equipmentTabs[currentIndex].GetItemType();
//    }

//    /// <summary>
//    /// Gets the rune type for the current index of rune tabs
//    /// </summary>
//    /// <returns>Rune type</returns>
//    public RuneType GetRuneType()
//    {
//        return runeTabs[currentIndex].GetRuneType();
//    }

//    /// <summary>
//    /// Creates the tabs for the respective inventory mode
//    /// </summary>
//    public void CreateTabs(InventoryMode inventoryMode)
//    {
//        currentIndex = 0;
//        foreach (Transform child in categoryTabPanel)
//        {
//            Destroy(child.gameObject);
//        }
//        switch (inventoryMode)
//        {
//            case InventoryMode.EquipmentBrowser:
//                CreateEquipmentTabs();
//                break;
//            case InventoryMode.RuneEditor:
//                bool success = CreateRuneTabs();
//                if (!success)
//                {
//                    inventoryMode = InventoryMode.EquipmentBrowser;
//                    CreateTabs(inventoryMode);
//                }
//                break;
//            default:
//                Debug.LogError("An invalid InventoryMode (" + inventoryMode.ToString() + ") was used!", gameObject);
//                return;
//        }
//        MoveTab(0);
//    }

//    /// <summary>
//    /// Creates the tabs for equipment browsing
//    /// </summary>
//    private void CreateEquipmentTabs()
//    {
//        tabs = new List<Tab>();
//        for (int i = 0; i < equipmentTabs.Length; i++)
//        {
//            var equipmentTabInstance = Instantiate(equipmentTabs[i]);
//            var equipmentTabTransform = equipmentTabInstance.GetComponent<RectTransform>();
//            if (!equipmentTabTransform)
//            {
//                Debug.LogError(equipmentTabInstance.name + " is missing a RectTransform!", equipmentTabInstance.gameObject);
//                return;
//            }
//            equipmentTabTransform.SetParent(categoryTabPanel);
//            float xPosition = i * (equipmentTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
//            float yPosition = 0.0f;
//            equipmentTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
//            tabs.Add(equipmentTabInstance);
//        }
//    }

//    /// <summary>
//    /// Creates the tabs for rune browsing
//    /// </summary>
//    /// <returns>Whether or not the tabs were successfully created</returns>
//    private bool CreateRuneTabs()
//    {
//        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
//        if (!currentSelectedGameObject)
//        {
//            return false;
//        }
//        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
//        tabs = new List<Tab>();
//        if (!itemButton)
//        {
//            Debug.LogError("Item Button is missing from the currently selected game object!", currentSelectedGameObject);
//            return false;
//        }
//        var activeItem = itemButton.GetItem();
//        if (!activeItem)
//        {
//            Debug.LogError("The Item represented by the Item Button is null!", itemButton.gameObject);
//            return false;
//        }
//        browserView.SetActiveEquipmentForRunes(activeItem.GetComponent<Equipment>());
//        if (!browserView.GetActiveEquipmentForRunes())
//        {
//            Debug.LogError(browserView.GetActiveEquipmentForRunes() + " is not Equipment, and it needs to be to access Runes!", browserView.GetActiveEquipmentForRunes().gameObject);
//            return false;
//        }
//        var runeTypes = browserView.GetActiveEquipmentForRunes().GetRuneSocketTypes();
//        int skippedRunes = 0;
//        for (int i = 0; i < runeTabs.Length; i++)
//        {
//            if (!runeTypes.Contains(runeTabs[i].GetRuneType()))
//            {
//                skippedRunes++;
//                continue;
//            }
//            var runeTabInstance = Instantiate(runeTabs[i]);
//            var runeTabTransform = runeTabInstance.GetComponent<RectTransform>();
//            if (!runeTabTransform)
//            {
//                Debug.LogError(runeTabInstance.name + " is missing a RectTransform!", runeTabInstance.gameObject);
//                return false;
//            }
//            runeTabTransform.SetParent(categoryTabPanel);
//            float xPosition = (i - skippedRunes) * (runeTabTransform.sizeDelta.x + HorizontalTabSpacing) + HorizontalTabSpacing;
//            float yPosition = 0.0f;
//            runeTabTransform.anchoredPosition = new Vector2(xPosition, yPosition);
//            tabs.Add(runeTabInstance);
//        }
//        return true;
//    }

//    /// <summary>
//    /// Moves the tab left or right by the specified number
//    /// </summary>
//    /// <param name="movement">Number of tabs to move</param>
//    private void MoveTab(int movement)
//    {
//        tabs[currentIndex].GetImage().color = Color.white;
//        currentIndex = (currentIndex + movement) % tabs.Count;
//        if (currentIndex < 0)
//        {
//            currentIndex = tabs.Count - 1;
//        }
//        tabs[currentIndex].GetImage().color = Color.red;
//        EventSystem.current.SetSelectedGameObject(null);
//        browserView.PopulateContentPanel();
//    }
//}
