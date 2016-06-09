using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Tooltip("Melee weapon which is currently available for use")]
    [SerializeField]
    private MeleeWeapon activeMeleeWeapon;

    [Tooltip("Bow which is currently available for use")]
    [SerializeField]
    private Bow activeBow;

    [Tooltip("Wand which is currently available for use")]
    [SerializeField]
    private Wand activeWand;

    [Tooltip("All melee weapons the player has obtained")]
    [SerializeField]
    private MeleeWeapon[] meleeWeapons;

    [Tooltip("All bows the player has obtained")]
    [SerializeField]
    private Bow[] bows;

    [Tooltip("All wands the player has obtained")]
    [SerializeField]
    private Wand[] wands;

    /// <summary>
    /// Gets the player's active melee weapon
    /// </summary>
    /// <returns>Active melee weapon</returns>
    public MeleeWeapon GetActiveMeleeWeapon()
    {
        return activeMeleeWeapon;
    }

    /// <summary>
    /// Gets the player's active bow
    /// </summary>
    /// <returns>Active bow</returns>
    public Bow GetActiveBow()
    {
        return activeBow;
    }

    /// <summary>
    /// Gets the player's active wand
    /// </summary>
    /// <returns>Active wand</returns>
    public Wand GetActiveWand()
    {
        return activeWand;
    }
}