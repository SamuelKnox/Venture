using UnityEngine;

public class CollectRunesQuest : Quest
{
    [Tooltip("Number of runes needed to complete this quest")]
    [SerializeField]
    [Range(1, 10)]
    private int runesNeeded = 1;

    void Update()
    {
        if (IsComplete())
        {
            return;
        }
        if (PlayerManager.Player.GetInventory().GetItems(ItemType.Rune).Length >= runesNeeded)
        {
            Complete();
        }
    }

    /// <summary>
    /// If the player has few enough runes to take on this quest
    /// </summary>
    /// <returns>Player has enough runes</returns>
    public override bool IsQualified()
    {
        return PlayerManager.Player.GetInventory().GetItems(ItemType.Rune).Length < runesNeeded;
    }
}