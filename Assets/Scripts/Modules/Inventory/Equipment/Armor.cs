using UnityEngine;

public abstract class Armor : MonoBehaviour
{
    [Tooltip("Damage over time applied on contact with this armor")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float damageOverTime = 0.0f;

    /// <summary>
    /// Gets the damage over time applied by this armor
    /// </summary>
    /// <returns>Damage over time</returns>
    public float GetDamageOverTime()
    {
        return damageOverTime;
    }

    /// <summary>
    /// Adjusts the player's damage over time defense based on this armor
    /// </summary>
    /// <param name="damageOverTime">Damage over time to apply</param>
    public void SetDamageOverTime(float damageOverTime)
    {
        var player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError("Could not find player!", gameObject);
        }
        var playerDamage = player.GetDamage();
        if (!playerDamage)
        {
            Debug.LogError("Could not find Damage on " + player + "!", player.gameObject);
        }
        float change = damageOverTime - this.damageOverTime;
        this.damageOverTime = damageOverTime;
        playerDamage.SetDamageOverTime(playerDamage.GetDamageOverTime() + change);
    }
}