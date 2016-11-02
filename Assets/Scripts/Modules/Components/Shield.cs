using CreativeSpore.SmartColliders;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Shield : MonoBehaviour
{
    [Tooltip("How much of the knockback damage is received, where 0 is no knock back, 1 is normal knockback, and 2 is double knockback")]
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float knockBackReceived = 1.0f;

    public void ApplyDamage(Damage damage)
    {
        if (!damage)
        {
            Debug.LogError("Cannot apply null damage to " + gameObject + "!", gameObject);
            return;
        }
        bool harmful = damage.GetKnockBack() > 0;
        bool friendly = TeamUtility.IsFriendly(gameObject, damage.gameObject);
        if (!harmful || friendly)
        {
            return;
        }
        var root = transform.root;
        var orbOfProtection = root.GetComponentInChildren<OrbOfProtection>();
        if (orbOfProtection)
        {
            Destroy(orbOfProtection.gameObject);
            return;
        }
        var knockBackDirection = (transform.position - damage.transform.position).normalized;
        float knockBackForce = damage.GetKnockBack() * knockBackReceived;
        var knockBack = knockBackDirection * knockBackForce;
        var platformCharacterController = root.GetComponent<PlatformCharacterController>();
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
        }
    }
}