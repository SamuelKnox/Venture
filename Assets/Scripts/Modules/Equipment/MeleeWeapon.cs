using System;
using UnityEngine;

[RequireComponent(typeof(Damage))]
public class MeleeWeapon : Weapon
{
    private Damage damage;

    void Awake()
    {
        damage = GetComponent<Damage>();
    }

    void Start()
    {
        damage.SetActive(false);
    }

    void OnEnable()
    {
        damage.OnDamageDealt += OnDamageDealt;
    }

    void OnDisable()
    {
        damage.OnDamageDealt -= OnDamageDealt;
    }

    /// <summary>
    /// Swings the weapon
    /// </summary>
    public void BeginSwing()
    {
        damage.SetActive(true);
    }

    /// <summary>
    /// Ends the weapon swing
    /// </summary>
    public void FinishSwing()
    {
        damage.SetActive(false);
    }

    /// <summary>
    /// Turns off the damage collider to prevent duplicate damage
    /// </summary>
    private void OnDamageDealt(Health health)
    {
        damage.SetActive(false);
    }

    public override void SetDamageOverTime(float damageOverTime)
    {
        damage.SetDamageOverTime(damageOverTime);
        this.damageOverTime = damageOverTime;
    }

    public override void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease)
    {
        damage.SetDamageOverTimeRateIncrease(damageOverTimeRateIncrease);
        this.damageOverTimeRateIncrease = damageOverTimeRateIncrease;
    }
}