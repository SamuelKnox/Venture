using CreativeSpore.SmartColliders;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExplodingPlatform : MonoBehaviour
{
    [Tooltip("How long in seconds after player contact until the platform is destroyed")]
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float explosionTime = 1.0f;

    private Collider2D edge;

    void Awake()
    {
        edge = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        var player = collider2D.GetComponent<Player>();
        if (!player)
        {
            return;
        }
        if (edge.bounds.center.y <= collider2D.bounds.min.y)
        {
            Destroy(gameObject, explosionTime);
        }
    }
}