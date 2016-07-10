using CustomUnityLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int prestige;
    public int gold;
    public float level;
    public List<string> weaponNames = new List<string>();
    public List<bool> weaponsEquipped = new List<bool>();
    public List<string> armorNames = new List<string>();
    public List<bool> armorsEquipped = new List<bool>();
    public List<string> runeNames = new List<string>();
    public List<string> runeEquipmentNames = new List<string>();
    public List<int> runeLevels = new List<int>();
    public List<string> questNames = new List<string>();
    public List<bool> questsComplete = new List<bool>();

    /// <summary>
    /// Converts player to json of PlayerData
    /// </summary>
    /// <param name="player">Player to convert</param>
    /// <returns>Json data</returns>
    public static string ToJson(Player player)
    {
        var playerData = new PlayerData();
        playerData.prestige = player.GetPrestige();
        playerData.gold = player.GetGold();
        playerData.level = player.GetLevel();
        foreach (var weapon in player.GetWeapons())
        {
            weapon.name = weapon.name.TrimEnd(GameObjectUtility.CloneSuffix);
            playerData.weaponNames.Add(weapon.name);
            playerData.weaponsEquipped.Add(weapon.IsEquipped());
        }
        foreach (var armor in player.GetArmor())
        {
            armor.name = armor.name.TrimEnd(GameObjectUtility.CloneSuffix);
            playerData.armorNames.Add(armor.name);
            playerData.armorsEquipped.Add(armor.IsEquipped());
        }
        foreach (var rune in player.GetRunes())
        {
            rune.name = rune.name.TrimEnd(GameObjectUtility.CloneSuffix);
            playerData.runeNames.Add(rune.name);
            if (rune.IsEquipped())
            {
                var allEquipment = player.GetInventory().GetItems().Where(e => e.GetComponent<Equipment>()).Select(e => e.GetComponent<Equipment>());
                var runeEquipment = allEquipment.Where(e => e.GetRune(rune.GetRuneType()) == rune).First();
                playerData.runeEquipmentNames.Add(runeEquipment.name);
            }
            else
            {
                playerData.runeEquipmentNames.Add(null);
            }
            playerData.runeLevels.Add(rune.GetLevel());
        }
        foreach (var quest in player.GetQuests())
        {
            quest.name = quest.name.TrimEnd(GameObjectUtility.CloneSuffix);
            playerData.questNames.Add(quest.name);
            playerData.questsComplete.Add(quest.IsComplete());
        }
        string playerDataJson = JsonUtility.ToJson(playerData);
        return playerDataJson;
    }

    /// <summary>
    /// Applies the player data to the player
    /// </summary>
    /// <param name="player">Player to update</param>
    public void Apply(Player player)
    {
        player.SetPrestige(prestige);
        player.SetGold(gold);
        player.SetLevel(level);
        player.InitializeWeapons(weaponNames.ToArray(), weaponsEquipped.ToArray());
        player.InitializeArmor(armorNames.ToArray(), armorsEquipped.ToArray());
        player.InitializeRunes(runeNames.ToArray(), runeEquipmentNames.ToArray(), runeLevels.ToArray());
        player.InitializeQuests(questNames.ToArray(), questsComplete.ToArray());
    }
}