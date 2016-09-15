using System;
using UnityEngine;

public abstract class Armor : Equipment
{
    /// <summary>
    /// Sets the damage over time amount that is applied by this armor when an enemy comes into contact with the player
    /// </summary>
    /// <param name="damageOverTime">Damage over time amount to apply</param>
    public override void SetDamageOverTime(float damageOverTime)
    {
        var playerDamage = PlayerManager.Player.GetDefensiveDamage();
        playerDamage.SetDamageOverTime(playerDamage.GetDamageOverTime() - GetDamageOverTime() + damageOverTime);
        this.damageOverTime = damageOverTime;
    }

    /// <summary>
    /// Sets the rate increase by which damage over time is applied by this armor to the enemy when an enemy comes into contact with the player
    /// </summary>
    /// <param name="damageOverTimeRateIncrease">Increase in the rate at which damage is applied</param>
    public override void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease)
    {
        var playerDamage = PlayerManager.Player.GetDefensiveDamage();
        playerDamage.SetDamageOverTimeRateIncrease(playerDamage.GetDamageOverTimeRateIncrease() - GetDamageOverTimeRateIncrease() + damageOverTimeRateIncrease);
        this.damageOverTimeRateIncrease = damageOverTimeRateIncrease;
    }
}