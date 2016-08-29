using UnityEngine;
using UnityEngine.UI;

public class RuneSocket : MonoBehaviour
{
    [Tooltip("Type of rune this tab represents")]
    [SerializeField]
    private RuneType runeType;

    [Tooltip("Icon of rune in socket")]
    [SerializeField]
    private Image icon;

    /// <summary>
    /// Gets the type of rune represented by this tab
    /// </summary>
    /// <returns>Type of rune</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }

    /// <summary>
    /// Gets the image holding this rune socket's rune's icon
    /// </summary>
    /// <returns>Rune icon holder</returns>
    public Image GetIcon()
    {
        return icon;
    }
}