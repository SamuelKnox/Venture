using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryInstructionsView : MonoBehaviour
{
    [Tooltip("Instructions for when A Button is pressed")]
    [SerializeField]
    private RectTransform aButton;

    [Tooltip("Instructions for when B Button is pressed")]
    [SerializeField]
    private RectTransform bButton;

    [Tooltip("Instructions for when X Button is pressed")]
    [SerializeField]
    private RectTransform xButton;

    [Tooltip("Instructions for when Y Button is pressed")]
    [SerializeField]
    private RectTransform yButton;

    /// <summary>
    /// Updates the instructions for the gamepad inputs
    /// </summary>
    /// <param name="inputInstructions">Instructions to use</param>
    public void UpdateInstructions(Dictionary<GamePadInputs, string> inputInstructions)
    {
        var aInstruction = aButton.GetComponentInChildren<TextMeshProUGUI>();
        var bInstruction = bButton.GetComponentInChildren<TextMeshProUGUI>();
        var xInstruction = xButton.GetComponentInChildren<TextMeshProUGUI>();
        var yInstruction = yButton.GetComponentInChildren<TextMeshProUGUI>();
        aInstruction.text = inputInstructions[GamePadInputs.A];
        bInstruction.text = inputInstructions[GamePadInputs.B];
        xInstruction.text = inputInstructions[GamePadInputs.X];
        yInstruction.text = inputInstructions[GamePadInputs.Y];
        aButton.gameObject.SetActive(aInstruction.text != null);
        bButton.gameObject.SetActive(bInstruction.text != null);
        xButton.gameObject.SetActive(xInstruction.text != null);
        yButton.gameObject.SetActive(yInstruction.text != null);
    }
}