using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pacer : MonoBehaviour
{
    private const float CheckDistance = 1.0f;

    [Tooltip("Layers to check before turning around")]
    [SerializeField]
    private LayerMask turnAroundLayers;

    [Tooltip("Number of rays to use when checking for walls")]
    [SerializeField]
    [Range(0, 10)]
    private int horizontalRays = 3;

    [Tooltip("Distance from edge until warned that the edge is reached")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float range = 1.0f;

    [Tooltip("Speed at which the Pacer moves")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float speed = 4.0f;

    private Vector2 extents;
    private Rigidbody2D body2D;
    private bool active = true;

    void Awake()
    {
        SetExtents();
        body2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (active)
        {
            body2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed, body2D.velocity.y);
            SearchForWalls();
            SearchForLedges();
        }
    }

    /// <summary>
    /// Sets whether or not pacing should occur
    /// </summary>
    /// <param name="active">Whether or not to pace</param>
    public void SetPacingActive(bool active)
    {
        this.active = active;
        if (this.active)
        {
            body2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed, body2D.velocity.y);
        }
        else
        {
            body2D.velocity = new Vector2(0.0f, body2D.velocity.y);
        }
    }

    /// <summary>
    /// Casts rays on the edges to see if there is a wall in the way
    /// </summary>
    private void SearchForWalls()
    {
        if (horizontalRays == 0)
        {
            return;
        }
        var origin = (Vector2)transform.position - extents - new Vector2(range, 0.0f);
        bool leftEdgeReached = false;
        bool rightEdgeReached = false;
        for (int i = 0; i < horizontalRays; i++)
        {
            origin.y = transform.position.y - extents.y + ((float)i / (horizontalRays - 1)) * extents.y * 2.0f;
            var leftRaycastHit2D = Physics2D.Raycast(origin, -transform.right, CheckDistance, turnAroundLayers);
            if (leftRaycastHit2D.collider)
            {
                leftEdgeReached = true;
                break;
            }
        }
        origin.x += (extents.x + range) * 2.0f;
        for (int i = 0; i < horizontalRays; i++)
        {
            origin.y = transform.position.y - extents.y + ((float)i / (horizontalRays - 1)) * extents.y * 2.0f;
            var rightRaycastHit2D = Physics2D.Raycast(origin, transform.right, CheckDistance, turnAroundLayers);
            if (rightRaycastHit2D.collider)
            {
                rightEdgeReached = true;
                break;
            }
        }
        AdjustFacing(leftEdgeReached, rightEdgeReached);
    }

    /// <summary>
    /// Casts rays on the edges to see if there is no more area to walk on the left or right, and changes directions if an edge is reached
    /// </summary>
    private void SearchForLedges()
    {
        var origin = (Vector2)transform.position - extents - new Vector2(range, 0.0f);
        var leftRaycastHit2D = Physics2D.Raycast(origin, -transform.up, CheckDistance, turnAroundLayers);
        bool leftEdgeReached = !leftRaycastHit2D.collider;
        origin.x += (extents.x + range) * 2.0f;
        var rightRaycastHit2D = Physics2D.Raycast(origin, -transform.up, CheckDistance, turnAroundLayers);
        bool rightEdgeReached = !rightRaycastHit2D.collider;
        AdjustFacing(leftEdgeReached, rightEdgeReached);
    }

    /// <summary>
    /// Turns the player around
    /// </summary>
    /// <param name="leftEdgeReached">Whether or not the left edge was reached</param>
    /// <param name="rightEdgeReached">Whether or not the right edge was reached</param>
    private void AdjustFacing(bool leftEdgeReached, bool rightEdgeReached)
    {
        if (leftEdgeReached && rightEdgeReached)
        {
            return;
        }
        float xScale = transform.localScale.x;
        float yScale = transform.localScale.y;
        float zScale = transform.localScale.z;
        float absoluteXScale = Mathf.Abs(xScale);
        if (leftEdgeReached)
        {
            xScale = absoluteXScale;
        }
        if (rightEdgeReached)
        {
            xScale = -absoluteXScale;
        }
        transform.localScale = new Vector3(xScale, yScale, zScale);
    }

    /// <summary>
    /// Sets the extents used for pacing, prioritizing colliders, then triggers, then sprite renderers
    /// </summary>
    private void SetExtents()
    {
        var collisionArea = GetComponents<Collider2D>().Where(c => !c.isTrigger).FirstOrDefault();
        if (!collisionArea)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                extents = spriteRenderer.bounds.extents;
                return;
            }
            collisionArea = GetComponents<Collider2D>().Where(c => c.isTrigger).FirstOrDefault();
        }
        if (collisionArea)
        {
            extents = collisionArea.bounds.extents;
            return;
        }
        Debug.LogError("A Collider2D or Sprite Renderer is required for a Pacer!", gameObject);
        return;
    }
}