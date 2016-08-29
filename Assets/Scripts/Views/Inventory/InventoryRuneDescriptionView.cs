using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRuneDescriptionView : MonoBehaviour
{
    [Tooltip("Container for when there is a rune to describe")]
    [SerializeField]
    private Transform runeDescriptionContainer;

    [Tooltip("Container for when there is no rune to describe")]
    [SerializeField]
    private Transform noRuneDescriptionContainer;

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

    [Tooltip("Icon for equipment rune is attached to")]
    [SerializeField]
    private Image equipmentIcon;

    [Tooltip("Name of equipment rune is attached to")]
    [SerializeField]
    private TextMeshProUGUI equipmentName;

    [Tooltip("Selector for icon display for runes by level")]
    [SerializeField]
    public RuneLevelIconSelector runeLevelIconSelector;

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    /// <param name="equipment">Equipment this rune is attached to, or null if there is none</param>
    public void SetDescription(Rune rune, Equipment equipment)
    {
        if (rune)
        {
            runeDescriptionContainer.gameObject.SetActive(true);
            noRuneDescriptionContainer.gameObject.SetActive(false);
            runeIcon.sprite = rune.GetIcon();
            runeName.text = rune.name;
            level.sprite = runeLevelIconSelector.GetIcon(rune.GetLevel());
            generalDescription.text = rune.GetDescription();
            specificDescription.text = rune.GetLevelDescription(rune.GetLevel());
            equipmentIcon.enabled = equipment;
            equipmentName.enabled = equipment;
            if (equipment)
            {
                equipmentIcon.sprite = equipment.GetIcon();
                equipmentName.text = equipment.name;
            }
        }
        else
        {
            runeDescriptionContainer.gameObject.SetActive(false);
            noRuneDescriptionContainer.gameObject.SetActive(true);
        }
    }
}