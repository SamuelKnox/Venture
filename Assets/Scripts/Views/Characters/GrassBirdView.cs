using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GrassBirdView : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Grass bird takes off from perch
    /// </summary>
    public void Launch()
    {
        animator.SetTrigger(AnimationNames.GrassBird.Triggers.Launch);
    }

    /// <summary>
    /// Grass bird lands on perch
    /// </summary>
    public void Land()
    {
        animator.SetTrigger(AnimationNames.GrassBird.Triggers.Land);
    }
}