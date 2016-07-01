using System;
using UnityEngine;

[Serializable]
public abstract class Rune : Item
{
    private const float ExperiencePerLevel = 100.0f;

    [Tooltip("Type of rune")]
    [SerializeField]
    private RuneType runeType;

    [Tooltip("Experience on this rune, which affects the level")]
    [SerializeField]
    [Range(0.0f, 1000.0f)]
    private float experience = 0.0f;

    [Tooltip("Rune level")]
    [SerializeField]
    [Range(1, 10)]
    protected int level = 1;

    void OnValidate()
    {
        UpdateLevel();
    }

    /// <summary>
    /// Attaches a rune to equipment
    /// </summary>
    /// <param name="equipment">Equipment rune is being attached to</param>
    public abstract void AttachRune(Equipment equipment);

    /// <summary>
    /// Detaches a rune from equipment
    /// </summary>
    /// <param name="equipment">Equipment rune is being detached from</param>
    public abstract void DetachRune(Equipment equipment);

    /// <summary>
    /// Gets the type of this rune
    /// </summary>
    /// <returns>Rune Type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }

    /// <summary>
    /// Gets the experience for this rune
    /// </summary>
    /// <returns>Experience</returns>
    public float GetExperience()
    {
        return experience;
    }

    /// <summary>
    /// Sets the experience for this rune
    /// </summary>
    /// <param name="experience">Experience to set</param>
    public void SetExperience(float experience)
    {
        this.experience = experience;
    }

    /// <summary>
    /// Updates the level to match the experience
    /// </summary>
    private void UpdateLevel()
    {
        level = (int)(experience / ExperiencePerLevel);
    }
}