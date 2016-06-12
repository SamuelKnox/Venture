using UnityEngine;

public abstract class Rune : MonoBehaviour
{
    [Tooltip("Type of rune")]
    [SerializeField]
    private RuneType runeType;

    /// <summary>
    /// Gets the type of this rune
    /// </summary>
    /// <returns>Rune Type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }
}