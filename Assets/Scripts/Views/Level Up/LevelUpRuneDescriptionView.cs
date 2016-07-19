using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpRuneDescriptionView : MonoBehaviour
{
    [Tooltip("Icon for rune")]
    [SerializeField]
    private Image icon;

    [Tooltip("Current level description")]
    [SerializeField]
    private TextMeshProUGUI currentLevelDescription;

    [Tooltip("Next level description")]
    [SerializeField]
    private TextMeshProUGUI nextLevelDescription;

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void UpdateDescription(Rune rune)
    {
        icon.sprite = rune.GetIcon();
        currentLevelDescription.text = rune.GetLevelDescription(rune.GetLevel());
        nextLevelDescription.text = rune.GetLevelDescription(rune.GetLevel() + 1);
    }
}