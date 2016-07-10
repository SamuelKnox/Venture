using TMPro;
using UnityEngine;

public class InventoryRuneDescriptionView : MonoBehaviour
{
    [Tooltip("Text used to display the detailed rune description")]
    [SerializeField]
    private TextMeshProUGUI description;

    [Tooltip("Text to be displayed if there is no rune to describe")]
    [SerializeField]
    private string noDescriptionText;

    void Awake()
    {
        if (string.IsNullOrEmpty(noDescriptionText))
        {
            Debug.LogWarning("There is no description for missing runes.", gameObject);
        }
    }

    /// <summary>
    /// Sets the full description for the specified rune
    /// </summary>
    /// <param name="rune">Rune to describe</param>
    public void SetDescription(Rune rune)
    {
        string descriptionText = rune ? rune.GetDescription() : noDescriptionText;
        description.text = descriptionText;
    }
}