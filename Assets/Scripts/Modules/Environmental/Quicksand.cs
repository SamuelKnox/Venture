using CreativeSpore.SmartColliders;
using UnityEngine;

public class Quicksand : MonoBehaviour
{
    [Tooltip("Jump speed multiplier, used to sink the player into the quick sand")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float handicap = 0.1f;
    
    private Vector3 initialGravity;
    private float initialJumpSpeed;
    private int sandCount;

    void Awake()
    {
        var playerController = FindObjectOfType<PlatformCharacterController>();
        if (!playerController)
        {
            Debug.LogError("Could not find player controller!", gameObject);
            return;
        }
        initialGravity = playerController.PlatformCharacterPhysics.Gravity;
        initialJumpSpeed = playerController.JumpingSpeed;
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
        }
    }
}