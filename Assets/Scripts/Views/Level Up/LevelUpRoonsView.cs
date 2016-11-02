using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomUnityLibrary;
using TMPro;
using System;

public class LevelUpRoonsView : MonoBehaviour
{
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

    [Tooltip("Short description for currently selected roon")]
    [SerializeField]
    private TextMeshProUGUI description;

    private int tabIndex = 0;

    /// <summary>
    /// Creates the category tabs for all of the roons
    /// </summary>
    public void CreateTabs(RoonType[] roonTypes)
    {
        foreach (var roonTab in roonTabs)
        {
            if (Array.IndexOf(roonTypes, roonTab.GetRoonType()) < 0)
            {
                continue;
            }
            var roonTabInstance = Instantiate(roonTab);
            roonTabInstance.transform.SetParent(categoryContainer);
            var button = roonTabInstance.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(roonTabInstance + " could not find button component!", roonTabInstance.gameObject);
                return;
            }
            button.SetNavigation(Navigation.Mode.None);
        }
    }

    /// <summary>
    /// Moves the roon tab by the index passed to the new tab
    /// </summary>
    /// <param name="movement">Amount to move the tab to the left or right</param>
    /// <param name="roons">Roons which can be displayed</param>
    public void MoveTab(int movement, Roon[] roons)
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
        var roonsOfType = roons.Where(r => r.GetRoonType() == roonTabInstances[tabIndex].GetRoonType()).ToArray();
        for (int i = 0; i < roonsOfType.Length; i++)
        {
            var button = Instantiate(itemButton);
            button.transform.SetParent(roonsContainer);
            button.SetItemType(ItemType.Roon);
            button.SetItem(roonsOfType[i]);
            if (i == 0 || roonsOfType[i].IsEquipped())
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
    }

    /// <summary>
    /// Updates the description for the roon
    /// </summary>
    /// <param name="roon">Roon to describe</param>
    public void UpdateDescription(Roon roon)
    {
        description.text = roon.GetDescription();
    }
}