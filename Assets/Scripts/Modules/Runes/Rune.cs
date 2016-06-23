using System;
using UnityEngine;

[Serializable]
public abstract class Rune : Item
{
    [Tooltip("Type of rune")]
    [SerializeField]
    private RuneType runeType;

    /// <summary>
    /// Gets the type of this rune
    /// </summary>
    /// <returns>Rune Type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
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
}