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

    [Tooltip("Icon showing the rune's current level")]
    [SerializeField]
    private Image currentLevelIcon;

    [Tooltip("Next level description")]
    [SerializeField]
    private TextMeshProUGUI nextLevelDescription;

    [Tooltip("Icon showing the rune's next level")]
    [SerializeField]
    private Image nextLevelIcon;

    [Tooltip("Next level prestige cost")]
    [SerializeField]
    private TextMeshProUGUI prestigeCost;

    [Tooltip("Selector for icon display for runes by level")]
    [SerializeField]
    public RuneLevelIconSelector runeLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void UpdateDescription(Rune rune)
    {
        icon.sprite = rune.GetIcon();
        currentLevelDescription.text = rune.GetLevelDescription(rune.GetLevel());
        currentLevelIcon.sprite = runeLevelIconSelector.GetIcon(rune.GetLevel());
        nextLevelDescription.text = rune.GetLevelDescription(rune.GetLevel() + 1);
        nextLevelIcon.sprite = runeLevelIconSelector.GetIcon(rune.GetLevel() + 1);
        prestigeCost.text = rune.GetPrestigeCostToLevelUp().ToString();
    }
}