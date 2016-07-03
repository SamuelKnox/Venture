using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonsView : MonoBehaviour
{
    [Tooltip("Image used for A Button")]
    [SerializeField]
    private Image aButton;

    [Tooltip("Image used for B Button")]
    [SerializeField]
    private Image bButton;

    [Tooltip("Image used for X Button")]
    [SerializeField]
    private Image xButton;

    [Tooltip("Image used for Y Button")]
    [SerializeField]
    private Image yButton;

    /// <summary>
    /// Plays the button animation for the button being pressed
    /// </summary>
    /// <param name="inputName">Input button is tied to</param>
    public void PressButton(string inputName)
    {
        Debug.Log(inputName + " pressed.");
    }

    /// <summary>
    /// Plays the animation for a button being held
    /// </summary>
    /// <param name="inputName">Input button is tied to</param>
    /// <param name="percentPressed">Percentage of hold complete to perform action</param>
    public void HoldButton(string inputName, float percentPressed)
    {
        switch (inputName)
        {
            case InputNames.ClearRunes:
                yButton.fillAmount = percentPressed;
                break;
            case InputNames.LevelUpRune:
                xButton.fillAmount = percentPressed;
                break;
            default:
                Debug.LogError("An invalid input name was used for the button animation!", gameObject);
                return;
        }
    }
}