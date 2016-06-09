using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// Projectiles are specified components with a Rigidbody2D
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damage))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D body2D;

    void Awake()
    {
        body2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        transform.RotateTowardsVelocity();
    }

    void Update()
    {
        if(body2D.gravityScale != 0)
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
        Damage.OnDamageDealt += OnDamageDealt;
    }

    void OnDisable()
    {
        Damage.OnDamageDealt -= OnDamageDealt;
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
    private void OnDamageDealt()
    {
        Destroy(gameObject);
    }
}