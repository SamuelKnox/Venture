using UnityEngine;

public class RuneSocket : MonoBehaviour
{
    [Tooltip("Type of rune this socket can hold")]
    [SerializeField]
    private RuneType runeType;

    [Tooltip("Rune in this socket")]
    [SerializeField]
    private Rune rune;

    /// <summary>
    /// Gets the type of rune this socket can hold
    /// </summary>
    /// <returns>Rune socket type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }
}