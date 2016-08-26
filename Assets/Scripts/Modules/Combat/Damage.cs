using UnityEngine;

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

    [Tooltip("Whether or not this damage component is active and will actually deal damage")]
    [SerializeField]
    private bool active = true;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!active)
        {
            return;
        }
        var health = other.GetComponent<Health>();
        if (!health)
        {
            return;
        }
        var friendly = TeamUtility.IsFriendly(gameObject, health.gameObject);
        if (!friendly)
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
    /// Gets the amount of damage dealt by this
    /// </summary>
    /// <returns>Damage points</returns>
    public float GetBaseDamage()
    {
        return baseDamage;
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
}