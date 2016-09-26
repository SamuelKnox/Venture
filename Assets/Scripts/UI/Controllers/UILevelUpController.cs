using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UILevelUpController : MonoBehaviour
{
    [Tooltip("View used to display the roons")]
    [SerializeField]
    private LevelUpRoonsView roonsView;

    [Tooltip("View used to display the description of the currently selected roon")]
    [SerializeField]
    private LevelUpRoonDescriptionView roonDescriptionView;

    [Tooltip("View used to display how much prestige is available")]
    [SerializeField]
    private LevelUpPrestigeView prestigeView;

    [Tooltip("Container to store all of the roons and weapon")]
    [SerializeField]
    private Transform itemContainer;

    private bool dirty;
    private GameObject previousSelectedGameObject;
    private int prestige;
    private Item[] items;
    private Roon[] roons;

    void Start()
    {
        LoadData();
        if (roons.Length == 0)
        {
            Debug.LogError("Cannot level up roons, because the player doesn't have any!", gameObject);
            return;
        }
        roonsView.CreateTabs(GetRoonTypes());
        roonsView.MoveTab(0, roons);
        prestigeView.UpdatePrestige(prestige);
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
        var roon = item.GetComponent<Roon>();
        if (!roon)
        {
            Debug.LogError("Could not find roon!", item.gameObject);
            return;
        }
        if (Input.GetButtonDown(InputNames.Back))
        {
            FinishLevelingUpRoons();
        }
        if (Input.GetButtonDown(InputNames.LevelUpRoon))
        {
            LevelUpRoon(roon);
            prestigeView.UpdatePrestige(prestige);
        }
        if (Input.GetButtonDown(InputNames.TabRight) && GetRoonTypes().Length > 1)
        {
            roonsView.MoveTab(1, roons);
        }
        if (Input.GetButtonDown(InputNames.TabLeft) && GetRoonTypes().Length > 1)
        {
            roonsView.MoveTab(-1, roons);
        }
        if (dirty)
        {
            dirty = false;
            roonsView.UpdateDescription(roon);
            roonDescriptionView.UpdateDescription(roon);
            prestigeView.UpdatePrestige(prestige);
        }
    }

    /// <summary>
    /// Levels up the roon, if possible
    /// </summary>
    /// <param name="roon">Roon to level</param>
    private void LevelUpRoon(Roon roon)
    {
        if (prestige < roon.GetPrestigeCostToLevelUp())
        {
            return;
        }
        int levelUp = roon.GetLevel() + 1;
        if (levelUp < Roon.GetMinLevel() || levelUp > Roon.GetMaxLevel())
        {
            return;
        }
        prestige -= roon.GetPrestigeCostToLevelUp();
        roon.SetLevel(levelUp);
        dirty = true;
    }

    /// <summary>
    /// Loads the player's roons
    /// </summary>
    /// <returns>All the roons</returns>
    private void LoadData()
    {
        prestige = SaveData.LoadPrestige();
        items = SaveData.LoadItems();
        foreach (var item in items)
        {
            item.transform.SetParent(itemContainer);
        }
        itemContainer.gameObject.SetActive(false);
        roons = items.Where(i => i.GetComponent<Roon>()).Select(r => r.GetComponent<Roon>()).ToArray();
    }

    /// <summary>
    /// Types of roons available
    /// </summary>
    /// <returns>Roon types</returns>
    private RoonType[] GetRoonTypes()
    {
        var roonTypes = new List<RoonType>();
        foreach (var roon in roons)
        {
            if (!roonTypes.Contains(roon.GetRoonType()))
            {
                roonTypes.Add(roon.GetRoonType());
            }
        }
        return roonTypes.ToArray();
    }

    /// <summary>
    /// Finishes leveling up roons and returns to game
    /// </summary>
    private void FinishLevelingUpRoons()
    {
        SaveData.SaveItems(items);
        SaveData.SavePrestige(prestige);
        SceneManager.LoadScene(SceneNames.Venture);
    }
}