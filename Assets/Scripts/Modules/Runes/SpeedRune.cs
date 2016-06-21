using System;
using UnityEngine;

public class SpeedRune : Rune
{
    [Tooltip("Percent speed by which the player increases")]
    [SerializeField]
    [Range(0.0f, 10.0f)]
    float speedIncreaseRate = 1.0f;

    /// <summary>
    /// Increases the player's speed by the modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void AttachRune(Equipment equipment)
    {
        AdjustPlayerSpeedModifier(speedIncreaseRate);
    }

    /// <summary>
    /// Decreases the player's speed by the modifier
    /// </summary>
    /// <param name="equipment">Equipment which rune is attached to</param>
    public override void DetachRune(Equipment equipment)
    {
        AdjustPlayerSpeedModifier(-speedIncreaseRate);
    }

    /// <summary>
    /// Changes the speed modfier of the player
    /// </summary>
    /// <param name="change">Change is speed modifier</param>
    private void AdjustPlayerSpeedModifier(float change)
    {
        var playerController = FindObjectOfType<PlayerController>();
        if (!playerController)
        {
            Debug.LogError(gameObject + " could not find Player Controller!", gameObject);
            return;
        }
        playerController.SetSpeedModifier(playerController.GetSpeedModifier() + change);
    }
}