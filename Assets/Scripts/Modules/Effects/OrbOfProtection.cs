using UnityEngine;

public class OrbOfProtection : MonoBehaviour
{
    private const string OrbOfProtectionGameObjectName = "Orb of Protection";
    private const float SpriteSizeMultiplier = 2.0f;

    private Collider2D healthCollider;

    void Awake()
    {
        var root = transform.root;
        var health = root.GetComponentInChildren<Health>();
        if (!health)
        {
            Debug.LogError("Could not find health in parent!", gameObject);
            return;
        }
        healthCollider = health.GetComponent<Collider2D>();
        if (!healthCollider)
        {
            Debug.LogError("Could not find health collider!", transform.parent ? transform.parent.gameObject : gameObject);
            return;
        }
        float xScale = healthCollider.bounds.size.x * SpriteSizeMultiplier;
        float yScale = healthCollider.bounds.size.y * SpriteSizeMultiplier;
        var healthColliderParent = healthCollider.transform.parent;
        if (healthColliderParent)
        {
            xScale /= healthColliderParent.localScale.x;
            yScale /= healthColliderParent.localScale.y;
        }
        transform.localScale = new Vector2(xScale, yScale);
    }
}