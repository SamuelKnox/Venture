using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Smasher : Enemy
{
    [Tooltip("Direction smasher moves when the player is detected within its vision")]
    [SerializeField]
    private Direction attackDirection;

    [Tooltip("Force applied when the player enters the smasher's vision")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float attackForce = 7.5f;

    [Tooltip("Force applied when the smaher is returning to its starting position")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float returnForce = 2.5f;

    [Tooltip("Whether or not the smasher will attack before returning to its original position when the player is in the smasher's vision")]
    [SerializeField]
    private bool attackOnSight = false;

    private Rigidbody2D body;
    private Vector2 startingPosition;
    private Vector2 attackVelocity;
    private Vector2 returnVelocity;
    private bool attacking = false;
    private bool returning = false;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        SetUpBody();
        startingPosition = transform.position;
        InitializeVelocities();
    }

    void Update()
    {
        if (attacking && body.velocity == Vector2.zero)
        {
            body.velocity = returnVelocity;
            attacking = false;
            returning = true;
        }
        if (returning)
        {
            StopAfterReturning();
        }
    }

    void OnVisionStay(Collider2D collider)
    {
        if (collider.GetComponent<Player>() && (!attacking && !returning || attackOnSight))
        {
            Debug.Log(attackVelocity);
            body.velocity = attackVelocity;
            StartCoroutine(EnableAttacking());
        }
    }

    /// <summary>
    /// Sets the smasher's attacking boolean to true after the physics frame is complete
    /// </summary>
    /// <returns>Coroutine required IEnumerator</returns>
    private IEnumerator EnableAttacking()
    {
        yield return new WaitForEndOfFrame();
        attacking = true;
    }

    /// <summary>
    /// Smasher stops after it has reached its starting location when returning
    /// </summary>
    private void StopAfterReturning()
    {
        bool startingPositionReached = false;
        if (body.velocity == Vector2.zero)
        {
            startingPositionReached = true;
        }
        else
        {
            switch (attackDirection)
            {
                case Direction.North:
                    startingPositionReached = transform.position.y <= startingPosition.y;
                    break;
                case Direction.South:
                    startingPositionReached = transform.position.y >= startingPosition.y;
                    break;
                case Direction.East:
                    startingPositionReached = transform.position.x <= startingPosition.x;
                    break;
                case Direction.West:
                    startingPositionReached = transform.position.x >= startingPosition.x;
                    break;
                default:
                    Debug.LogError("Invalid direction (" + attackDirection + ") was provided!", gameObject);
                    return;
            }
        }
        if (startingPositionReached)
        {
            body.velocity = Vector2.zero;
            returning = false;
        }
    }

    /// <summary>
    /// Sets up the attributes of the Rigidbody2D
    /// </summary>
    private void SetUpBody()
    {
        body.gravityScale = 0.0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (attackDirection == Direction.North || attackDirection == Direction.South)
        {
            body.constraints |= RigidbodyConstraints2D.FreezePositionX;
        }
        else if (attackDirection == Direction.West || attackDirection == Direction.East)
        {
            body.constraints |= RigidbodyConstraints2D.FreezePositionY;
        }
    }

    /// <summary>
    /// Sets up the attack and return velocities
    /// </summary>
    private void InitializeVelocities()
    {
        switch (attackDirection)
        {
            case Direction.North:
                attackVelocity = Vector2.up * attackForce;
                returnVelocity = Vector2.down * returnForce;
                break;
            case Direction.South:
                attackVelocity = Vector2.down * attackForce;
                returnVelocity = Vector2.up * returnForce;
                break;
            case Direction.East:
                attackVelocity = Vector2.right * attackForce;
                returnVelocity = Vector2.left * returnForce;
                break;
            case Direction.West:
                attackVelocity = Vector2.left * attackForce;
                returnVelocity = Vector2.right * returnForce;
                break;
            default:
                Debug.LogError("Invalid direction (" + attackDirection + ") was provided!", gameObject);
                return;
        }
    }
}