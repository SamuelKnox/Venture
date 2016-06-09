using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// This is an example controller for using the CharacterPlatformer
/// </summary>
[RequireComponent(typeof(CharacterPlatformer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Inventory))]
public class PlayerController : MonoBehaviour
{
    private const float DropDownForceRequired = 0.5f;

    private CharacterPlatformer characterPlatformer;
    private Animator animator;
    private Inventory inventory;
    private PlayerWeaponTypes activeWeaponType;

    void Awake()
    {
        characterPlatformer = GetComponent<CharacterPlatformer>();
        animator = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
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
        if (Input.GetButtonDown(InputNames.Attack) && IsAttackValid())
        {
            Attack();
        }
        else if (Input.GetButtonDown(InputNames.SelectMeleeWeapon))
        {
            SetActiveWeapon(PlayerWeaponTypes.MeleeWeapon);
        }
        else if (Input.GetButtonDown(InputNames.SelectBow))
        {
            SetActiveWeapon(PlayerWeaponTypes.Bow);
        }
        else if (Input.GetButtonDown(InputNames.SelectWand))
        {
            SetActiveWeapon(PlayerWeaponTypes.Wand);
        }
    }

    void OnCollision(RaycastHit2D hit)
    {
        ///Debug.Log("onControllerCollider: " + hit.transform.gameObject.name);
    }


    void OnEnter(Collider2D collider2D)
    {
        ///Debug.Log("onTriggerEnterEvent: " + collider2D.gameObject.name);
    }

    void OnStay(Collider2D collider2D)
    {
        ///Debug.Log("onTriggerStayEvent: " + collider2D.gameObject.name);
    }


    void OnExit(Collider2D collider2D)
    {
        ///Debug.Log("onTriggerExitEvent: " + collider2D.gameObject.name);
    }

    /// <summary>
    /// Swings the melee weapon
    /// </summary>
    public void BeginMeleeAttack()
    {
        var meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        if (meleeWeapon)
        {
            meleeWeapon.BeginSwing();
        }
    }

    /// <summary>
    /// Finishes the melee weaponn swing
    /// </summary>
    public void FinishMeleeAttack()
    {
        var meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        if (meleeWeapon)
        {
            meleeWeapon.FinishSwing();
        }
    }

    /// <summary>
    /// Fires the player's bow
    /// </summary>
    public void FireBow()
    {
        var bow = GetComponentInChildren<Bow>();
        if (bow)
        {
            bow.Fire();
        }
    }

    /// <summary>
    /// Casts a spell based on the player's currently equipped wand
    /// </summary>
    public void CastSpell()
    {
        var wand = GetComponentInChildren<Wand>();
        if (wand)
        {
            wand.CastSpell();
        }
    }

    /// <summary>
    /// Initiates the player's attack with their currently equipped weapon
    /// </summary>
    private void Attack()
    {
        switch (activeWeaponType)
        {
            case PlayerWeaponTypes.MeleeWeapon:
                animator.SetTrigger(AnimationNames.Player.MeleeWeaponAttack);
                break;
            case PlayerWeaponTypes.Bow:
                animator.SetTrigger(AnimationNames.Player.BowAttack);
                break;
            case PlayerWeaponTypes.Wand:
                animator.SetTrigger(AnimationNames.Player.WandAttack);
                break;
            default:
                Debug.LogError("An invalid WeaponType is active!", gameObject);
                break;
        }
    }

    /// <summary>
    /// Checks whether or not the player is able to perform an attack
    /// </summary>
    /// <returns>If the attack is valid</returns>
    private bool IsAttackValid()
    {
        switch (activeWeaponType)
        {
            case PlayerWeaponTypes.MeleeWeapon:
                return GetComponentInChildren<MeleeWeapon>();
            case PlayerWeaponTypes.Bow:
                var bow = GetComponentInChildren<Bow>();
                if (!bow)
                {
                    return false;
                }
                return bow.IsReady();
            case PlayerWeaponTypes.Wand:
                var wand = GetComponentInChildren<Wand>();
                if (!wand)
                {
                    return false;
                }
                return wand.IsReady();
            default:
                Debug.LogError("An invalid WeaponType is active!", gameObject);
                return false;
        }
    }

    /// <summary>
    /// Sets the player's currently active weapon
    /// </summary>
    /// <param name="weaponType">Type of weapon to activate</param>
    private void SetActiveWeapon(PlayerWeaponTypes weaponType)
    {
        var meleeWeapon = inventory.GetActiveMeleeWeapon();
        if (meleeWeapon)
        {
            meleeWeapon.gameObject.SetActive(weaponType == PlayerWeaponTypes.MeleeWeapon);
        }
        var bow = inventory.GetActiveBow();
        if (bow)
        {
            bow.gameObject.SetActive(weaponType == PlayerWeaponTypes.Bow);
        }
        var wand = inventory.GetActiveWand();
        if (wand)
        {
            wand.gameObject.SetActive(weaponType == PlayerWeaponTypes.Wand);
        }
        activeWeaponType = weaponType;
    }
}