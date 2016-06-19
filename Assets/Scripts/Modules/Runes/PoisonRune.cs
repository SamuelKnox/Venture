using UnityEngine;

public class PoisonRune : Rune
{
    [Tooltip("Amount of poison to apply")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    private float poison = 1.0f;

    /// <summary>
    /// Applies poison to the equipment
    /// </summary>
    /// <param name="equipment">Equipment to apply poison to</param>
    public override void AttachRune(Equipment equipment)
    {
        ChangePoison(equipment, poison);
    }

    /// <summary>
    /// Removes poison from the equipment
    /// </summary>
    /// <param name="equipment">Equipment to remove poison from</param>
    public override void DetachRune(Equipment equipment)
    {
        ChangePoison(equipment, -poison);
    }

    /// <summary>
    /// Changes the poison on a piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment whose poison will be changed</param>
    /// <param name="change">Poison to apply</param>
    public void ChangePoison(Equipment equipment, float change)
    {
        equipment.SetDamageOverTime(equipment.GetDamageOverTime() + change);
    }
}