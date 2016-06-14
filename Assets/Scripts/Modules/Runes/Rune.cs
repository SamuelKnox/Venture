using UnityEngine;

[System.Serializable]
public class Rune : Item
{
    [Tooltip("Type of rune")]
    [SerializeField]
    private RuneType runeType;

    void OnValidate()
    {
        if (GetRuneTypes().Length > 0)
        {
            Debug.LogError("Runes cannot have rune sockets!", gameObject);
        }
    }

    /// <summary>
    /// Gets the type of this rune
    /// </summary>
    /// <returns>Rune Type</returns>
    public RuneType GetRuneType()
    {
        return runeType;
    }
}