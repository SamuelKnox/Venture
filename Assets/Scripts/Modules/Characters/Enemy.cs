using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Enemy : Character
{
    protected Animator animator;
    private Rigidbody2D body2d;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (body2d)
        {
            animator.SetFloat(AnimationNames.Enemy.Floats.HorizontalSpeed, Mathf.Abs(body2d.velocity.x));
            animator.SetFloat(AnimationNames.Enemy.Floats.VerticalSpeed, body2d.velocity.y);
        }
    }

    /// <summary>
    /// Receives damage
    /// </summary>
    /// <param name="damage">Damage dealt</param>
    protected override void OnDamageDealt(Damage damage)
    {
        base.OnDamageDealt(damage);
        if (health.IsDead())
        {
            animator.SetTrigger(AnimationNames.Enemy.Triggers.Die);
            foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>())
            {
                monoBehaviour.enabled = false;
            }
            gameObject.layer = LayerMask.NameToLayer(LayerNames.Trigger);
        }
        else
        {
            animator.SetTrigger(AnimationNames.Enemy.Triggers.Hurt);
        }
    }
}