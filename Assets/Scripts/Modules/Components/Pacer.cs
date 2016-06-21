using CustomUnityLibrary;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pacer : MonoBehaviour
{
    private const float EdgeHeightCheck = 1.0f;

    [Tooltip("Distance from edge until warned that the edge is reached")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float range = 1.0f;

    [Tooltip("Speed at which the Pacer moves")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float speed = 5.0f;

    private Vector2 extents;
    private Rigidbody2D body2D;
    private Vector2 cornerOffset;
    private bool active = true;

    void Awake()
    {
        SetExtents();
        body2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        cornerOffset = extents + new Vector2(range + FloatUtility.MaxInaccuracy, FloatUtility.MaxInaccuracy);
    }

    void FixedUpdate()
    {
        if (active)
        {
            body2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * speed, body2D.velocity.y);
            SearchForEdges();
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
    /// Casts rays on the edges to see if there is no more area to walk on the left or right, adn changes directions if an edge is reached
    /// </summary>
    private void SearchForEdges()
    {
        var layerMask = LayerMask.GetMask(LayerNames.Collider, LayerNames.TriggerAndCollider, LayerNames.OneWayPlatform);
        var origin = (Vector2)transform.position - cornerOffset;
        var leftRaycastHit2D = Physics2D.Raycast(origin, -transform.up, EdgeHeightCheck, layerMask);
        bool leftEdgeReached = !leftRaycastHit2D.collider;
        origin.x += cornerOffset.x * 2.0f;
        var rightRaycastHit2D = Physics2D.Raycast(origin, -transform.up, EdgeHeightCheck, layerMask);
        bool rightEdgeReached = !rightRaycastHit2D.collider;
        if (leftEdgeReached && rightEdgeReached)
        {
            return;
        }
        if (leftEdgeReached)
        {
            FlipX(false);
        }
        if (rightEdgeReached)
        {
            FlipX(true);
        }
    }

    /// <summary>
    /// Flips the scale so that the pacer faces the direction they are pacing
    /// </summary>
    /// <param name="flipped">Whether or not the flip the scale</param>
    private void FlipX(bool flipped)
    {
        float xScale = transform.localScale.x;
        float yScale = transform.localScale.y;
        float zScale = transform.localScale.z;
        xScale = Mathf.Abs(xScale);
        if (flipped)
        {
            xScale *= -1.0f;
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