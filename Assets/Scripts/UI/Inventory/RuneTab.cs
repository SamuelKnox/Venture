using UnityEngine;
using UnityEngine.UI;

public class RuneTab : Tab
{
    [Tooltip("Type of rune this tab contains")]
    [SerializeField]
    private RuneType runeType;

    /// <summary>
    /// Gets the rune type for this category
    /// </summary>
    /// <returns>Rune type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }
}
