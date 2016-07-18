using TMPro;
using UnityEngine;

public class LevelUpRuneDescriptionView : MonoBehaviour
{
    private const string CurrentAbilityDecription = "Current Ability:";
    private const string NextAbilityDescription = "Next Ability";

    [Tooltip("Text used to display the detailed rune description")]
    [SerializeField]
    private TextMeshProUGUI description;

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void UpdateDescription(Rune rune)
    {
        description.text = rune.GetDescription();
        description.text += StringUtility.NewLine();
        description.text += CurrentAbilityDecription;
        description.text += StringUtility.NewLine();
        description.text += NextAbilityDescription;
        description.text += rune.GetDescriptionByLevel(rune.GetLevel());
    }
}