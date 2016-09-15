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
        PlayerManager.Player.SetGold(PlayerManager.Player.GetGold() + goldIncrease);
    }
}