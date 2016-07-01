using Devdog.InventorySystem;
using Devdog.InventorySystem.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryActionHelper))]
public class MenuController : MonoBehaviour
{
    /// <summary>
    /// Delegate for Selected Wrapper Changed
    /// </summary>
    public delegate void SelectedWrapperChanged(InventoryUIItemWrapperBase wrapper);

    /// <summary>
    /// Event called when Currently selected wrapper is changed
    /// </summary>
    public static event SelectedWrapperChanged OnSelectedWrapperChanged;

    private UIWindow[] inventoryUIWindows;
    private int currentIndex = 0;
    private InventoryActionHelper inventoryActionHelper;
    private GameObject previousSelectedGameObject;
    private ItemCollectionBase[] inventoryCollections;

    void Awake()
    {
        inventoryActionHelper = GetComponent<InventoryActionHelper>();
        var inventoryPlayer = FindObjectOfType<InventoryPlayer>();
        if (!inventoryPlayer)
        {
            Debug.LogError("Could not find Inventory Player!", gameObject);
            return;
        }
        inventoryCollections = inventoryPlayer.inventoryCollections;
        if (inventoryCollections == null || inventoryCollections.Length == 0)
        {
            Debug.LogError(inventoryPlayer + " did not have any Inventory Collections!", inventoryPlayer.gameObject);
            return;
        }
        inventoryUIWindows = inventoryCollections.Where(c => c.GetComponent<UIWindow>()).Select(w => w.GetComponent<UIWindow>()).ToArray();
    }

    void Update()
    {
        UpdateSelectedWrapper();
    }

    /// <summary>
    /// Shows the equipment UI
    /// </summary>
    public void ShowEquipment()
    {
        inventoryUIWindows[currentIndex].Show();
        inventoryActionHelper.SelectFirstWrapperOfCollection(inventoryCollections[currentIndex]);
    }

    /// <summary>
    /// Hides the equipment UI
    /// </summary>
    public void HideEquipment()
    {
        inventoryUIWindows[currentIndex].Hide();
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Iterates through player inventories
    /// </summary>
    public void IterateInventoryCollections()
    {
        currentIndex = (currentIndex + 1) % inventoryUIWindows.Length;
        ShowEquipment();
    }

    /// <summary>
    /// Updates the currently selected wrapper, and informs anyone listening for it
    /// </summary>
    private void UpdateSelectedWrapper()
    {
        if (EventSystem.current.currentSelectedGameObject != previousSelectedGameObject)
        {
            previousSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (OnSelectedWrapperChanged != null)
            {
                OnSelectedWrapperChanged(InventoryUIUtility.currentlySelectedWrapper);
            }
        }
    }
}