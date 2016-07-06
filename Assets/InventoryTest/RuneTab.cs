using UnityEngine;

public class RuneTab : MonoBehaviour
{
    [Tooltip("Type of rune this tab represents")]
    [SerializeField]
    private RuneType runeType;

    /// <summary>
    /// Gets the type of rune represented by this tab
    /// </summary>
    /// <returns>Type of rune</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }
}