using UnityEngine;

public class HealthRoon : RedRoon
{
    /// <summary>
    /// Increases the player's max hit points
    /// </summary>
    /// <param name="weapon">Weapon with roon attached</param>
    public override void Activate(Weapon weapon)
    {
        AdjustPlayerHitPoints(GetBaseValue());
        AdjustPlayerInvincibilityTime(GetSpecialValue());
    }

    /// <summary>
    /// Decreases the player's max hit points
    /// </summary>
    /// <param name="weapon">Weapon with roon attached</param>
    public override void Deactivate(Weapon weapon)
    {
        AdjustPlayerHitPoints(-GetBaseValue());
        AdjustPlayerInvincibilityTime(-GetSpecialValue());
    }

    /// <summary>
    /// Changes the player's max hit points, and gives them 100% health
    /// </summary>
    /// <param name="change">Amount to change by</param>
    private void AdjustPlayerHitPoints(float change)
    {
        var playerHealth = PlayerManager.Player.GetComponentInChildren<Health>();
        playerHealth.SetMaxHitPoints(playerHealth.GetMaxHitPoints() + change);
        playerHealth.SetCurrentHitPoints(playerHealth.GetMaxHitPoints());
    }

    /// <summary>
    /// Changes the duration that the player is invincible for after taking damage
    /// </summary>
    /// <param name="time">Change in seconds</param>
    private void AdjustPlayerInvincibilityTime(float time)
    {
        var playerHealth = PlayerManager.Player.GetComponentInChildren<Health>();
        playerHealth.SetInvincibilityTime(playerHealth.GetInvincibilityTime() + time);
    }
}