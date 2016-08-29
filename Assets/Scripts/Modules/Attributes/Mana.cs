using CustomUnityLibrary;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [Tooltip("Current mana points")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float currentManaPoints = 100.0f;

    [Tooltip("Max mana points")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float maxManaPoints = 100.0f;

    [Tooltip("Mana over time left to be spent")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float manaCostOverTime = 0.0f;

    void Update()
    {
        ApplyManaCostOverTime();
    }

    void OnValidate()
    {
        currentManaPoints = Mathf.Min(currentManaPoints, maxManaPoints);
    }

    /// <summary>
    /// Gets the current mana points
    /// </summary>
    /// <returns>Current mana points</returns>
    public float GetCurrentManaPoints()
    {
        return currentManaPoints;
    }

    /// <summary>
    /// Sets the current mana points
    /// </summary>
    /// <param name="mana">Mana points to set</param>
    public void SetCurrentManaPoints(float mana)
    {
        currentManaPoints = Mathf.Clamp(mana, 0, GetMaxManaPoints());
    }

    /// <summary>
    /// Gets the max mana points
    /// </summary>
    /// <returns>Max mana points</returns>
    public float GetMaxManaPoints()
    {
        return maxManaPoints;
    }

    /// <summary>
    /// Sets the max mana points
    /// </summary>
    /// <param name="maxManaPoints">Max max points to use</param>
    public void SetMaxManaPoints(float maxManaPoints)
    {
        this.maxManaPoints = maxManaPoints;
    }

    /// <summary>
    /// Applies the damage over time
    /// </summary>
    private void ApplyManaCostOverTime()
    {
        SetCurrentManaPoints(GetCurrentManaPoints() - manaCostOverTime);
    }
}