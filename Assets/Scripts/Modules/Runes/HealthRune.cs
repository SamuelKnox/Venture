using UnityEngine;

public class HealthRune : Rune
{
    [Tooltip("Hit points by which this rune increases max hit points")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float maxHitPointIncrease = 10.0f;

    /// <summary>
    /// Increases the player's max hit points
    /// </summary>
    /// <param name="equipment">Equipment with rune attached</param>
    public override void AttachRune(Equipment equipment)
    {
        AdjustPlayerHitPoints(maxHitPointIncrease);
    }

    /// <summary>
    /// Decreases the player's max hit points
    /// </summary>
    /// <param name="equipment">Equipment with rune attached</param>
    public override void DetachRune(Equipment equipment)
    {
        AdjustPlayerHitPoints(-maxHitPointIncrease);
    }

    /// <summary>
    /// Changes the player's max hit points
    /// </summary>
    /// <param name="change">Amount to change by</param>
    private void AdjustPlayerHitPoints(float change)
    {
        var player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find player!", gameObject);
        }
        var health = player.GetComponent<Health>();
        if (!health)
        {
            Debug.LogError(player + " did not have Health!", player.gameObject);
        }
        health.SetMaxHitPoints(health.GetMaxHitPoints() + change);
    }
}