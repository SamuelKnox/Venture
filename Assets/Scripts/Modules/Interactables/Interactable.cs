using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Whether or not to interact with this interactable on trigger, opposed to waiting for the player to trigger it manually")]
    [SerializeField]
    private bool actImmediately = false;

    [Tooltip("Whether or not the player can be controlled while interacting with this interactable")]
    [SerializeField]
    private bool playerControllable = true;

    private static Interactable activeInteractable;

    private PlayerController playerController;

    protected virtual void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (!playerController)
        {
            Debug.LogError(gameObject + " could not find player controller!", gameObject);
            return;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
        var player = collider2D.GetComponent<Player>();
        if (player && actImmediately)
        {
            Interact();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collider2D)
    {
        var player = collider2D.GetComponent<Player>();
        if (player)
        {
            EndInteraction();
        }
    }

    public abstract void OnInteractionEnter();

    public abstract void OnInteractionExit();

    /// <summary>
    /// Interacts with the interactable
    /// </summary>
    public void Interact()
    {
        if (activeInteractable)
        {
            return;
        }
        activeInteractable = this;
        playerController.SetPlayerControllable(playerControllable);
        OnInteractionEnter();
    }

    /// <summary>
    /// Finishes interaction
    /// </summary>
    public void EndInteraction()
    {
        if(activeInteractable != this)
        {
            return;
        }
        activeInteractable = null;
        playerController.SetPlayerControllable(true);
        OnInteractionExit();
    }

    /// <summary>
    /// Whether or not to allow the player to be controlled while interacting
    /// </summary>
    /// <returns>Player is controllable</returns>
    public bool IsPlayerControllable()
    {
        return playerControllable;
    }
}