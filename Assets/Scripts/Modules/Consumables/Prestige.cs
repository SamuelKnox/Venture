using UnityEngine;

public class Prestige : Consumable
{
    [Tooltip("Amount to increase prestige by")]
    [SerializeField]
    [Range(0, 10)]
    private int prestigeIncrease = 1;

    /// <summary>
    /// Consumes the prestige
    /// </summary>
    public override void Consume()
    {
        AddPlayerPrestige();
    }

    /// <summary>
    /// Increases the player prestige
    /// </summary>
    private void AddPlayerPrestige()
    {
        PlayerManager.Player.SetPrestige(PlayerManager.Player.GetPrestige() + prestigeIncrease);
    }
}