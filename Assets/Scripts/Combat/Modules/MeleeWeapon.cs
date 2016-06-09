using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Damage))]
public class MeleeWeapon : MonoBehaviour
{
    private Collider2D areaOfEffect;

    void Awake()
    {
        areaOfEffect = GetComponent<Collider2D>();
        areaOfEffect.enabled = false;
    }

    void OnEnable()
    {
        Damage.OnDamageDealt += OnDamageDealt;
    }

    void OnDisable()
    {
        Damage.OnDamageDealt -= OnDamageDealt;
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
    private void OnDamageDealt()
    {
        areaOfEffect.enabled = false;
    }
}