using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [Tooltip("Image used to show health")]
    [SerializeField]
    private Image heart;

    /// <summary>
    /// Adjusts the image to display the health
    /// </summary>
    /// <param name="healthPercent">Percent of health remaining</param>
    public void AdjustHealth(float healthPercent)
    {
        heart.fillAmount = healthPercent;
    }
}