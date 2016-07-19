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

    [Tooltip("Level 1 sprite")]
    [SerializeField]
    private Sprite level1;

    [Tooltip("Level 2 sprite")]
    [SerializeField]
    private Sprite level2;

    [Tooltip("Level 3 sprite")]
    [SerializeField]
    private Sprite level3;

    [Tooltip("Level 4 sprite")]
    [SerializeField]
    private Sprite level4;

    [Tooltip("Level 5 sprite")]
    [SerializeField]
    private Sprite level5;

    [Tooltip("Level 6 sprite")]
    [SerializeField]
    private Sprite level6;

    [Tooltip("Level 7 sprite")]
    [SerializeField]
    private Sprite level7;

    [Tooltip("Level 8 sprite")]
    [SerializeField]
    private Sprite level8;

    [Tooltip("Level 9 sprite")]
    [SerializeField]
    private Sprite level9;

    [Tooltip("Level 10 sprite")]
    [SerializeField]
    private Sprite level10;

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
            switch (rune.GetLevel())
            {
                case 1:
                    level.sprite = level1;
                    break;
                case 2:
                    level.sprite = level2;
                    break;
                case 3:
                    level.sprite = level3;
                    break;
                case 4:
                    level.sprite = level4;
                    break;
                case 5:
                    level.sprite = level5;
                    break;
                case 6:
                    level.sprite = level6;
                    break;
                case 7:
                    level.sprite = level7;
                    break;
                case 8:
                    level.sprite = level8;
                    break;
                case 9:
                    level.sprite = level9;
                    break;
                case 10:
                    level.sprite = level10;
                    break;
                default:
                    Debug.LogError("Invalid level used!", gameObject);
                    return;
            }
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