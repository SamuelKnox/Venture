using TMPro;
using UnityEngine;

public class LevelUpPrestigeView : MonoBehaviour
{
    [Tooltip("Text display used to show current prestige available")]
    [SerializeField]
    private TextMeshProUGUI prestige;

    /// <summary>
    /// Updates the prestige displayed
    /// </summary>
    /// <param name="prestige">Prestige count</param>
    public void UpdatePrestige(int prestige)
    {
        this.prestige.text = prestige.ToString();
    }
}