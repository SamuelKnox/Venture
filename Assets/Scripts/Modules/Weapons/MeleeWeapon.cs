using System.Linq;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Tooltip("Handle for when melee weapon is sheathed")]
    [SerializeField]
    private Sprite handle;

    void OnEnable()
    {
        damage.OnDamageDealt += OnDamageDealt;
    }

    void OnDisable()
    {
        damage.OnDamageDealt -= OnDamageDealt;
    }

    /// <summary>
    /// Swings the weapon
    /// </summary>
    public void BeginSwing()
    {
        damage.SetActive(true);
    }

    /// <summary>
    /// Ends the weapon swing
    /// </summary>
    public void FinishSwing()
    {
        damage.SetActive(false);
    }

    /// <summary>
    /// Gets the sprite used to display the weapon's handle when sheathed
    /// </summary>
    /// <returns>Melee Weapon's handle</returns>
    public Sprite GetHandle()
    {
        return handle;
    }

    /// <summary>
    /// Turns off the damage collider to prevent duplicate damage
    /// </summary>
    private void OnDamageDealt(Health health)
    {
        damage.SetActive(false);
    }

    /// <summary>
    /// Validates the roon socket types for this weapon type
    /// </summary>
    protected override void ValidateRoonSocketTypes()
    {
        var roonSocketTypes = GetRoonSocketTypes().ToList();
        if (!roonSocketTypes.Contains(RoonType.MeleeWeapon))
        {
            Debug.LogError(name + " must contain a Melee Weapon Roon Type Socket!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.Bow))
        {
            Debug.LogError(name + " cannot have a Bow Roon Socket Type!", gameObject);
            return;
        }
        if (roonSocketTypes.Contains(RoonType.Wand))
        {
            Debug.LogError(name + " cannot have a Wand Roon Socket Type!", gameObject);
            return;
        }
    }
}