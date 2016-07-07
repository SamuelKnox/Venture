using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Whether or not to emphasize this item and play a player-locking animation when picking this up")]
    [SerializeField]
    private bool specialItem = true;

    [Tooltip("Sound effect to be played when collectable is picked up")]
    [SerializeField]
    private AudioClip pickUpSoundEffect;

    /// <summary>
    /// Gets whether or not this is a special collectable and deserves a special animation
    /// </summary>
    /// <returns>Whether or not the collectable is special</returns>
    public bool IsSpecialItem()
    {
        return specialItem;
    }

    /// <summary>
    /// Gets the sound effect to be played when the collectable is picked up
    /// </summary>
    /// <returns>Sound effect</returns>
    public AudioClip GetPickUpSoundEffect()
    {
        return pickUpSoundEffect;
    }

    /// <summary>
    /// Sets whether or not this item is special and deserves a special animation
    /// </summary>
    /// <param name="special">Whether or not it's special</param>
    public void SetSpecialItem(bool special)
    {
        specialItem = special;
    }
}