using CustomUnityLibrary;
using UnityEngine;

/// <summary>
/// This is an example controller for using the CharacterPlatformer
/// </summary>
[RequireComponent(typeof(CharacterPlatformer))]
[RequireComponent(typeof(RangedWeapon))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private const float DropDownForceRequired = 0.5f;

    private CharacterPlatformer characterPlatformer;
    private RangedWeapon rangedWeapon;
    private Animator animator;

    void Awake()
    {
        characterPlatformer = GetComponent<CharacterPlatformer>();
        characterPlatformer.onControllerCollidedEvent += OnCollision;
        characterPlatformer.onTriggerEnterEvent += OnEnter;
        characterPlatformer.onTriggerStayEvent += OnStay;
        characterPlatformer.onTriggerExitEvent += OnExit;
        rangedWeapon = GetComponent<RangedWeapon>();
        animator = GetComponent<Animator>();
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
        if (Input.GetButtonDown(InputNames.Fire))
        {
            rangedWeapon.Fire();
            animator.SetTrigger(AnimationNames.Player.Attack0);
        }
    }

    void OnCollision(RaycastHit2D hit)
    {
        Debug.Log("onControllerCollider: " + hit.transform.gameObject.name);
    }


    void OnEnter(Collider2D collider2D)
    {
        Debug.Log("onTriggerEnterEvent: " + collider2D.gameObject.name);
    }

    void OnStay(Collider2D collider2D)
    {
        Debug.Log("onTriggerStayEvent: " + collider2D.gameObject.name);
    }


    void OnExit(Collider2D collider2D)
    {
        Debug.Log("onTriggerExitEvent: " + collider2D.gameObject.name);
    }
}