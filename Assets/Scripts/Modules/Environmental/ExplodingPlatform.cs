using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ExplodingPlatformView))]
public class ExplodingPlatform : MonoBehaviour
{
    [Tooltip("How long in seconds after player contact until the platform is destroyed")]
    [SerializeField]
    [Range(0.0f, 5.0f)]
    private float explosionTime = 1.0f;

    private Collider2D edge;
    private ExplodingPlatformView explodingPlatformView;

    void Awake()
    {
        edge = GetComponent<Collider2D>();
        explodingPlatformView = GetComponent<ExplodingPlatformView>();
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
            explodingPlatformView.Explode();
            Destroy(gameObject, explosionTime);
        }
    }
}