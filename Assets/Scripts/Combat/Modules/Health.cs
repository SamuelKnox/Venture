using CustomUnityLibrary;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Current hit points")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float hitPoints = 100.0f;

    [Tooltip("How resistant it is to a Damage's knockback")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float knockBackResistance = 1.0f;

    /// <summary>
    /// Deals damage to this Health
    /// </summary>
    /// <param name="damage">Damage to deal</param>
    public void TakeDamage(Damage damage)
    {
        if (damage && !TeamUtility.IsFriendly(gameObject, damage.gameObject))
        {
            hitPoints -= damage.GetDamagePoints();
        }
        var knockBackDirection = (transform.position - damage.transform.position).normalized;
        float knockBackForce = damage.GetKnockBack() / knockBackResistance;
        var knockBack = knockBackDirection * knockBackForce;
        var characterPlatformer = GetComponent<CharacterPlatformer>();
        if (characterPlatformer)
        {
            characterPlatformer.AddForce(knockBack);
            return;
        }
        var body = GetComponent<Rigidbody2D>();
        if (body)
        {
            body.AddForce(knockBack * 100);
        }
    }
}