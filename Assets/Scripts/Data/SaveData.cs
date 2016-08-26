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
    public List<string> equipmentNames = new List<string>();
    public List<bool> equipmentEquipped = new List<bool>();
    public List<string> runeNames = new List<string>();
    public List<string> runeEquipmentNames = new List<string>();
    public List<int> runeLevels = new List<int>();
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
    /// Loads the players items, including equipment and runes
    /// </summary>
    /// <returns>Equipment and runes</returns>
    public static Item[] LoadItems()
    {
        var saveData = LoadSaveData();
        var equipmentNames = saveData.equipmentNames;
        var equipmentEquipped = saveData.equipmentEquipped;
        if (equipmentNames.Count != equipmentEquipped.Count)
        {
            Debug.LogError("Every piece of equipment must either be equipped or unequipped!");
            return null;
        }
        var equipmentPrefabs = Resources.LoadAll<Equipment>(FilePaths.Equipment);
        var equipment = new List<Equipment>();
        for (int i = 0; i < equipmentNames.Count && i < equipmentEquipped.Count; i++)
        {
            var collectedEquipmentPrefab = equipmentPrefabs.Where(p => p.name == equipmentNames[i]).First();
            var equipmentPiece = UnityEngine.Object.Instantiate(collectedEquipmentPrefab) as Equipment;
            equipmentPiece.name = equipmentPiece.name.TrimEnd(GameObjectUtility.CloneSuffix);
            if (equipmentEquipped[i])
            {
                equipmentPiece.SetEquipped(true);
            }
            equipment.Add(equipmentPiece);
        }
        var runeNames = saveData.runeNames;
        var runeLevels = saveData.runeLevels;
        var runeEquipmentNames = saveData.runeEquipmentNames;
        if (runeNames.Count != runeLevels.Count)
        {
            Debug.LogError("Every rune must have a level!");
            return null;
        }
        var runePrefabs = Resources.LoadAll<Rune>(FilePaths.Runes);
        var runes = new List<Rune>();
        for (int i = 0; i < runeNames.Count && i < runeLevels.Count; i++)
        {
            var collectedRunePrefab = runePrefabs.Where(r => r.name == runeNames[i]).First();
            var runeInstance = UnityEngine.Object.Instantiate(collectedRunePrefab) as Rune;
            runeInstance.name = runeInstance.name.TrimEnd(GameObjectUtility.CloneSuffix);
            runeInstance.SetLevel(saveData.runeLevels[i]);
            runes.Add(runeInstance);
            if (!string.IsNullOrEmpty(runeEquipmentNames[i]))
            {
                var equipmentPiece = equipment.Where(e => e.name == runeEquipmentNames[i]).First();
                equipmentPiece.SetRune(runeInstance);
            }
        }
        var items = new List<Item>();
        items.AddRange(equipment.ToArray());
        items.AddRange(runes.ToArray());
        return items.ToArray();
    }

    /// <summary>
    /// Saves the player's items, overwriting the existing items
    /// </summary>
    /// <param name="items">Items to save</param>
    public static void SaveItems(Item[] items)
    {
        var saveData = LoadSaveData();
        var equipmentNames = new List<string>();
        var equipmentEquipped = new List<bool>();
        var runeNames = new List<string>();
        var runeLevels = new List<int>();
        var runeEquipmentNames = new List<string>();
        var equipment = items.Where(i => i.GetComponent<Equipment>()).Select(e => e.GetComponent<Equipment>());
        var runes = items.Where(i => i.GetComponent<Rune>()).Select(r => r.GetComponent<Rune>()).ToList();
        foreach (var equipmentPiece in equipment)
        {
            string equipmentName = equipmentPiece.name.TrimEnd(GameObjectUtility.CloneSuffix);
            equipmentNames.Add(equipmentName);
            equipmentEquipped.Add(equipmentPiece.IsEquipped());
            foreach (var rune in equipmentPiece.GetRunes())
            {
                runeNames.Add(rune.name.TrimEnd(GameObjectUtility.CloneSuffix));
                runeLevels.Add(rune.GetLevel());
                runeEquipmentNames.Add(equipmentName);
                runes.Remove(rune);
            }
        }
        foreach (var rune in runes)
        {
            runeNames.Add(rune.name.TrimEnd(GameObjectUtility.CloneSuffix));
            runeLevels.Add(rune.GetLevel());
            runeEquipmentNames.Add(null);
        }
        saveData.equipmentNames = equipmentNames;
        saveData.equipmentEquipped = equipmentEquipped;
        saveData.runeNames = runeNames;
        saveData.runeLevels = runeLevels;
        saveData.runeEquipmentNames = runeEquipmentNames;
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