using System;

public class StunRune : PurpleRune
{
    /// <summary>
    /// Applies stun to the equipment
    /// </summary>
    /// <param name="equipment">Equipment to apply stun to</param>
    public override void Activate(Equipment equipment)
    {
        ChangeEnemyStunAmount(equipment, GetBaseValue());
        ChangePlayerStunAmount(equipment, GetSpecialValue());
    }

    /// <summary>
    /// Removes stun from the equipment
    /// </summary>
    /// <param name="equipment">Equipment to remove stun from</param>
    public override void Deactivate(Equipment equipment)
    {
        ChangeEnemyStunAmount(equipment, -GetBaseValue());
        ChangePlayerStunAmount(equipment, -GetSpecialValue());
    }

    /// <summary>
    /// Changes the stun on a piece of equipment
    /// </summary>
    /// <param name="equipment">Equipment whose stun will be changed</param>
    /// <param name="change">Stun to apply</param>
    public void ChangeEnemyStunAmount(Equipment equipment, float change)
    {
        Enemy.SetStunTime(Enemy.GetStunTime() + change);
    }

    /// <summary>
    /// Changes the rate at which stun is applied
    /// </summary>
    /// <param name="equipment">Equipment to be changed</param>
    /// <param name="change">Stun rate change</param>
    public void ChangePlayerStunAmount(Equipment equipment, float change)
    {
        Player.SetStunTime(Player.GetStunTime() + change);
    }
}