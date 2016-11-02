using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpikeTrapView : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Deploys the spike trap
    /// </summary>
    public void Deploy()
    {
        animator.SetTrigger(AnimationNames.SpikeTrap.Triggers.Deploy);
    }

    /// <summary>
    /// Retracts the spike trap
    /// </summary>
    public void Retract()
    {
        animator.SetTrigger(AnimationNames.SpikeTrap.Triggers.Retract);
    }
}