using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExplodingPlatformView : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Explodes the platform
    /// </summary>
    public void Explode()
    {
        animator.SetTrigger(AnimationNames.ExplodingPlatform.Triggers.Explode);
    }
}