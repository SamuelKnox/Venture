using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SmasherView : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Smasher is attacking
    /// </summary>
    public void Attack()
    {
        animator.SetTrigger(AnimationNames.Smasher.Triggers.Attack);
    }

    /// <summary>
    /// Smasher is on the retreat
    /// </summary>
    public void Retreat()
    {
        animator.SetTrigger(AnimationNames.Smasher.Triggers.Retreat);
    }

    /// <summary>
    /// Smasher goes idle
    /// </summary>
    public void Idle()
    {
        animator.SetTrigger(AnimationNames.Smasher.Triggers.Idle);
    }
}