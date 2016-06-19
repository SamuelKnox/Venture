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
        projectile.tag = tag;
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
        }
    }
}