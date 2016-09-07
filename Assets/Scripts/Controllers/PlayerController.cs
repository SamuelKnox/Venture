using CreativeSpore.SmartColliders;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlatformCharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Whether or not to use input to determine speed, or snap speed to a constant")]
    [SerializeField]
    private bool useAxisAsSpeedFactor = true;

    [Tooltip("Minimum axis value to start moving")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float axisMovingThreshold = 0.2f;

    [Tooltip("Minimum axis value to jump or drop down")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float axisJumpingThreshold = 0.5f;

    [Tooltip("Layers which have a quicksand effect on the player")]
    [SerializeField]
    private LayerMask quicksandLayers;

    [Tooltip("Vertical handicap applied to player when quicksand is in effect")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float quicksandSinkRate = 50.0f;

    [Tooltip("Handicap applied to player jumping while in quicksand")]
    [SerializeField]
    [Range(1.0f, 100.0f)]
    private float quicksandJumpHandicap = 25.0f;

    private Player player;
    private PlatformCharacterController platformCharacterController;
    private PlayerView playerView;
    private Interactable nearbyInteractable;
    private Health health;
    private QuestsView questsView;
    private bool controllable = true;
    private bool inQuicksand = false;
    private Vector3 initialGravity;
    private float initialJumpSpeed;
    private Collider2D playerCollider;

    void Awake()
    {
        player = GetComponent<Player>();
        platformCharacterController = GetComponent<PlatformCharacterController>();
        playerView = GetComponent<PlayerView>();
        health = GetComponentInChildren<Health>();
        playerCollider = GetComponent<Collider2D>();
        initialGravity = platformCharacterController.PlatformCharacterPhysics.Gravity;
        initialJumpSpeed = platformCharacterController.JumpingSpeed;
        if (!health)
        {
            Debug.LogError("Could not find player health!", gameObject);
            return;
        }
        questsView = FindObjectOfType<QuestsView>();
        if (!questsView)
        {
            Debug.LogError("Could not find quests view!", gameObject);
            return;
        }
    }

    void Start()
    {
        player.Load();
    }

    void Update()
    {
        if (health.IsDead())
        {
            GameOver();
            return;
        }
        if (Input.GetButtonDown(InputNames.Interact) && nearbyInteractable)
        {
            nearbyInteractable.Interact();
        }
        if (Input.GetButtonUp(InputNames.Interact) && nearbyInteractable && nearbyInteractable.IsPlayerControllable())
        {
            SetPlayerControllable(true);
            return;
        }
        if (!controllable || player.IsStunned())
        {
            return;
        }
        var directionalInput = GetDirectionalInput();
        Move(directionalInput);
        Jump(directionalInput.y);
        if (Input.GetButtonDown(InputNames.Attack) && player.IsAttackValid() && !playerView.IsAttacking())
        {
            Attack();
        }
        if (Input.GetButtonDown(InputNames.ToggleWeapon))
        {
            player.ToggleWeapon();
            playerView.FinishAttacking();
        }
        if (Input.GetButtonDown(InputNames.QuestLeft))
        {
            questsView.SwitchQuest(-1);
        }
        if (Input.GetButtonDown(InputNames.QuestRight))
        {
            questsView.SwitchQuest(1);
        }
    }

    void OnHitboxEnter(Collider2D collider2D)
    {
        var interactable = collider2D.GetComponent<Interactable>();
        if (interactable && !nearbyInteractable)
        {
            nearbyInteractable = interactable;
        }
        if (quicksandLayers.Contains(collider2D.gameObject.layer))
        {
            inQuicksand = true;
            platformCharacterController.PlatformCharacterPhysics.Gravity = initialGravity / quicksandSinkRate;
            platformCharacterController.PlatformCharacterPhysics.Velocity = platformCharacterController.PlatformCharacterPhysics.Gravity;
            platformCharacterController.JumpingSpeed = initialJumpSpeed / quicksandJumpHandicap;
        }
    }

    void OnHitboxStay(Collider2D collider2D)
    {
        var collectable = collider2D.GetComponent<Collectable>();
        if (collectable)
        {
            Collect(collectable);
        }
        if (quicksandLayers.Contains(collider2D.gameObject.layer))
        {
            platformCharacterController.IsGrounded = true;
            if (playerCollider.bounds.max.y <= collider2D.bounds.max.y)
            {
                player.Die();
            }
        }
    }

    void OnHitboxExit(Collider2D collider2D)
    {
        var interactable = collider2D.GetComponent<Interactable>();
        if (interactable && interactable == nearbyInteractable)
        {
            nearbyInteractable = null;
        }
        if (quicksandLayers.Contains(collider2D.gameObject.layer))
        {
            inQuicksand = false;
            platformCharacterController.PlatformCharacterPhysics.Gravity = initialGravity;
            platformCharacterController.JumpingSpeed = initialJumpSpeed;
        }
    }

    /// <summary>
    /// Sets whether or not the player is controllable
    /// </summary>
    /// <param name="controllable">Player can be controlled</param>
    public void SetPlayerControllable(bool controllable)
    {
        this.controllable = controllable;
        if (!this.controllable)
        {
            platformCharacterController.PlatformCharacterPhysics.Velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Gets the gamepad/keyboard controller input for horizontal and vertical
    /// </summary>
    /// <returns>Horizontal and vertical input</returns>
    public Vector2 GetDirectionalInput()
    {
        float horizontalInput = Input.GetAxis(InputNames.Horizontal);
        float verticalInput = Input.GetAxis(InputNames.Vertical);
        var input = new Vector2(horizontalInput, verticalInput);
        return input;
    }

    /// <summary>
    /// Collect an item
    /// </summary>
    /// <param name="item">Item to collect</param>
    public void Collect(Collectable collectable)
    {
        player.Collect(collectable);
    }

    /// <summary>
    /// Applies movement to the player
    /// </summary>
    private void Move(Vector2 input)
    {
        if (inQuicksand)
        {
            input = Vector2.zero;
        }
        float horizontalMovement = input.x * Mathf.Abs(input.x);
        float absoluteHorizontalMovement = Mathf.Abs(horizontalMovement);
        float verticalMovement = input.y * Mathf.Abs(input.y);
        float absoluteVerticalMovement = Mathf.Abs(verticalMovement);
        if (absoluteHorizontalMovement >= axisMovingThreshold)
        {
            platformCharacterController.HorizontalSpeedScale = useAxisAsSpeedFactor ? absoluteHorizontalMovement : 1.0f;
        }
        if (absoluteVerticalMovement >= axisMovingThreshold)
        {
            platformCharacterController.VerticalSpeedScale = useAxisAsSpeedFactor ? absoluteVerticalMovement : 1.0f;
        }
        platformCharacterController.SetActionState(eControllerActions.Left, horizontalMovement <= -axisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Right, horizontalMovement >= axisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Down, verticalMovement <= -axisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Up, verticalMovement >= axisMovingThreshold);
    }

    /// <summary>
    /// Applies jump and drop down to the player
    /// </summary>
    private void Jump(float verticalInput)
    {
        bool jumping;
        if (inQuicksand)
        {
            jumping = Input.GetButtonDown(InputNames.Jump);
        }
        else
        {
            jumping = Input.GetButton(InputNames.Jump) && verticalInput > -axisJumpingThreshold;
        }
        bool droppingDown = Input.GetButtonDown(InputNames.Jump) && verticalInput <= -axisJumpingThreshold;
        platformCharacterController.SetActionState(eControllerActions.Jump, jumping);
        platformCharacterController.SetActionState(eControllerActions.PlatformDropDown, droppingDown);
        platformCharacterController.HorizontalSpeedScale = 1.0f;
        platformCharacterController.VerticalSpeedScale = 1.0f;
    }

    /// <summary>
    /// Acts when the player has died
    /// </summary>
    private void GameOver()
    {
        player.Save();
        if (player.GetPrestige() > 0)
        {
            SceneManager.LoadScene(SceneNames.LevelUp);
        }
        else
        {
            SceneManager.LoadScene(SceneNames.Venture);
        }
    }

    /// <summary>
    /// Initiates the player's attack with their currently equipped weapon
    /// </summary>
    private void Attack()
    {
        var activeWeapon = player.GetActiveWeapon();
        if (!activeWeapon)
        {
            return;
        }
        var activeWeaponType = activeWeapon.GetItemType();
        switch (activeWeaponType)
        {
            case ItemType.MeleeWeapon:
                playerView.MeleeWeaponAttack();
                break;
            case ItemType.RangedWeapon:
                if (activeWeapon.GetComponent<Bow>())
                {
                    playerView.BowAttack();
                }
                else if (activeWeapon.GetComponent<Wand>())
                {
                    playerView.WandAttack();
                }
                else
                {
                    Debug.LogError("Attempting to attack with a Ranged Weapon that is neither a bow nor a wand!", activeWeapon.gameObject);
                    return;
                }
                break;
            default:
                Debug.LogError("An invalid WeaponType is active!", player.gameObject);
                return;
        }
    }
}