using UnityEngine;

public class CollectRoonsQuest : Quest
{
    [Tooltip("Number of roons needed to complete this quest")]
    [SerializeField]
    [Range(1, 10)]
    private int roonsNeeded = 1;

    void Update()
    {
        if (IsComplete())
        {
            return;
        }
        if (PlayerManager.Player.GetInventory().GetItems(ItemType.Roon).Length >= roonsNeeded)
        {
            Complete();
        }
    }

    /// <summary>
    /// If the player has few enough roons to take on this quest
    /// </summary>
    /// <returns>Player has enough roons</returns>
    public override bool IsQualified()
    {
        return PlayerManager.Player.GetInventory().GetItems(ItemType.Roon).Length < roonsNeeded;
    }
}