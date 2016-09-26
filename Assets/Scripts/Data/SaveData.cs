using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int prestige;
    public int gold;
    public float level;
    public List<string> weaponNames = new List<string>();
    public List<bool> weaponsEquipped = new List<bool>();
    public List<string> roonNames = new List<string>();
    public List<string> roonWeaponNames = new List<string>();
    public List<int> roonLevels = new List<int>();
    public List<string> questNames = new List<string>();
    public List<bool> questsComplete = new List<bool>();

    /// <summary>
    /// Loads the player's prestige
    /// </summary>
    /// <returns>Player prestige</returns>
    public static int LoadPrestige()
    {
        var saveData = LoadSaveData();
        return saveData.prestige;
    }

    /// <summary>
    /// Saves the player's prestige
    /// </summary>
    /// <param name="prestige">New prestige</param>
    public static void SavePrestige(int prestige)
    {
        var saveData = LoadSaveData();
        saveData.prestige = prestige;
        SaveSaveData(saveData);
    }

    /// <summary>
    /// Loads the player's gold
    /// </summary>
    /// <returns>Player gold</returns>
    public static int LoadGold()
    {
        var saveData = LoadSaveData();
        return saveData.gold;
    }

    /// <summary>
    /// Saves the player's gold
    /// </summary>
    /// <param name="gold">New gold</param>
    public static void SaveGold(int gold)
    {
        var saveData = LoadSaveData();
        saveData.gold = gold;
        SaveSaveData(saveData);
    }

    /// <summary>
    /// Loads the player's level
    /// </summary>
    /// <returns>Player level</returns>
    public static float LoadLevel()
    {
        var saveData = LoadSaveData();
        return saveData.level;
    }

    /// <summary>
    /// Saves the player's level
    /// </summary>
    /// <param name="level">New player level</param>
    public static void SaveLevel(float level)
    {
        var saveData = LoadSaveData();
        saveData.level = level;
        SaveSaveData(saveData);
    }

    /// <summary>
    /// Loads the players items, including weapons and roons
    /// </summary>
    /// <returns>Weapons and roons</returns>
    public static Item[] LoadItems()
    {
        var saveData = LoadSaveData();
        var weaponNames = saveData.weaponNames;
        var weaponsEquipped = saveData.weaponsEquipped;
        if (weaponNames.Count != weaponsEquipped.Count)
        {
            Debug.LogError("Every weapon must either be equipped or unequipped!");
            return null;
        }
        var weaponPrefabs = Resources.LoadAll<Weapon>(FilePaths.Weapons);
        var weapon = new List<Weapon>();
        for (int i = 0; i < weaponNames.Count && i < weaponsEquipped.Count; i++)
        {
            var collectedWeaponPrefab = weaponPrefabs.Where(p => p.name == weaponNames[i]).First();
            var weaponInstance = UnityEngine.Object.Instantiate(collectedWeaponPrefab) as Weapon;
            weaponInstance.name = weaponInstance.name.TrimEnd(GameObjectUtility.CloneSuffix);
            if (weaponsEquipped[i])
            {
                weaponInstance.SetEquipped(true);
            }
            weapon.Add(weaponInstance);
        }
        var roonNames = saveData.roonNames;
        var roonLevels = saveData.roonLevels;
        var roonWeaponNames = saveData.roonWeaponNames;
        if (roonNames.Count != roonLevels.Count)
        {
            Debug.LogError("Every roon must have a level!");
            return null;
        }
        var roonPrefabs = Resources.LoadAll<Roon>(FilePaths.Roons);
        var roons = new List<Roon>();
        for (int i = 0; i < roonNames.Count && i < roonLevels.Count; i++)
        {
            var collectedRoonPrefab = roonPrefabs.Where(r => r.name == roonNames[i]).First();
            var roonInstance = UnityEngine.Object.Instantiate(collectedRoonPrefab) as Roon;
            roonInstance.name = roonInstance.name.TrimEnd(GameObjectUtility.CloneSuffix);
            roonInstance.SetLevel(saveData.roonLevels[i]);
            roons.Add(roonInstance);
            if (!string.IsNullOrEmpty(roonWeaponNames[i]))
            {
                var roonedWeapon = weapon.Where(e => e.name == roonWeaponNames[i]).First();
                roonedWeapon.SetRoon(roonInstance);
            }
        }
        var items = new List<Item>();
        items.AddRange(weapon.ToArray());
        items.AddRange(roons.ToArray());
        return items.ToArray();
    }

    /// <summary>
    /// Saves the player's items, overwriting the existing items
    /// </summary>
    /// <param name="items">Items to save</param>
    public static void SaveItems(Item[] items)
    {
        var saveData = LoadSaveData();
        var weaponNames = new List<string>();
        var weaponsEquipped = new List<bool>();
        var roonNames = new List<string>();
        var roonLevels = new List<int>();
        var roonWeaponNames = new List<string>();
        var weapons = items.Where(i => i.GetComponent<Weapon>()).Select(e => e.GetComponent<Weapon>());
        var roons = items.Where(i => i.GetComponent<Roon>()).Select(r => r.GetComponent<Roon>()).ToList();
        foreach (var weapon in weapons)
        {
            string weaponName = weapon.name.TrimEnd(GameObjectUtility.CloneSuffix);
            weaponNames.Add(weaponName);
            weaponsEquipped.Add(weapon.IsEquipped());
            foreach (var roon in weapon.GetRoons())
            {
                roonNames.Add(roon.name.TrimEnd(GameObjectUtility.CloneSuffix));
                roonLevels.Add(roon.GetLevel());
                roonWeaponNames.Add(weaponName);
                roons.Remove(roon);
            }
        }
        foreach (var roon in roons)
        {
            roonNames.Add(roon.name.TrimEnd(GameObjectUtility.CloneSuffix));
            roonLevels.Add(roon.GetLevel());
            roonWeaponNames.Add(null);
        }
        saveData.weaponNames = weaponNames;
        saveData.weaponsEquipped = weaponsEquipped;
        saveData.roonNames = roonNames;
        saveData.roonLevels = roonLevels;
        saveData.roonWeaponNames = roonWeaponNames;
        SaveSaveData(saveData);
    }

    /// <summary>
    /// Loads the quests the player has collected
    /// </summary>
    /// <returns>Player quests</returns>
    public static Quest[] LoadQuests()
    {
        var saveData = LoadSaveData();
        var questNames = saveData.questNames;
        var questsComplete = saveData.questsComplete;
        if (questNames.Count != questsComplete.Count)
        {
            Debug.LogError("Every quest must either be complete or incomplete!");
            return null;
        }
        var questPrefabs = Resources.LoadAll<Quest>(FilePaths.Quests);
        var quests = new List<Quest>();
        for (int i = 0; i < questNames.Count && i < questsComplete.Count; i++)
        {
            var collectedQuestPrefab = questPrefabs.Where(q => q.name == questNames[i]).First();
            var questInstance = UnityEngine.Object.Instantiate(collectedQuestPrefab) as Quest;
            questInstance.SetComplete(questsComplete[i]);
            questInstance.SetLongTermQuest(true);
            quests.Add(questInstance);
        }
        return quests.ToArray();
    }

    /// <summary>
    /// Saves the quests the player has
    /// </summary>
    /// <param name="quests">Player quests</param>
    public static void SaveQuests(Quest[] quests)
    {
        var saveData = LoadSaveData();
        var questNames = new List<string>();
        var questsComplete = new List<bool>();
        foreach (var quest in quests)
        {
            if (!quest.IsLongTermQuest())
            {
                continue;
            }
            questNames.Add(quest.name.TrimEnd(GameObjectUtility.CloneSuffix));
            questsComplete.Add(quest.IsComplete());
        }
        saveData.questNames = questNames;
        saveData.questsComplete = questsComplete;
        SaveSaveData(saveData);
    }

    /// <summary>
    /// Loads the save data
    /// </summary>
    /// <returns>Save Data, or null if none exists</returns>
    private static SaveData LoadSaveData()
    {
        if (!ES2.Exists(FilePaths.SaveFile))
        {
            return new SaveData();
        }
        string saveDataJson = ES2.Load<string>(FilePaths.SaveFile);
        var saveData = JsonUtility.FromJson<SaveData>(saveDataJson);
        return saveData;
    }

    /// <summary>
    /// Saves the save data
    /// </summary>
    /// <param name="saveData">Save data to save</param>
    public static void SaveSaveData(SaveData saveData)
    {
        var saveDataJson = JsonUtility.ToJson(saveData);
        ES2.Save(saveDataJson, FilePaths.SaveFile);
    }
}