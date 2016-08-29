using UnityEngine;

public class HealthPotion : Consumable
{
    [Tooltip("Amount of health to heal")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float healthRestore = 10;

    /// <summary>
    /// Uses the potion
    /// </summary>
    public override void Consume()
    {
        HealPlayer();
    }

    /// <summary>
    /// Gets the potions health restore
    /// </summary>
    /// <returns>Health restore</returns>
    public float GetHealthRestore()
    {
        return healthRestore;
    }

    /// <summary>
    /// Sets the potion's health restore
    /// </summary>
    /// <param name="healthRestore">New health restore</param>
    public void SetHealthRestore(float healthRestore)
    {
        this.healthRestore = healthRestore;
    }

    /// <summary>
    /// Heals the player
    /// </summary>
    private void HealPlayer()
    {
        var player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find Player!", gameObject);
            return;
        }
        var playerHealth = player.GetComponentInChildren<Health>();
        if (!playerHealth)
        {
            Debug.LogError(gameObject + " could not find player health!", gameObject);
            return;
        }
        playerHealth.SetCurrentHitPoints(playerHealth.GetCurrentHitPoints() + healthRestore);
    }
}