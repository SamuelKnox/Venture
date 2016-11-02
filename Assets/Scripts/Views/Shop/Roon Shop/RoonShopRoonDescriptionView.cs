using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoonShopRoonDescriptionView : MonoBehaviour
{
    private const string CostPrefix = "Cost: ";

    [Tooltip("Container for when there is a roon to describe")]
    [SerializeField]
    private Transform roonDescriptionContainer;

    [Tooltip("Icon for roon")]
    [SerializeField]
    private Image roonIcon;

    [Tooltip("Name of roon")]
    [SerializeField]
    private TextMeshProUGUI roonName;

    [Tooltip("Level of roon")]
    [SerializeField]
    private Image level;

    [Tooltip("General roon description")]
    [SerializeField]
    private TextMeshProUGUI generalDescription;

    [Tooltip("Specific roon description")]
    [SerializeField]
    private TextMeshProUGUI specificDescription;

    [Tooltip("Text to display cost of roon")]
    [SerializeField]
    private TextMeshProUGUI costDescription;

    [Tooltip("Selector for icon display for roons by level")]
    [SerializeField]
    public RoonLevelIconSelector roonLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified roon
    /// </summary>
    /// <param name="roon">Roon to describe</param>
    public void SetDescription(Roon roon)
    {
        if (!roon)
        {
            Debug.LogError("Roon is null!", gameObject);
            return;
        }
        roonDescriptionContainer.gameObject.SetActive(true);
        roonIcon.sprite = roon.GetIcon();
        roonName.text = roon.name;
        level.sprite = roonLevelIconSelector.GetIcon(roon.GetLevel());
        generalDescription.text = roon.GetDescription();
        specificDescription.text = roon.GetLevelDescription(roon.GetLevel());
        var item = roon.GetComponent<Item>();
        if (!item)
        {
            Debug.LogError("Could not find Item!", gameObject);
            return;
        }
        costDescription.text = CostPrefix + item.GetCost();
    }
}