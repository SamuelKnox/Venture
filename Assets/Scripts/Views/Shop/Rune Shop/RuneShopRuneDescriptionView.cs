using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuneShopRuneDescriptionView : MonoBehaviour
{
    private const string CostPrefix = "Cost: ";

    [Tooltip("Container for when there is a rune to describe")]
    [SerializeField]
    private Transform runeDescriptionContainer;

    [Tooltip("Icon for rune")]
    [SerializeField]
    private Image runeIcon;

    [Tooltip("Name of rune")]
    [SerializeField]
    private TextMeshProUGUI runeName;

    [Tooltip("Level of rune")]
    [SerializeField]
    private Image level;

    [Tooltip("General rune description")]
    [SerializeField]
    private TextMeshProUGUI generalDescription;

    [Tooltip("Specific rune description")]
    [SerializeField]
    private TextMeshProUGUI specificDescription;

    [Tooltip("Text to display cost of rune")]
    [SerializeField]
    private TextMeshProUGUI costDescription;

    [Tooltip("Selector for icon display for runes by level")]
    [SerializeField]
    public RuneLevelIconSelector runeLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void SetDescription(Rune rune)
    {
        if (!rune)
        {
            Debug.LogError("Rune is null!", gameObject);
            return;
        }
        runeDescriptionContainer.gameObject.SetActive(true);
        runeIcon.sprite = rune.GetIcon();
        runeName.text = rune.name;
        level.sprite = runeLevelIconSelector.GetIcon(rune.GetLevel());
        generalDescription.text = rune.GetDescription();
        specificDescription.text = rune.GetLevelDescription(rune.GetLevel());
        var item = rune.GetComponent<Item>();
        if (!item)
        {
            Debug.LogError("Could not find Item!", gameObject);
            return;
        }
        costDescription.text = CostPrefix + item.GetCost();
    }
}