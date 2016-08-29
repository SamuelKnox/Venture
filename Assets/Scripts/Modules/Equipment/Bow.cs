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
    public void Fire(Vector2 direction)
    {
        var projectile = rangedWeapon.Fire(direction);
        if (projectile)
        {
            projectile.tag = tag;
            var damage = projectile.GetDamage();
            damage.SetDamageOverTime(GetDamageOverTime());
            damage.SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease());
        }
    }

    /// <summary>
    /// Fires the bow at a target
    /// </summary>
    /// <param name="target">Target to aim at</param>
    public void Fire(Transform target)
    {
        var projectile = rangedWeapon.Fire(target);
        if (projectile)
        {
            projectile.tag = tag;
            var damage = projectile.GetDamage();
            damage.SetDamageOverTime(GetDamageOverTime());
            damage.SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease());
        }
    }

    public override void SetDamageOverTime(float damageOverTime)
    {
        this.damageOverTime = damageOverTime;
    }

    public override void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease)
    {
        this.damageOverTimeRateIncrease = damageOverTimeRateIncrease;
    }
}