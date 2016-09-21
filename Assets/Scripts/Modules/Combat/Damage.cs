﻿using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Damage : MonoBehaviour
{
    /// <summary>
    /// Delegate for dealing damage
    /// </summary>
    public delegate void DamageDealt(Health health);

    /// <summary>
    /// Event called when damage is dealt
    /// </summary>
    public event DamageDealt OnDamageDealt;

    [Tooltip("Base Damage dealt by this")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float baseDamage = 10.0f;

    [Tooltip("How much the damage knocks back")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float knockBack = 250.0f;

    [Tooltip("Amount of damage over time applied by this damage")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float damageOverTime = 0.0f;

    [Tooltip("Amount of damage over time applied by this damage")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float damageOverTimeRateIncrease = 0.0f;

    [Tooltip("Stun duration dealt by this damage")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float stun;

    [Tooltip("Whether or not this damage component is active and will actually deal damage")]
    [SerializeField]
    private bool active = true;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!active)
        {
            return;
        }
        bool friendly = TeamUtility.IsFriendly(gameObject, other.gameObject);
        if (friendly)
        {
            return;
        }
        var shield = other.GetComponent<Shield>();
        if (shield)
        {
            shield.ApplyDamage(this);
            var shieldedHealth = other.transform.root.GetComponentInChildren<Health>();
            if (shieldedHealth)
            {
                shieldedHealth.ResetInvincibilityTime();
            }
        }
        var health = other.GetComponent<Health>();
        if (health)
        {
            health.ApplyDamage(this);
            if (OnDamageDealt != null)
            {
                OnDamageDealt(health);
            }
        }
    }

    /// <summary>
    /// Sets whether or not this damage component is allowed to deal damage
    /// </summary>
    /// <param name="active">Can deal damage</param>
    public void SetActive(bool active)
    {
        this.active = active;
    }

    /// <summary>
    /// Modifies the effectiveness of this damage by a percent, where 0.5 is half effectiveness, 2.0 is double effectiveness, and 1.0f is normal effectiveness.
    /// </summary>
    /// <param name="percent">Percent to modify damage by</param>
    public void Modify(float percent)
    {
        SetBaseDamage(GetBaseDamage() * percent);
        SetKnockBack(GetKnockBack() * percent);
        SetDamageOverTime(GetDamageOverTime() * percent);
        SetDamageOverTimeRateIncrease(GetDamageOverTimeRateIncrease() * percent);
        SetStun(GetStun() * percent);
    }

    /// <summary>
    /// Gets the amount of damage dealt
    /// </summary>
    /// <returns>Damage points</returns>
    public float GetBaseDamage()
    {
        return baseDamage;
    }

    /// <summary>
    /// Sets the amount of damage dealt
    /// </summary>
    /// <param name="baseDamage">Damage points</param>
    public void SetBaseDamage(float baseDamage)
    {
        this.baseDamage = baseDamage;
    }

    /// <summary>
    /// Gets the amount of knock back done by this Damage
    /// </summary>
    /// <returns>Knock back force</returns>
    public float GetKnockBack()
    {
        return knockBack;
    }

    /// <summary>
    /// Sets the amount of knock back done by this Damage
    /// </summary>
    /// <param name="knockBack">Knock back force</param>
    public void SetKnockBack(float knockBack)
    {
        this.knockBack = knockBack;
    }

    /// <summary>
    /// Gets the damage over time applied by this damage
    /// </summary>
    /// <returns>Damage over time</returns>
    public float GetDamageOverTime()
    {
        return damageOverTime;
    }

    /// <summary>
    /// Sets the damage over time
    /// </summary>
    /// <param name="damage">Damage over time</param>
    public void SetDamageOverTime(float damage)
    {
        damageOverTime = damage;
    }

    /// <summary>
    /// Rate at which the damage over time from this damage is applied
    /// </summary>
    /// <returns>Damage over time Rate</returns>
    public float GetDamageOverTimeRateIncrease()
    {
        return damageOverTimeRateIncrease;
    }

    /// <summary>
    /// Sets the amount by which this damage over time rate is increased
    /// </summary>
    /// <param name="damageOverTimeRateIncrease">Damge over time rate increase</param>
    public void SetDamageOverTimeRateIncrease(float damageOverTimeRateIncrease)
    {
        this.damageOverTimeRateIncrease = damageOverTimeRateIncrease;
    }

    /// <summary>
    /// Stun dealt by this damage
    /// </summary>
    /// <returns>Stun duration in seconds</returns>
    public float GetStun()
    {
        return stun;
    }

    /// <summary>
    /// Sets the stun dealt by this damage
    /// </summary>
    /// <param name="stun">Stun duration in seconds</param>
    public void SetStun(float stun)
    {
        this.stun = stun;
    }
}