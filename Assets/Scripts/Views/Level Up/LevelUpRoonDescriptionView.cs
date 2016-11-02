using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpRoonDescriptionView : MonoBehaviour
{
    [Tooltip("Icon for roon")]
    [SerializeField]
    private Image icon;

    [Tooltip("Current level description")]
    [SerializeField]
    private TextMeshProUGUI currentLevelDescription;

    [Tooltip("Icon showing the roon's current level")]
    [SerializeField]
    private Image currentLevelIcon;

    [Tooltip("Next level description")]
    [SerializeField]
    private TextMeshProUGUI nextLevelDescription;

    [Tooltip("Icon showing the roon's next level")]
    [SerializeField]
    private Image nextLevelIcon;

    [Tooltip("Next level prestige cost")]
    [SerializeField]
    private TextMeshProUGUI prestigeCost;

    [Tooltip("Selector for icon display for roons by level")]
    [SerializeField]
    public RoonLevelIconSelector roonLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified roon
    /// </summary>
    /// <param name="roon">Roon to describe</param>
    public void UpdateDescription(Roon roon)
    {
        icon.sprite = roon.GetIcon();
        currentLevelDescription.text = roon.GetLevelDescription(roon.GetLevel());
        currentLevelIcon.sprite = roonLevelIconSelector.GetIcon(roon.GetLevel());
        nextLevelDescription.text = roon.GetLevelDescription(roon.GetLevel() + 1);
        nextLevelIcon.sprite = roonLevelIconSelector.GetIcon(roon.GetLevel() + 1);
        prestigeCost.text = roon.GetPrestigeCostToLevelUp().ToString();
    }
}