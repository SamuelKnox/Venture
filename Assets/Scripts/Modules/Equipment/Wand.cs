using System;
using UnityEngine;

[RequireComponent(typeof(RangedWeapon))]
public class Wand : Weapon
{
    [Tooltip("Cost in mana to cast a spell")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float manaCost = 1.0f;

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
    /// Casts the wand's spell
    /// </summary>
    /// <param name="direction">Direction to cast the spell</param>
    /// <returns>The Projectile cast, or null if spell cast failed</returns>
    public Projectile CastSpell(Vector2 direction)
    {
        bool canAffordManaCost = mana.GetCurrentManaPoints() >= manaCost || allowManaOverflow && mana.GetCurrentManaPoints() > 0;
        bool wandIsReady = rangedWeapon.IsReady();
        if(!canAffordManaCost || !wandIsReady)
        {
            return null;
        }
        mana.SetCurrentManaPoints(mana.GetCurrentManaPoints() - manaCost);
        var projectile = rangedWeapon.Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
            var damage = projectile.GetDamage();
            damage.SetDamageOverTime(GetDamageOverTime());
            damage.SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease());
        }
        return projectile;
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