using UnityEngine;

public abstract class ResourcePool : MonoBehaviour
{
    [Tooltip("Amount of stat available")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float amount;

    /// <summary>
    /// Gets the amount of experience that this provides
    /// </summary>
    /// <returns>Number of experience points</returns>
    public float GetAmount()
    {
        return amount;
    }
}