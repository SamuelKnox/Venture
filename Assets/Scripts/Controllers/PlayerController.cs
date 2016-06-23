using CustomUnityLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is an example controller for using the CharacterPlatformer
/// </summary>
[RequireComponent(typeof(CharacterPlatformer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private const float DropDownForceRequired = 0.5f;

    private CharacterPlatformer characterPlatformer;
    private Animator animator;
    private Player player;
    private Collectable itemBeingCollected;
    private float speedModifier = 1.0f;

    void Awake()
    {
        characterPlatformer = GetComponent<CharacterPlatformer>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        characterPlatformer.onControllerCollidedEvent += OnCollision;
        characterPlatformer.onTriggerEnterEvent += OnEnter;
        characterPlatformer.onTriggerStayEvent += OnStay;
        characterPlatformer.onTriggerExitEvent += OnExit;
    }

    void Update()
    {
        if (!SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
        {
            characterPlatformer.Move(Input.GetAxis(InputNames.Horizontal) * speedModifier);
            if (Input.GetButton(InputNames.Jump))
            {
                if (Input.GetAxis(InputNames.Vertical) <= -DropDownForceRequired)
                {
                    characterPlatformer.DropDownPlatform();
                }
                else
                {
                    characterPlatformer.Jump();
                }
            }
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
            animator.SetFloat(AnimationNames.Player.Floats.HorizontalSpeed, Mathf.Abs(characterPlatformer.GetVelocity().x));
            animator.SetFloat(AnimationNames.Player.Floats.VerticalSpeed, characterPlatformer.GetVelocity().y);
        }
    }

    void OnCollision(RaycastHit2D hit)
    {
        ///Debug.Log("OnCollision: " + hit.transform.gameObject.name);
    }


    void OnEnter(Collider2D collider2D)
    {
        ///Debug.Log("OnEnter: " + collider2D.gameObject.name);
    }
    
    void OnExit(Collider2D collider2D)
    {
        ///Debug.Log("OnExit: " + collider2D.gameObject.name);
    }

    void OnStay(Collider2D collider2D)
    {
        ///Debug.Log("OnStay: " + collider2D.gameObject.name);
        var collectable = collider2D.GetComponent<Collectable>();
        if (collectable)
        {
            CollectItem(collectable);
        }
    }

    /// <summary>
    /// Gets the speed modifier
    /// </summary>
    /// <returns>Rate by which the speed will be modified</returns>
    public float GetSpeedModifier()
    {
        return speedModifier;
    }

    /// <summary>
    /// Sets the speed modifier
    /// </summary>
    /// <param name="speed">New speed modifier</param>
    public void SetSpeedModifier(float speed)
    {
        speedModifier = speed;
    }

    /// <summary>
    /// Collect an item
    /// </summary>
    /// <param name="item">Item to collect</param>
    private void CollectItem(Collectable collectable)
    {
        if (itemBeingCollected)
        {
            return;
        }
        itemBeingCollected = collectable;
        var equippedWeapon = player.GetActiveWeapon();
        equippedWeapon.gameObject.SetActive(false);
        player.CollectItem(collectable);
        animator.SetTrigger(AnimationNames.Player.Triggers.CollectItem);
    }

    /// <summary>
    /// Finished collecting item
    /// </summary>
    private void FinishCollectingItem()
    {
        itemBeingCollected.gameObject.SetActive(false);
        var equippedWeapon = player.GetActiveWeapon();
        equippedWeapon.gameObject.SetActive(true);
        Destroy(itemBeingCollected);
        itemBeingCollected = null;
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
                animator.SetTrigger(AnimationNames.Player.Triggers.MeleeWeaponAttack);
                break;
            case ItemType.RangedWeapon:
                if (activeWeapon.GetComponent<Bow>())
                {
                    animator.SetTrigger(AnimationNames.Player.Triggers.BowAttack);
                }
                else if (activeWeapon.GetComponent<Wand>())
                {
                    animator.SetTrigger(AnimationNames.Player.Triggers.WandAttack);
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