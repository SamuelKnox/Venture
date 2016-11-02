using UnityEngine;

public class PoisonRoon : BlueRoon
{
    /// <summary>
    /// Applies poison to the weapon
    /// </summary>
    /// <param name="weapon">Weapon to apply poison to</param>
    public override void Activate(Weapon weapon)
    {
        ChangePoisonAmount(weapon, GetBaseValue());
        ChangePoisonSpeed(weapon, GetSpecialValue());
    }

    /// <summary>
    /// Removes poison from the weapon
    /// </summary>
    /// <param name="weapon">Weapon to remove poison from</param>
    public override void Deactivate(Weapon weapon)
    {
        ChangePoisonAmount(weapon, -GetBaseValue());
        ChangePoisonSpeed(weapon, -GetSpecialValue());
    }

    /// <summary>
    /// Changes the poison on a piece of weapon
    /// </summary>
    /// <param name="weapon">Weapon whose poison will be changed</param>
    /// <param name="change">Poison to apply</param>
    public void ChangePoisonAmount(Weapon weapon, float change)
    {
        var damage = weapon.GetComponent<Damage>();
        if (!damage)
        {
            Debug.LogError(weapon.name + " is missing Damage component!", weapon.gameObject);
            return;
        }
        damage.SetDamageOverTime(weapon.GetDamageOverTime() + change);
    }

    /// <summary>
    /// Changes the rate at which poison is applied
    /// </summary>
    /// <param name="weapon">Weapon to be changed</param>
    /// <param name="change">Poison rate change</param>
    public void ChangePoisonSpeed(Weapon weapon, float change)
    {
        var damage = weapon.GetComponent<Damage>();
        if (!damage)
        {
            Debug.LogError(weapon.name + " is missing Damage component!", weapon.gameObject);
            return;
        }
        damage.SetDamageOverTimeRateIncrease(weapon.GetDamageOverTimeRateIncrease() + change);
    }
}