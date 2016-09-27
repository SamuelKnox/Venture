using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Bow : Weapon
{
    private RangedWeapon rangedWeapon;

    protected override void Awake()
    {
        base.Awake();
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
            var projectileDamage = projectile.GetDamage();
            projectileDamage.AddDamage(damage);
        }
        return projectile;
    }

    /// <summary>
    /// Validates the roon socket types for this weapon type
    /// </summary>
    protected override void ValidateRoonSocketTypes()
    {
        var roonSocketTypes = GetRoonSocketTypes().ToList();
        if (!roonSocketTypes.Contains(RoonType.Bow))
        {
            Debug.LogError(name + " must contain a Bow Roon Type Socket!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.MeleeWeapon))
        {
            Debug.LogError(name + " cannot have a Melee Weapon Roon Socket Type!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.Wand))
        {
            Debug.LogError(name + " cannot have a Wand Roon Socket Type!", gameObject);
            return;
        }
    }
}