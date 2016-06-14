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
    private Item itemPickedUp;

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
        characterPlatformer.Move(Input.GetAxis(InputNames.Horizontal));
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
        if (Input.GetButtonDown(InputNames.SelectMeleeWeapon))
        {
            player.SetActiveWeapon(ItemType.MeleeWeapon);
        }
        if (Input.GetButtonDown(InputNames.SelectBow))
        {
            player.SetActiveWeapon(ItemType.Bow);
        }
        if (Input.GetButtonDown(InputNames.SelectWand))
        {
            player.SetActiveWeapon(ItemType.Wand);
        }
        if (Input.GetButtonDown(InputNames.ItemMenu) && !SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
        {
            Time.timeScale = 0.0f;
            SceneManager.LoadScene(SceneNames.Inventory, LoadSceneMode.Additive);
        }
        animator.SetFloat(AnimationNames.Player.Floats.HorizontalSpeed, Mathf.Abs(characterPlatformer.GetVelocity().x));
        animator.SetFloat(AnimationNames.Player.Floats.VerticalSpeed, characterPlatformer.GetVelocity().y);
    }

    void OnCollision(RaycastHit2D hit)
    {
        ///Debug.Log("OnCollision: " + hit.transform.gameObject.name);
    }


    void OnEnter(Collider2D collider2D)
    {
        ///Debug.Log("OnEnter: " + collider2D.gameObject.name);
        var item = collider2D.GetComponent<Item>();
        if (item)
        {
            CollectItem(item);
        }
    }

    void OnStay(Collider2D collider2D)
    {
        ///Debug.Log("OnStay: " + collider2D.gameObject.name);
    }


    void OnExit(Collider2D collider2D)
    {
        ///Debug.Log("OnExit: " + collider2D.gameObject.name);
    }

    /// <summary>
    /// Collect an item
    /// </summary>
    /// <param name="item">Item to collect</param>
    private void CollectItem(Item item)
    {
        if (itemPickedUp)
        {
            return;
        }
        itemPickedUp = item;
        var equippedWeapon = player.GetActiveWeapon();
        equippedWeapon.gameObject.SetActive(false);
        player.CollectItem(item);
        animator.SetTrigger(AnimationNames.Player.Triggers.CollectItem);
    }

    /// <summary>
    /// Finished collecting item
    /// </summary>
    private void FinishCollectingItem()
    {
        itemPickedUp.gameObject.SetActive(false);
        var equippedWeapon = player.GetActiveWeapon();
        equippedWeapon.gameObject.SetActive(true);
        itemPickedUp = null;
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
            case ItemType.Bow:
                animator.SetTrigger(AnimationNames.Player.Triggers.BowAttack);
                break;
            case ItemType.Wand:
                animator.SetTrigger(AnimationNames.Player.Triggers.WandAttack);
                break;
            default:
                Debug.LogError("An invalid WeaponType is active!", player.gameObject);
                break;
        }
    }
}