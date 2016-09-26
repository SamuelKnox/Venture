using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Wand : Weapon
{
    [Tooltip("Cost in mana to cast a projectile")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float manaCost = 10.0f;

    [Tooltip("Allow for casting when mana is greater than 0, but is not enough to cover the cost")]
    [SerializeField]
    private bool allowManaOverflow = true;

    private RangedWeapon rangedWeapon;
    private Mana mana;

    void Awake()
    {
        rangedWeapon = GetComponent<RangedWeapon>();
        mana = PlayerManager.Player.GetComponentInChildren<Mana>();
        if (!mana)
        {
            Debug.LogError(gameObject + " could not find Mana!", gameObject);
            return;
        }
    }

    /// <summary>
    /// Casts the wand's projectile
    /// </summary>
    /// <param name="direction">Direction to cast the projectile</param>
    /// <returns>The Projectile cast, or null if spell cast failed</returns>
    public Projectile CastProjectile(Vector2 direction)
    {
        bool canAffordManaCost = mana.GetCurrentManaPoints() >= manaCost || allowManaOverflow && mana.GetCurrentManaPoints() > 0;
        bool wandIsReady = rangedWeapon.IsReady();
        if (!canAffordManaCost || !wandIsReady)
        {
            return null;
        }
        mana.SetCurrentManaPoints(mana.GetCurrentManaPoints() - manaCost);
        var projectile = rangedWeapon.Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
            var projectileDamage = projectile.GetDamage();
            projectileDamage.MergeDamage(damage);
        }
        return projectile;
    }

    /// <summary>
    /// Casts the wand's spell, based on its equipped roons
    /// </summary>
    public void CastSpell()
    {
        var wandRoon = GetRoon(RoonType.Wand) as WandRoon;
        bool canAffordManaCost = mana.GetCurrentManaPoints() >= wandRoon.GetManaCost() || allowManaOverflow && mana.GetCurrentManaPoints() > 0;
        bool wandIsReady = rangedWeapon.IsReady();
        if (!canAffordManaCost || !wandIsReady)
        {
            return;
        }
        mana.SetCurrentManaPoints(mana.GetCurrentManaPoints() - wandRoon.GetManaCost());
        wandRoon.ActivateAbility(this);
    }

    /// <summary>
    /// Validates the roon socket types for this weapon type
    /// </summary>
    protected override void ValidateRoonSocketTypes()
    {
        var roonSocketTypes = GetRoonSocketTypes().ToList();
        if (!roonSocketTypes.Contains(RoonType.Wand))
        {
            Debug.LogError(name + " must contain a Wand Roon Type Socket!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.MeleeWeapon))
        {
            Debug.LogError(name + " cannot have a Melee Weapon Roon Socket Type!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.Bow))
        {
            Debug.LogError(name + " cannot have a Bow Roon Socket Type!", gameObject);
            return;
        }
    }
}