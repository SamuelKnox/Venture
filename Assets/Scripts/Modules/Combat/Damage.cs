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

    void OnTriggerEnter2D(Collider2D other)
    {
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
}