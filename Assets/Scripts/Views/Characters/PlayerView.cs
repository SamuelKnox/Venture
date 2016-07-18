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

    private PlatformCharacterController platformCharacterController;
    private PlayerController playerController;
    private Animator animator;
    private Player player;
    private bool climbing;
    private bool attacking;
    private Collectable collectable;

    void Awake()
    {
        platformCharacterController = GetComponent<PlatformCharacterController>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
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
    /// Initiates a bow attack
    /// </summary>
    public void BowAttack()
    {
        if (attacking)
        {
            return;
        }
        attacking = true;
        animator.SetTrigger(AnimationNames.Player.Triggers.BowAttack);
    }

    /// <summary>
    /// Initiates a wand attack
    /// </summary>
    public void WandAttack()
    {
        if (attacking)
        {
            return;
        }
        attacking = true;
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
    /// Collects an item
    /// </summary>
    public void Collect(Collectable collectable)
    {
        SoundManager.Instance.Play(collectable.GetPickUpSoundEffect());
        if (collectable.IsSpecialItem() && !this.collectable)
        {
            this.collectable = collectable;
            var equippedWeapon = player.GetActiveWeapon();
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
    /// Finished collecting item
    /// </summary>
    private void FinishCollecting()
    {
        collectable.gameObject.SetActive(false);
        var equippedWeapon = player.GetActiveWeapon();
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
            float x = platformCharacterController.RealVelocity.x;
            float y = platformCharacterController.RealVelocity.y;
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