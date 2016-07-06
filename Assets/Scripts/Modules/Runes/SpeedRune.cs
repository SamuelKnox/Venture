using CreativeSpore.SmartColliders;
using UnityEngine;

public class SpeedRune : YellowRune
{
    [Tooltip("Scalar speed by which the player increases horizontally per level")]
    [SerializeField]
    [Range(0.0f, 100.0f)]
    float speedIncrease = 10.0f;

    /// <summary>
    /// Increases the player's speed by the modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void AttachRune(Equipment equipment)
    {
        AdjustPlayerHorizontalSpeedModifier(speedIncrease * level);
    }

    /// <summary>
    /// Decreases the player's speed by the modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void DetachRune(Equipment equipment)
    {
        AdjustPlayerHorizontalSpeedModifier(-speedIncrease * level);
    }

    /// <summary>
    /// Changes the speed modfier of the player
    /// </summary>
    /// <param name="change">Change is speed modifier</param>
    private void AdjustPlayerHorizontalSpeedModifier(float change)
    {
        var platformCharacterController = transform.root.GetComponent<PlatformCharacterController>();
        if (!platformCharacterController)
        {
            Debug.LogError("A Platform Character Controller was not found in " + name + "'s root!", gameObject);
        }
        platformCharacterController.WalkingAcc += change;
        platformCharacterController.AirborneAcc += change;
        platformCharacterController.MaxWalkingSpeed = Mathf.Infinity;
    }
}