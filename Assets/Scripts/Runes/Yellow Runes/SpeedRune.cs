using CreativeSpore.SmartColliders;
using UnityEngine;

public class SpeedRune : YellowRune
{
    /// <summary>
    /// Increases the player's speed by the base modifier, and jump height by the special modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void Activate(Equipment equipment)
    {
        AdjustPlayerHorizontalSpeedModifier(GetBaseValue());
        AdjustPlayerJumpHeight(GetSpecialValue());
    }

    /// <summary>
    /// Decreases the player's speed by the modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void Deactivate(Equipment equipment)
    {
        AdjustPlayerHorizontalSpeedModifier(-GetBaseValue());
        AdjustPlayerJumpHeight(-GetSpecialValue());
    }

    /// <summary>
    /// Changes the speed modfier of the player
    /// </summary>
    /// <param name="change">Change is speed modifier</param>
    private void AdjustPlayerHorizontalSpeedModifier(float change)
    {
        var platformCharacterController = PlayerManager.Player.GetComponent<PlatformCharacterController>();
        if (!platformCharacterController)
        {
            Debug.LogError("A Platform Character Controller was not found!", gameObject);
        }
        platformCharacterController.WalkingAcc *= 1.0f + change;
        platformCharacterController.AirborneAcc *= 1.0f + change;
        Debug.LogWarning("this works?");
        platformCharacterController.MaxWalkingSpeed = Mathf.Infinity;
    }

    /// <summary>
    /// Changes the height at which the player jumps
    /// </summary>
    /// <param name="change">How much to change the jump height by</param>
    private void AdjustPlayerJumpHeight(float change)
    {
        var platformCharacterController = PlayerManager.Player.GetComponent<PlatformCharacterController>();
        if (!platformCharacterController)
        {
            Debug.LogError("A Platform Character Controller was not found!", gameObject);
        }
        platformCharacterController.JumpingAcc *= 1.0f + change;
    }
}