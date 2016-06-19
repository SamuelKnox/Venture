using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Wand : Weapon
{
    private RangedWeapon rangedWeapon;

    void Awake()
    {
        rangedWeapon = GetComponent<RangedWeapon>();
    }

    /// <summary>
    /// Casts the wand's spell
    /// </summary>
    public void CastSpell()
    {
        var root = transform.root;
        var direction = Mathf.Sign(root.localScale.x) * root.right;
        var projectile = rangedWeapon.Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
        }
    }
}