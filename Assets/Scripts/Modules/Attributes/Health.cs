using CreativeSpore.SmartColliders;
using UnityEngine;

public class Health : MonoBehaviour
{
    /// <summary>
    /// Delegate for taking damage
    /// </summary>
    public delegate void DamageDealt(Damage damage);

    /// <summary>
    /// Event called when damage is received
    /// </summary>
    public event DamageDealt OnDamageDealt;

    [Tooltip("Whether or not this unit is invincible.  If invincible, Damage will not affect this.")]
    [SerializeField]
    private bool invincible = false;

    [Tooltip("Current hit points")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float currentHitPoints = 100.0f;

    [Tooltip("Max hit points")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float maxHitPoints = 100.0f;

    [Tooltip("Damage over time left to be applied")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float damageOverTime = 0.0f;

    [Tooltip("How fast damage over time is distributed")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float damageOverTimeRate = 1.0f;

    [Tooltip("How long invincible after receiving damage.  Damage over time does not reset the timer.")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float invincibilityCooldown = 1.0f;

    [Tooltip("Optional health view, used to display the health")]
    [SerializeField]
    private HealthView healthView;

    [Tooltip("How much of the knockback damage is received, where 0 is no knock back, 1 is normal knockback, and 2 is double knockback")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float knockBackReceived = 1.0f;

    private float totalInvincibilityCooldown;

    void Awake()
    {
        SetInvincibilityTime(invincibilityCooldown);
    }

    void Update()
    {
        ApplyDamageOverTime();
        UpdateCooldownTimer();
    }

    void OnValidate()
    {
        currentHitPoints = Mathf.Min(currentHitPoints, maxHitPoints);
    }

    /// <summary>
    /// Checks whether or not this GameObject is dead
    /// </summary>
    /// <returns>Whether or not dead</returns>
    public bool IsDead()
    {
        return currentHitPoints <= 0;
    }

    /// <summary>
    /// Gets the current hit points
    /// </summary>
    /// <returns>Current hit points</returns>
    public float GetCurrentHitPoints()
    {
        return currentHitPoints;
    }

    /// <summary>
    /// Sets the current hitpoint for this health component
    /// </summary>
    /// <param name="hitPoints">Hit points to set to</param>
    public void SetCurrentHitPoints(float hitPoints)
    {
        currentHitPoints = Mathf.Clamp(hitPoints, 0, GetMaxHitPoints());
        if (OnDamageDealt != null)
        {
            OnDamageDealt(null);
        }
        if (healthView)
        {
            healthView.AdjustHealth(currentHitPoints / maxHitPoints);
        }
    }

    /// <summary>
    /// Gets the max hit points
    /// </summary>
    /// <returns>Max hit points</returns>
    public float GetMaxHitPoints()
    {
        return maxHitPoints;
    }

    /// <summary>
    /// Sets the max hit points
    /// </summary>
    /// <param name="maxHitPoints">Max hit points to use</param>
    public void SetMaxHitPoints(float maxHitPoints)
    {
        this.maxHitPoints = maxHitPoints;
    }

    /// <summary>
    /// Checks whether or not this Health component can take Damage
    /// </summary>
    /// <returns>Whether or not invincible</returns>
    public bool IsInvincible()
    {
        return invincible;
    }

    /// <summary>
    /// Sets whether or not this Health component can take Damage
    /// </summary>
    /// <param name="invincible">Whether or not invincible</param>
    public void SetInvincible(bool invincible)
    {
        this.invincible = invincible;
    }

    /// <summary>
    /// Gets the duration of invincibility after taking damage
    /// </summary>
    /// <returns>Seconds</returns>
    public float GetInvincibilityTime()
    {
        return invincibilityCooldown;
    }

    /// <summary>
    /// Resets the invincibility time to full invincibility
    /// </summary>
    public void ResetInvincibilityTime()
    {
        SetInvincibilityTime(totalInvincibilityCooldown);
    }

    /// <summary>
    /// Sets the duration of invincibility after taking damage
    /// </summary>
    /// <param name="time">Seconds</param>
    public void SetInvincibilityTime(float time)
    {
        invincibilityCooldown = time;
        totalInvincibilityCooldown = invincibilityCooldown;
    }

    /// <summary>
    /// Deals damage to this Health
    /// </summary>
    /// <param name="damage">Damage to deal</param>
    public void ApplyDamage(Damage damage)
    {
        if (invincible)
        {
            return;
        }
        if (!damage)
        {
            Debug.LogError("Cannot apply null damage to " + gameObject + "!", gameObject);
            return;
        }
        bool harmful = damage.GetBaseDamage() > 0 || damage.GetDamageOverTime() > 0 || damage.GetKnockBack() > 0 || (damage.GetSpeedModifierIntensity() > 0 && damage.GetSpeedModifierDuration() > 0);
        bool friendly = TeamUtility.IsFriendly(gameObject, damage.gameObject);
        bool coolingDown = invincibilityCooldown > 0;
        if (!harmful || friendly || coolingDown)
        {
            return;
        }
        var root = transform.root;
        invincibilityCooldown = totalInvincibilityCooldown;
        var orbOfProtection = root.GetComponentInChildren<OrbOfProtection>();
        if (orbOfProtection)
        {
            Destroy(orbOfProtection.gameObject);
            return;
        }
        currentHitPoints -= damage.GetBaseDamage();
        damageOverTime += damage.GetDamageOverTime();
        damageOverTimeRate += damage.GetDamageOverTimeRateIncrease();
        var knockBackDirection = (transform.position - damage.transform.position).normalized;
        float knockBackForce = damage.GetKnockBack() * knockBackReceived;
        var knockBack = knockBackDirection * knockBackForce;
        var platformCharacterController = root.GetComponentInChildren<PlatformCharacterController>();
        if (platformCharacterController)
        {
            platformCharacterController.PlatformCharacterPhysics.Velocity = Vector3.zero;
            platformCharacterController.PlatformCharacterPhysics.AddAcceleration(knockBack);
        }
        else
        {
            var body = root.GetComponent<Rigidbody2D>();
            if (body)
            {
                body.AddForce(knockBack);
            }
            if (OnDamageDealt != null)
            {
                OnDamageDealt(damage);
            }
            if (healthView)
            {
                healthView.AdjustHealth(currentHitPoints / maxHitPoints);
            }
        }
        var character = root.GetComponentInChildren<Character>();
        if (!character)
        {
            Debug.LogError("Could not find Character associated with " + this, gameObject);
            return;
        }
        character.AddSpeedModifer(damage.GetSpeedModifierIntensity(), damage.GetSpeedModifierDuration());
        character.AddTintModifier(damage.GetTint(), GetInvincibilityTime());
    }

    /// <summary>
    /// Updates the time remaining for invincibility
    /// </summary>
    private void UpdateCooldownTimer()
    {
        invincibilityCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Applies the damage over time
    /// </summary>
    private void ApplyDamageOverTime()
    {
        if (damageOverTime > 0)
        {
            float damage = Mathf.Min(Time.deltaTime * damageOverTimeRate, damageOverTime);
            damageOverTime -= damage;
            currentHitPoints -= damage;
            if (healthView)
            {
                healthView.AdjustHealth(currentHitPoints / maxHitPoints);
            }
            if (IsDead() && OnDamageDealt != null)
            {
                OnDamageDealt(null);
            }
        }
    }
}