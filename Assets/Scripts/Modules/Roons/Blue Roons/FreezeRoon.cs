using CustomUnityLibrary;
using UnityEngine;

public class FreezeRoon : BlueRoon
{
    [Tooltip("Color applied when Enemy is hit by the freeze roon's effect")]
    [SerializeField]
    private Color freezeColor = Color.blue;

    public override void Activate(Weapon weapon)
    {
        var damage = weapon.GetComponent<Damage>();
        if (!damage)
        {
            Debug.LogError(weapon.name + " does not have a Damage Component!", weapon.gameObject);
            return;
        }
        damage.SetSpeedModifierIntensity(damage.GetSpeedModifierIntensity() * GetBaseValue());
        damage.SetSpeedModifierDuration(damage.GetSpeedModifierDuration() + GetSpecialValue());
        damage.SetTint(damage.GetTint() * ColorUtility.GetMinColor(freezeColor));
    }

    public override void Deactivate(Weapon weapon)
    {
        var damage = weapon.GetComponent<Damage>();
        if (!damage)
        {
            Debug.LogError(weapon.name + " does not have a Damage Component!", weapon.gameObject);
            return;
        }
        damage.SetSpeedModifierIntensity(damage.GetSpeedModifierIntensity() / GetBaseValue());
        damage.SetSpeedModifierDuration(damage.GetSpeedModifierDuration() - GetSpecialValue());
        damage.SetTint(damage.GetTint().DivideBy(ColorUtility.GetMinColor(freezeColor)));
    }
}