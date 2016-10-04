using UnityEngine;
using CreativeSpore.SmartColliders;
using CustomUnityLibrary;

[RequireComponent(typeof(PlatformCharacterController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerView : MonoBehaviour
{
    [Tooltip("Whether or not the player is facing right initially")]
    [SerializeField]
    private bool facingRight = true;

    [Tooltip("Sound effect to play when player is stunned")]
    [SerializeField]
    private AudioClip stunSound;

    private PlatformCharacterController platformCharacterController;
    private PlayerController playerController;
    private Animator animator;
    private bool jumping;
    private bool climbing;
    private bool attacking;
    private Collectable collectable;

    void Awake()
    {
        platformCharacterController = GetComponent<PlatformCharacterController>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        SetMovementSpeeds();
        FlipSprite();
        Climb();
    }

    /// <summary>
    /// Whether or not an attack animation is being played
    /// </summary>
    /// <returns>Is attacking</returns>
    public bool IsAttacking()
    {
        return attacking;
    }

    /// <summary>
    /// Initiates a melee attack
    /// </summary>
    public void MeleeWeaponAttack()
    {
        if (attacking)
        {
            return;
        }
        attacking = true;
        animator.SetTrigger(AnimationNames.Player.Triggers.MeleeWeaponAttack);
    }

    /// <summary>
    /// Initiates the bow attack
    /// </summary>
    public void DrawBow()
    {
        if (attacking)
        {
            return;
        }
        attacking = true;
        animator.SetTrigger(AnimationNames.Player.Triggers.BowDraw);
    }

    /// <summary>
    /// Executes a bow attack
    /// </summary>
    public void BowAttack()
    {
        if (!attacking)
        {
            Debug.LogError("Player is attemping to fire their bow, but the Player is not in the attacking state!", gameObject);
            return;
        }
        animator.SetFloat(AnimationNames.Player.Floats.HorizontalInput, Input.GetAxis(InputNames.Horizontal));
        animator.SetFloat(AnimationNames.Player.Floats.VerticalInput, Input.GetAxis(InputNames.Vertical));
        animator.SetTrigger(AnimationNames.Player.Triggers.BowAttack);
    }

    /// <summary>
    /// Initiates a wand attack
    /// </summary>
    public void PrepareWand()
    {
        if (attacking)
        {
            return;
        }
        attacking = true;
        animator.SetTrigger(AnimationNames.Player.Triggers.WandPreparation);
    }

    /// <summary>
    /// Executes a wand attack
    /// </summary>
    public void WandAttack()
    {
        if (!attacking)
        {
            Debug.LogError("Player is attemping to cast a spell, but the Player is not in the attacking state!", gameObject);
            return;
        }
        attacking = true;
        animator.SetFloat(AnimationNames.Player.Floats.HorizontalInput, Input.GetAxis(InputNames.Horizontal));
        animator.SetFloat(AnimationNames.Player.Floats.VerticalInput, Input.GetAxis(InputNames.Vertical));
        animator.SetTrigger(AnimationNames.Player.Triggers.WandAttack);
    }

    /// <summary>
    /// Plays player death animation
    /// </summary>
    public void Die()
    {
        animator.SetBool(AnimationNames.Player.Bools.Dead, true);
        FinishAttacking();
    }

    /// <summary>
    /// Animates the player's jumping
    /// </summary>
    public void Jump()
    {
        if (jumping)
        {
            return;
        }
        jumping = true;
        animator.SetTrigger(AnimationNames.Player.Triggers.Jump);
    }

    /// <summary>
    /// Finishes the player's jump
    /// </summary>
    public void FinishJumping()
    {
        jumping = false;
    }

    /// <summary>
    /// Collects an item
    /// </summary>
    public void Collect(Collectable collectable)
    {
        if (this.collectable && collectable.IsHighPriority())
        {
            this.collectable.gameObject.SetActive(false);
            Destroy(this.collectable);
            collectable.gameObject.SetActive(true);
            this.collectable = collectable;
            return;
        }
        SoundManager.Instance.Play(collectable.GetPickUpSoundEffect());
        if (collectable.IsSpecialItem() && !this.collectable)
        {
            this.collectable = collectable;
            var equippedWeapon = PlayerManager.Player.GetActiveWeapon();
            if (equippedWeapon)
            {
                equippedWeapon.gameObject.SetActive(false);
            }
            animator.SetTrigger(AnimationNames.Player.Triggers.CollectSpecialItem);
        }
        else
        {
            animator.SetTrigger(AnimationNames.Player.Triggers.CollectItem);
            collectable.gameObject.SetActive(false);
            Destroy(collectable);
            FinishAttacking();
        }
    }

    /// <summary>
    /// Sets whether or not the play the stunned animation
    /// </summary>
    /// <param name="stunned">Whether or not is stunned</param>
    public void SetStun(bool stunned)
    {
        animator.SetBool(AnimationNames.Player.Bools.Stunned, stunned);
        SoundManager.Instance.Play(stunSound, false, gameObject);
    }

    /// <summary>
    /// Finished collecting item
    /// </summary>
    private void FinishCollecting()
    {
        if (!collectable)
        {
            return;
        }
        collectable.gameObject.SetActive(false);
        var equippedWeapon = PlayerManager.Player.GetActiveWeapon();
        equippedWeapon.gameObject.SetActive(true);
        Destroy(collectable);
        FinishAttacking();
    }

    /// <summary>
    /// Finished attack animation
    /// </summary>
    public void FinishAttacking()
    {
        attacking = false;
    }

    /// <summary>
    /// Sets the animator's movement speeds
    /// </summary>
    private void SetMovementSpeeds()
    {
        if (!climbing)
        {
            float x = platformCharacterController.InstantVelocity.x;
            float y = platformCharacterController.InstantVelocity.y;
            animator.SetFloat(AnimationNames.Player.Floats.HorizontalSpeed, x);
            animator.SetFloat(AnimationNames.Player.Floats.VerticalSpeed, y);
        }
    }

    /// <summary>
    /// Flips the sprite in the direction the player is moving
    /// </summary>
    private void FlipSprite()
    {
        float absoluteX = Mathf.Abs(transform.localScale.x);
        if (platformCharacterController.GetActionState(eControllerActions.Left))
        {
            transform.localScale = new Vector3(facingRight ? -absoluteX : absoluteX, transform.localScale.y, transform.localScale.z);
        }
        else if (platformCharacterController.GetActionState(eControllerActions.Right))
        {
            transform.localScale = new Vector3(facingRight ? absoluteX : -absoluteX, transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// Updates whether or not the player is climbing
    /// </summary>
    private void Climb()
    {
        climbing = platformCharacterController.IsClimbing;
        animator.SetBool(AnimationNames.Player.Bools.Climbing, climbing);
        if (climbing)
        {
            var direction = playerController.GetDirectionalInput();
            animator.SetFloat(AnimationNames.Player.Floats.HorizontalSpeed, direction.x);
            animator.SetFloat(AnimationNames.Player.Floats.VerticalSpeed, direction.y);
        }
    }
}