using CreativeSpore.SmartColliders;
using UnityEngine;

public class Quicksand : MonoBehaviour
{
    [Tooltip("Jump speed multiplier, used to sink the player into the quick sand")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float handicap = 0.1f;

    //private Damage damage;
    private Vector3 initialGravity;
    private float initialJumpSpeed;
    private int sandCount;

    void Awake()
    {
        //damage = GetComponent<Damage>();
        var playerController = FindObjectOfType<PlatformCharacterController>();
        if (!playerController)
        {
            Debug.LogError("Could not find player controller!", gameObject);
            return;
        }
        initialGravity = playerController.PlatformCharacterPhysics.Gravity;
        initialJumpSpeed = playerController.JumpingSpeed;
    }

    void Start()
    {
        //damage.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("GEWG");
        var playerController = other.GetComponent<PlatformCharacterController>();
        if (playerController)
        {
            sandCount++;
            playerController.PlatformCharacterPhysics.Gravity = initialGravity * handicap;
            playerController.JumpingSpeed = initialJumpSpeed * handicap;
        }
        //var player = other.GetComponent<Player>();
        //if (player)
        //{
        //    var playerCollider = player.GetComponent<Collider2D>();
        //    var topOfPlayerY = player.transform.position.y + playerCollider.bounds.extents.y;
        //    if (transform.position.y >= topOfPlayerY)
        //    {
        //        damage.SetActive(true);
        //    }
        //}
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var playerController = other.GetComponent<PlatformCharacterController>();
        if (playerController)
        {
            sandCount--;
            if (sandCount == 0)
            {
                playerController.PlatformCharacterPhysics.Gravity = initialGravity;
                playerController.JumpingSpeed = initialJumpSpeed;
            }
            //damage.SetActive(false);
            //playerController.JumpingSpeed = initialJumpSpeed;
        }
    }
}