using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Springer : Enemy
{
    [Tooltip("Impulse applied to spring when player is in sight.  A negative x value will make the springer run away from the player.")]
    [SerializeField]
    private Vector2 spring = new Vector2(1, 2);

    private bool visible;
    private Rigidbody2D body;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        if (stunned)
        {
            return;
        }
        if (visible && body.velocity == Vector2.zero)
        {
            var force = spring;
            if (player.transform.position.x < transform.position.x)
            {
                force.x = -Mathf.Abs(spring.x);
            }
            else if (player.transform.position.x == transform.position.x)
            {
                force.x = 0.0f;
            }
            body.AddForce(force, ForceMode2D.Impulse);
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
}