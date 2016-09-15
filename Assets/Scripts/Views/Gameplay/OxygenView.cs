using CustomUnityLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OxygenView : MonoBehaviour
{
    [Tooltip("Image filled to display remaining oxygen")]
    [SerializeField]
    private Image oxygenImage;

    [Tooltip("Text to display the percentage of oxygen remaining")]
    [SerializeField]
    private TextMeshProUGUI oxygenText;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets the fill amount for the oxygen image
    /// </summary>
    /// <param name="oxygen">Oxygen (0-1) to set</param>
    public void SetOxygen(float oxygen)
    {
        oxygenImage.fillAmount = oxygen;
        oxygenText.text = oxygen.ToPercentage();
    }
}