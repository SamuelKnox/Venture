using CreativeSpore.SmartColliders;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : Enemy
{
    [Tooltip("Force applied to ghost to move it in the direction of the player")]
    [SerializeField]
    private Vector2 acceleration = Vector2.one;

    [Tooltip("The maximum speed at which the ghost can move.  Required to keep the ghost from accelerating off screen.")]
    [SerializeField]
    private float maxSpeed = 1.0f;

    private Rigidbody2D body2D;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        body2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        if (!player)
        {
            Debug.LogError(gameObject + " could not find player!");
            return;
        }
    }

    void Start()
    {
        body2D.gravityScale = 0.0f;
    }

    void Update()
    {
        if (stunned)
        {
            return;
        }
        FacePlayer();
    }

    void FixedUpdate()
    {
        if (stunned)
        {
            return;
        }
        MoveTowardsPlayer();
    }

    /// <summary>
    /// Prevents the ghost from moving before death, then destroys it after death
    /// </summary>
    public override void Die()
    {
        base.Die();
        body2D.constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(gameObject);
    }

    /// <summary>
    /// Makes the ghost face the player on its x axis
    /// </summary>
    private void FacePlayer()
    {
        float x = transform.localScale.x;
        float y = transform.localScale.y;
        float z = transform.localScale.z;
        if (transform.position.x < player.transform.position.x)
        {
            x = Mathf.Abs(x);
        }
        else if (transform.position.x > player.transform.position.x)
        {
            x = -Mathf.Abs(x);
        }
        transform.localScale = new Vector3(x, y, z);
    }

    /// <summary>
    /// Makes the ghost fly towards the player
    /// </summary>
    private void MoveTowardsPlayer()
    {
        var direction = (player.transform.position - transform.position).normalized;
        var force = Vector2.Scale(direction, acceleration);
        body2D.AddForce(force);
        body2D.velocity = Vector2.ClampMagnitude(body2D.velocity, maxSpeed);
    }
}