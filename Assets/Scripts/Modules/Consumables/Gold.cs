using UnityEngine;

public class Gold : Consumable
{
    [Tooltip("Amount to increase gold by")]
    [SerializeField]
    [Range(0, 100)]
    private int goldIncrease = 10;

    /// <summary>
    /// Consumes the gold
    /// </summary>
    public override void Consume()
    {
        IncreasePlayerGold();
    }

    /// <summary>
    /// Increases the player's gold
    /// </summary>
    private void IncreasePlayerGold()
    {
        var player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find Player!", gameObject);
            return;
        }
        player.SetGold(player.GetGold() + goldIncrease);
    }
}