using System;
using CreativeSpore.SmartColliders;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlatformCharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Player
    
    ))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Whether or not to use input to determine speed, or snap speed to a constant")]
    [SerializeField]
    public bool UseAxisAsSpeedFactor = true;

    [Tooltip("Minimum axis value to start moving")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public float AxisMovingThreshold = 0.2f;

    private Player player;
    private PlatformCharacterController platformCharacterController;
    private PlayerView playerView;
    private Vector2 speedModifier = Vector2.one;

    void Awake()
    {
        player = GetComponent<Player>();
        platformCharacterController = GetComponent<PlatformCharacterController>();
        playerView = GetComponent<PlayerView>();
    }

    void Update()
    {
        if (!SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
        {
            var directionalInput = GetDirectionalInput();
            Move(directionalInput);
            Jump(directionalInput.y);
            if (Input.GetButtonDown(InputNames.Attack) && player.IsAttackValid())
            {
                Attack();
            }
            if (Input.GetButtonDown(InputNames.ToggleWeapon))
            {
                player.ToggleWeapon();
            }
            if (Input.GetButtonDown(InputNames.Inventory) && !SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
            {
                Time.timeScale = 0.0f;
                SceneManager.LoadScene(SceneNames.Inventory, LoadSceneMode.Additive);
            }
        }
    }

    public Vector2 GetSpeedModifier()
    {
        return speedModifier;
    }

    public void SetSpeedModifier(Vector2 speedModifier)
    {
        this.speedModifier = speedModifier;
    }

    void OnCollectorStay(Collider2D collider2D)
    {
        var collectable = collider2D.GetComponent<Collectable>();
        if (collectable)
        {
            CollectItem(collectable);
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
    /// Applies movement to the player
    /// </summary>
    private void Move(Vector2 input)
    {
        float horizontalMovement = input.x * Mathf.Abs(input.x);
        float absoluteHorizontalMovement = Mathf.Abs(horizontalMovement);
        float verticalMovement = input.y * Mathf.Abs(input.y);
        float absoluteVerticalMovement = Mathf.Abs(verticalMovement);
        if (absoluteHorizontalMovement >= AxisMovingThreshold)
        {
            platformCharacterController.HorizontalSpeedScale = UseAxisAsSpeedFactor ? absoluteHorizontalMovement : 1.0f;
        }
        if (absoluteVerticalMovement >= AxisMovingThreshold)
        {
            platformCharacterController.VerticalSpeedScale = UseAxisAsSpeedFactor ? absoluteVerticalMovement : 1.0f;
        }
        platformCharacterController.SetActionState(eControllerActions.Left, horizontalMovement <= -AxisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Right, horizontalMovement >= AxisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Down, verticalMovement <= -AxisMovingThreshold);
        platformCharacterController.SetActionState(eControllerActions.Up, verticalMovement >= AxisMovingThreshold);
    }

    /// <summary>
    /// Applies jump and drop down to the player
    /// </summary>
    private void Jump(float verticalInput)
    {
        bool jumping = Input.GetButton(InputNames.Jump) && verticalInput > -AxisMovingThreshold;
        bool droppingDown = Input.GetButtonDown(InputNames.Jump) && verticalInput <= -AxisMovingThreshold;
        platformCharacterController.SetActionState(eControllerActions.Jump, jumping);
        platformCharacterController.SetActionState(eControllerActions.PlatformDropDown, droppingDown);
        platformCharacterController.HorizontalSpeedScale = 1.0f;
        platformCharacterController.VerticalSpeedScale = 1.0f;
    }

    /// <summary>
    /// Collect an item
    /// </summary>
    /// <param name="item">Item to collect</param>
    private void CollectItem(Collectable collectable)
    {
        player.Collect(collectable);
        playerView.Collect(collectable);
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