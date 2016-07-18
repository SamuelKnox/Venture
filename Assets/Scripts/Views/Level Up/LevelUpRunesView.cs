using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CustomUnityLibrary;
using TMPro;
using System;

public class LevelUpRunesView : MonoBehaviour
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

    [Tooltip("Short description for currently selected rune")]
    [SerializeField]
    private TextMeshProUGUI description;

    private int tabIndex = 0;

    /// <summary>
    /// Creates the category tabs for all of the runes
    /// </summary>
    public void CreateTabs(RuneType[] runeTypes)
    {
        foreach (var runeTab in runeTabs)
        {
            if (Array.IndexOf(runeTypes, runeTab.GetRuneType()) < 0)
            {
                continue;
            }
            var runeTabInstance = Instantiate(runeTab);
            runeTabInstance.transform.SetParent(categoryContainer);
            var button = runeTabInstance.GetComponent<Button>();
            if (!button)
            {
                Debug.LogError(runeTabInstance + " could not find button component!", runeTabInstance.gameObject);
                return;
            }
            button.SetNavigation(Navigation.Mode.None);
        }
    }

    /// <summary>
    /// Moves the rune tab by the index passed to the new tab
    /// </summary>
    /// <param name="movement">Amount to move the tab to the left or right</param>
    /// <param name="runes">Runes which can be displayed</param>
    public void MoveTab(int movement, Rune[] runes)
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
        var runesOfType = runes.Where(r => r.GetRuneType() == runeTabInstances[tabIndex].GetRuneType()).ToArray();
        for (int i = 0; i < runesOfType.Length; i++)
        {
            var button = Instantiate(itemButton);
            button.transform.SetParent(runesContainer);
            button.SetItemType(ItemType.Rune);
            button.SetItem(runesOfType[i]);
            if (i == 0 || runesOfType[i].IsEquipped())
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
    }

    /// <summary>
    /// Updates the description for the rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void UpdateDescription(Rune rune)
    {
        description.text = rune.GetDescription();
    }
}