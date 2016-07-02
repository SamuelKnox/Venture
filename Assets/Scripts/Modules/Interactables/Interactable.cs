using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [Tooltip("Whether or not to interact with this interactable on trigger, opposed to waiting for the player to trigger it manually")]
    [SerializeField]
    private bool actImmediately = false;
    /// <summary>
    /// Interacts with the interactable
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Finishes interaction
    /// </summary>
    public abstract void EndInteraction();

    /// <summary>
    /// Whether or not to interact with this interactable immediately, opposed to waiting for the player to initiate the interaction
    /// </summary>
    /// <returns>Should act immediately</returns>
    public bool IsActImmediately()
    {
        return actImmediately;
    }
}