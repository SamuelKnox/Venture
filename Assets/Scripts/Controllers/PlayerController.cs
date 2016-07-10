using CreativeSpore.SmartColliders;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlatformCharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerView))]
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

    private Player player;
    private PlatformCharacterController platformCharacterController;
    private PlayerView playerView;
    private Interactable nearbyInteractable;
    private bool interacting = false;
    private Health health;
    private QuestsView questsView;

    void Awake()
    {
        player = GetComponent<Player>();
        platformCharacterController = GetComponent<PlatformCharacterController>();
        playerView = GetComponent<PlayerView>();
        health = GetComponentInChildren<Health>();
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
        if (Input.GetButtonDown(InputNames.Interact) && nearbyInteractable && !nearbyInteractable.IsActImmediately() && !interacting)
        {
            interacting = true;
            nearbyInteractable.Interact();
        }
        if (interacting)
        {
            if (Input.GetButtonUp(InputNames.Interact))
            {
                interacting = false;
            }
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
        }
        if (Input.GetButtonDown(InputNames.QuestLeft))
        {
            questsView.SwitchQuest(-1);
        }
        if (Input.GetButtonDown(InputNames.QuestRight))
        {
            questsView.SwitchQuest(1);
        }
        if (Input.GetButtonDown(InputNames.Inventory) && !SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
        {
            Time.timeScale = 0.0f;
            SceneManager.LoadScene(SceneNames.Inventory, LoadSceneMode.Additive);
            enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        var interactable = collider2D.GetComponent<Interactable>();
        if (interactable && !nearbyInteractable)
        {
            nearbyInteractable = interactable;
            if (nearbyInteractable.IsActImmediately())
            {
                nearbyInteractable.Interact();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        var interactable = collider2D.GetComponent<Interactable>();
        if (interactable && interactable == nearbyInteractable)
        {
            nearbyInteractable = null;
            interactable.EndInteraction();
        }
    }

    void OnCollectorStay(Collider2D collider2D)
    {
        var collectable = collider2D.GetComponent<Collectable>();
        if (collectable)
        {
            Collect(collectable);
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
        bool jumping = Input.GetButton(InputNames.Jump) && verticalInput > -axisJumpingThreshold;
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
                    playerView.WantAttack();
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