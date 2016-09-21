using System;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Bow : Weapon
{
    private RangedWeapon rangedWeapon;

    void Awake()
    {
        rangedWeapon = GetComponent<RangedWeapon>();
    }

    /// <summary>
    /// Fires a bow in a direction
    /// </summary>
    /// <param name="direction">Direction to fire the bow</param>
    /// <returns>The Projectile fired, or null if fire failed</returns>
    public Projectile Fire(Vector2 direction)
    {
        var projectile = rangedWeapon.Fire(direction);
        if (projectile)
        {
            projectile.tag = tag;
            var damage = projectile.GetDamage();
            damage.SetDamageOverTime(GetDamageOverTime());
            damage.SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease());
        }
        return projectile;
    }

    /// <summary>
    /// Fires the bow at a target
    /// </summary>
    /// <param name="target">Target to aim at</param>
    /// <returns>The Projectile fired, or null if fire failed</returns>
    public Projectile Fire(Transform target)
    {
        var projectile = rangedWeapon.Fire(target);
        if (projectile)
        {
            projectile.tag = tag;
            var damage = projectile.GetDamage();
            damage.SetDamageOverTime(GetDamageOverTime());
            damage.SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease());
        }
        return projectile;
    }

    /// <summary>
    /// Sets the damage over time dealt by this Bow
    /// </summary>
    /// <param name="damageOverTime">Damage dealt over time</param>
    public override void SetDamageOverTime(float damageOverTime)
    {
        this.damageOverTime = damageOverTime;
    }

    /// <summary>
    /// Sets the rate at which damage over time is dealt
    /// </summary>
    /// <param name="damageOverTimeRateIncrease">Damage over time rate increase</param>
    public override void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease)
    {
        this.damageOverTimeRateIncrease = damageOverTimeRateIncrease;
    }
}