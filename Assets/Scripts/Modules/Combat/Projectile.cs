using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// Projectiles are specified components with a Rigidbody2D
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damage))]
public class Projectile : MonoBehaviour
{
    [Tooltip("How long until the projectile is automatically destroyed, where 0 means it will never be automatically destroyed")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float lifeSpan = 5.0f;

    [Tooltip("Whether or not to have the projectile be allowed to be destroyed while visible")]
    [SerializeField]
    private bool destroyedWhileVisible = false;

    private Rigidbody2D body2D;
    private Collider2D areaOfEffect;
    private Damage damage;
    bool visible;

    void Awake()
    {
        body2D = GetComponent<Rigidbody2D>();
        damage = GetComponent<Damage>();
    }

    void Start()
    {
        if(body2D.constraints != RigidbodyConstraints2D.FreezeRotation)
        {
            transform.RotateTowardsVelocity();
        }
    }

    void Update()
    {
        if (body2D.gravityScale != 0 && body2D.constraints != RigidbodyConstraints2D.FreezeRotation)
        {
            transform.RotateTowardsVelocity();
        }
        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0.0f && (!visible || destroyedWhileVisible))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameVisible()
    {
        visible = true;
    }

    void OnBecameInvisible()
    {
        visible = false;
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
    /// Gets the damage applied by this projectile
    /// </summary>
    /// <returns>Projectile Damage</returns>
    public Damage GetDamage()
    {
        return damage;
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