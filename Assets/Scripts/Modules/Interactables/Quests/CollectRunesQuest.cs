using UnityEngine;

public class CollectRunesQuest : Quest
{
    private Player player;

    [Tooltip("Number of runes needed to complete this quest")]
    [SerializeField]
    [Range(1, 10)]
    private int runesNeeded = 1;

    protected override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find Player!", gameObject);
            return;
        }
    }

    void Update()
    {
        if (!IsActiveQuest() || IsComplete())
        {
            return;
        }
        if (player.GetInventory().GetItems(ItemType.Rune).Length >= runesNeeded)
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
        return player.GetInventory().GetItems(ItemType.Rune).Length < runesNeeded;
    }
}