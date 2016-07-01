using System;
using UnityEngine;

[Serializable]
public abstract class Rune : Item
{
    private const int MinLevel = 1;
    private const int Maxlevel = 10;

    [Tooltip("Type of rune")]
    [SerializeField]
    private RuneType runeType;

    [Tooltip("Rune level")]
    [SerializeField]
    [Range(1, 10)]
    protected int level = 1;

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
    /// Gets the rune's level
    /// </summary>
    /// <returns>Rune level</returns>
    public int GetLevel()
    {
        return level;
    }

    /// <summary>
    /// Sets the rune's level
    /// </summary>
    /// <param name="level">Level to set</param>
    /// <returns>Whether or not the rune's level was successfully set</returns>
    public bool SetLevel(int level)
    {
        if (level < MinLevel || level > Maxlevel)
        {
            return false;
        }
        this.level = level;
        return true;
    }
}