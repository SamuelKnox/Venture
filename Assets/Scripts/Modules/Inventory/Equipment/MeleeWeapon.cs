using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Damage))]
public class MeleeWeapon : Weapon
{
    private Collider2D areaOfEffect;
    private Damage damage;

    void Awake()
    {
        areaOfEffect = GetComponent<Collider2D>();
        damage = GetComponent<Damage>();
    }

    void OnEnable()
    {
        areaOfEffect.isTrigger = true;
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
        areaOfEffect.enabled = true;
    }

    /// <summary>
    /// Ends the weapon swing
    /// </summary>
    public void FinishSwing()
    {
        areaOfEffect.enabled = false;
    }

    /// <summary>
    /// Turns off the damage collider to prevent duplicate damage
    /// </summary>
    private void OnDamageDealt(Health health)
    {
        areaOfEffect.enabled = false;
    }
}