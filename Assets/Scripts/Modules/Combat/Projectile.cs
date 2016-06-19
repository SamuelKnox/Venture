using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// Projectiles are specified components with a Rigidbody2D
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Damage))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D body2D;
    private Collider2D areaOfEffect;
    private Damage damage;

    void Awake()
    {
        body2D = GetComponent<Rigidbody2D>();
        areaOfEffect = GetComponent<Collider2D>();
        damage = GetComponent<Damage>();
    }

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(LayerNames.Trigger);
        areaOfEffect.isTrigger = true;
        transform.RotateTowardsVelocity();
    }

    void Update()
    {
        if (body2D.gravityScale != 0)
        {
            transform.RotateTowardsVelocity();
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnEnable()
    {
        damage.OnDamageDealt += OnDamageDealt;
    }

    void OnDisable()
    {
        damage.OnDamageDealt -= OnDamageDealt;
    }

    /// <summary>
    /// Gets the Rigidbody2D for the Projectile
    /// </summary>
    /// <returns>The Projectile's Rigidbody2D</returns>
    public Rigidbody2D GetRigidBody2D()
    {
        return body2D;
    }

    /// <summary>
    /// Destroys projectile after it has finished dealing damage
    /// </summary>
    private void OnDamageDealt(Health health)
    {
        Destroy(gameObject);
    }
}