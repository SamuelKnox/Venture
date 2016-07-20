using UnityEngine;

public class HealthRune : RedRune
{
    /// <summary>
    /// Increases the player's max hit points
    /// </summary>
    /// <param name="equipment">Equipment with rune attached</param>
    public override void Activate(Equipment equipment)
    {
        AdjustPlayerHitPoints(GetBaseValue());
        AdjustPlayerInvincibilityTime(GetSpecialValue());
    }

    /// <summary>
    /// Decreases the player's max hit points
    /// </summary>
    /// <param name="equipment">Equipment with rune attached</param>
    public override void Deactivate(Equipment equipment)
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
        var playerHealth = GetPlayer().GetComponentInChildren<Health>();
        playerHealth.SetMaxHitPoints(playerHealth.GetMaxHitPoints() + change);
        playerHealth.SetCurrentHitPoints(playerHealth.GetMaxHitPoints());
    }

    /// <summary>
    /// Changes the duration that the player is invincible for after taking damage
    /// </summary>
    /// <param name="time">Change in seconds</param>
    private void AdjustPlayerInvincibilityTime(float time)
    {
        var playerHealth = GetPlayer().GetComponentInChildren<Health>();
        playerHealth.SetInvincibilityTime(playerHealth.GetInvincibilityTime() + time);
    }
}