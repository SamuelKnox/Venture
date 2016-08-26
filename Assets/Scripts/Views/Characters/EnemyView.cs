using CustomUnityLibrary;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyView : MonoBehaviour
{
    [Tooltip("Sound effect to play when enemy attacks")]
    [SerializeField]
    private AudioClip attackSound;

    [Tooltip("Sound effect to play when enemy is injured")]
    [SerializeField]
    private AudioClip hurtSound;

    [Tooltip("Sound effect to play when enemy is stunned")]
    [SerializeField]
    private AudioClip stunSound;

    [Tooltip("Sound to be played when enemy dies")]
    [SerializeField]
    private AudioClip deathSound;

    private Animator animator;
    private Rigidbody2D body2D;
    private bool stunned;

    void Awake()
    {
        animator = GetComponent<Animator>();
        body2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        SetMovementSpeeds();
    }

    /// <summary>
    /// Sets movement speeds for animator
    /// </summary>
    public void SetMovementSpeeds()
    {
        if (body2D)
        {
            animator.SetFloat(AnimationNames.Enemy.Floats.HorizontalSpeed, Mathf.Abs(body2D.velocity.x));
            animator.SetFloat(AnimationNames.Enemy.Floats.VerticalSpeed, body2D.velocity.y);
        }
    }

    /// <summary>
    /// Attack animation
    /// </summary>
    public void Attack()
    {
        animator.SetTrigger(AnimationNames.Enemy.Triggers.Attack);
        SoundManager.Instance.Play(attackSound, false, gameObject);
    }

    /// <summary>
    /// Death animation
    /// </summary>
    public void Die()
    {
        animator.SetBool(AnimationNames.Enemy.Bools.Death, true);
        SoundManager.Instance.Play(deathSound, false, gameObject);
    }

    /// <summary>
    /// Injury animation
    /// </summary>
    public void Hurt()
    {
        animator.SetTrigger(AnimationNames.Enemy.Triggers.Hurt);
        SoundManager.Instance.Play(hurtSound, false, gameObject);
    }

    /// <summary>
    /// Sets whether or not the play the stunned animation
    /// </summary>
    /// <param name="stunned">Whether or not is stunned</param>
    public void SetStun(bool stunned)
    {
        animator.SetBool(AnimationNames.Enemy.Bools.Stunned, stunned);
        SoundManager.Instance.Play(stunSound, false, gameObject);
    }
}
