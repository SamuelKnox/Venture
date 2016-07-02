using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [Tooltip("Type of resource")]
    [SerializeField]
    private ResourceType resourceType;

    [Tooltip("Amount of resource")]
    [SerializeField]
    [Range(1, 100)]
    private int amount;

    /// <summary>
    /// Gets the type of this resource
    /// </summary>
    /// <returns>Resource type</returns>
    public ResourceType GetResourceType()
    {
        return resourceType;
    }

    /// <summary>
    /// Gets the amount of this resource
    /// </summary>
    /// <returns>Resource count</returns>
    public int GetAmount()
    {
        return amount;
    }
}