using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UILevelUpController : MonoBehaviour
{
    private const string SaveFilePath = FilePaths.SaveFile + FilePaths.SaveTagAffix + FilePaths.PlayerTag;

    [Tooltip("View used to display the runes")]
    [SerializeField]
    private LevelUpRunesView runesView;

    [Tooltip("View used to display the description of the currently selected rune")]
    [SerializeField]
    private LevelUpRuneDescriptionView runeDescriptionView;

    [Tooltip("Container to store all of the runes and equipment")]
    [SerializeField]
    private Transform itemContainer;

    private bool dirty;
    private GameObject previousSelectedGameObject;
    private int prestige;
    private Item[] items;
    private Rune[] runes;

    void Start()
    {
        LoadData();
        runesView.CreateTabs(GetRuneTypes());
        runesView.MoveTab(0, runes);
        dirty = true;
    }

    void Update()
    {
        var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedGameObject != previousSelectedGameObject)
        {
            dirty = true;
        }
        previousSelectedGameObject = currentSelectedGameObject;
        var itemButton = currentSelectedGameObject.GetComponent<ItemButton>();
        if (!itemButton)
        {
            Debug.Log(currentSelectedGameObject + " could not find ItemButton!", currentSelectedGameObject);
            return;
        }
        var item = itemButton.GetItem();
        if (!item)
        {
            Debug.LogError("Expecting item in ItemButton!", itemButton.gameObject);
            return;
        }
        var rune = item.GetComponent<Rune>();
        if (!rune)
        {
            Debug.LogError("Could not find rune!", item.gameObject);
            return;
        }
        if (Input.GetButtonDown(InputNames.Inventory))
        {
            FinishLevelingUpRunes();
        }
        if (Input.GetButtonDown(InputNames.LevelUpRune))
        {
            LevelUpRune(rune);
        }
        if (Input.GetButtonDown(InputNames.TabRight) && GetRuneTypes().Length > 1)
        {
            runesView.MoveTab(1, runes);
        }
        if (Input.GetButtonDown(InputNames.TabLeft) && GetRuneTypes().Length > 1)
        {
            runesView.MoveTab(-1, runes);
        }
        if (dirty)
        {
            dirty = false;
            runesView.UpdateDescription(rune);
            runeDescriptionView.UpdateDescription(rune);
        }
    }

    /// <summary>
    /// Levels up the rune, if possible
    /// </summary>
    /// <param name="rune">Rune to level</param>
    private void LevelUpRune(Rune rune)
    {
        if (prestige < rune.GetPrestigeCostToLevelUp())
        {
            return;
        }
        bool success = rune.SetLevel(rune.GetLevel() + 1);
        if (success)
        {
            prestige -= rune.GetPrestigeCostToLevelUp();
        }
        dirty = true;
    }

    /// <summary>
    /// Loads the player's runes
    /// </summary>
    /// <returns>All the runes</returns>
    private void LoadData()
    {
        prestige = SaveData.LoadPrestige();
        items = SaveData.LoadItems();
        foreach (var item in items)
        {
            item.transform.SetParent(itemContainer);
        }
        itemContainer.gameObject.SetActive(false);
        runes = items.Where(i => i.GetComponent<Rune>()).Select(r => r.GetComponent<Rune>()).ToArray(); 
        if (runes.Count() == 0)
        {
            FinishLevelingUpRunes();
        }
    }

    /// <summary>
    /// Types of runes available
    /// </summary>
    /// <returns>Rune types</returns>
    private RuneType[] GetRuneTypes()
    {
        var runeTypes = new List<RuneType>();
        foreach (var rune in runes)
        {
            if (!runeTypes.Contains(rune.GetRuneType()))
            {
                runeTypes.Add(rune.GetRuneType());
            }
        }
        return runeTypes.ToArray();
    }

    /// <summary>
    /// Finishes leveling up runes and returns to game
    /// </summary>
    private void FinishLevelingUpRunes()
    {
        SaveData.SaveItems(items);
        SaveData.SavePrestige(prestige);
        SceneManager.LoadScene(SceneNames.Venture);
    }
}