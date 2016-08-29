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
        var player = FindObjectOfType<Player>();
        if (player)
        {
            mana = player.GetComponentInChildren<Mana>();
            if (!mana)
            {
                Debug.LogError(gameObject + " could not find Mana!", gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Casts the wand's spell
    /// </summary>
    public void CastSpell()
    {
        bool canAffordManaCost = mana.GetCurrentManaPoints() >= manaCost || allowManaOverflow && mana.GetCurrentManaPoints() > 0;
        bool wandIsReady = rangedWeapon.IsReady();
        if(!canAffordManaCost || !wandIsReady)
        {
            return;
        }
        mana.SetCurrentManaPoints(mana.GetCurrentManaPoints() - manaCost);
        var root = transform.root;
        var direction = Mathf.Sign(root.localScale.x) * root.right;
        var projectile = rangedWeapon.Fire(direction, true);
        if (projectile)
        {
            projectile.tag = transform.root.tag;
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