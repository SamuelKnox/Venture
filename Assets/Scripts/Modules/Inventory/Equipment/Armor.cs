using System.Linq;
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
            return;
        }
        var playerDamages = player.GetComponentsInChildren<Damage>().Where(d => !d.GetComponent<MeleeWeapon>());
        if (playerDamages.Count() != 1)
        {
            Debug.LogError(playerDamages.Count() + " non-MeleeWeapon Damage children were found on the player, but there must be exactly one!", player.gameObject);
            return;
        }
        var playerDamage = playerDamages.FirstOrDefault();
        if (!playerDamage)
        {
            Debug.LogError("Could not find Damage on " + player + "!", player.gameObject);
            return;
        }
        float change = damageOverTime - this.damageOverTime;
        this.damageOverTime = damageOverTime;
        playerDamage.SetDamageOverTime(playerDamage.GetDamageOverTime() + change);
    }
}