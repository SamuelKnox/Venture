using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Damage : MonoBehaviour
{
    /// <summary>
    /// Delegate for dealing damage
    /// </summary>
    public delegate void DamageDealt();

    /// <summary>
    /// Event called when damage is dealt
    /// </summary>
    public event DamageDealt OnDamageDealt;

    [Tooltip("Damage dealt by this")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float damagePoints = 10.0f;

    [Tooltip("How much the damage knocks back")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float knockBack = 2.0f;

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
            health.TakeDamage(this);
            if (OnDamageDealt != null)
            {
                OnDamageDealt();
            }
        }
    }

    /// <summary>
    /// Gets the amount of damage dealt by this
    /// </summary>
    /// <returns>Damage points</returns>
    public float GetDamagePoints()
    {
        return damagePoints;
    }

    /// <summary>
    /// Gets the amount of knock back done by this Damage
    /// </summary>
    /// <returns>Knock back force</returns>
    public float GetKnockBack()
    {
        return knockBack;
    }
}