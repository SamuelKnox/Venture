using UnityEngine;

public class PoisonRune : BlueRune
{
    /// <summary>
    /// Applies poison to the equipment
    /// </summary>
    /// <param name="equipment">Equipment to apply poison to</param>
    public override void Activate(Equipment equipment)
    {
        ChangePoisonAmount(equipment, GetBaseValue());
        ChangePoisonSpeed(equipment, GetSpecialValue());
    }

    /// <summary>
    /// Removes poison from the equipment
    /// </summary>
    /// <param name="equipment">Equipment to remove poison from</param>
    public override void Deactivate(Equipment equipment)
    {
        ChangePoisonAmount(equipment, -GetBaseValue());
        ChangePoisonSpeed(equipment, -GetSpecialValue());
    }

    /// <summary>
    /// Changes the poison on a piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment whose poison will be changed</param>
    /// <param name="change">Poison to apply</param>
    public void ChangePoisonAmount(Equipment equipment, float change)
    {
        equipment.SetDamageOverTime(equipment.GetDamageOverTime() + change);
    }

    /// <summary>
    /// Changes the rate at which poison is applied
    /// </summary>
    /// <param name="equipment">Equipment to be changed</param>
    /// <param name="change">Poison rate change</param>
    public void ChangePoisonSpeed(Equipment equipment, float change)
    {
        equipment.SetDamageOverTimeRateIncrease(equipment.GetDamageOverTimeRateIncrease() + change);
    }
}