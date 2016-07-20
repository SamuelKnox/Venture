using System;
using UnityEngine;

[Serializable]
public abstract class Rune : Item
{
    private const int MinLevel = 1;
    private const int Maxlevel = 10;

    [Tooltip("Rune level")]
    [SerializeField]
    [Range(MinLevel, Maxlevel)]
    protected int level = 1;

    [Tooltip("Attributes for rune at each level")]
    [SerializeField]
    private RuneLevelAttributes[] attributesByLevel = new RuneLevelAttributes[Maxlevel - MinLevel + 1];

    [Tooltip("Description used for when there are no more upgrades available")]
    [SerializeField]
    [TextArea(1, 5)]
    private string noMoreUpgradesDescription;

    void Awake()
    {
        if (string.IsNullOrEmpty(noMoreUpgradesDescription))
        {
            Debug.LogWarning("There is no description for when no more upgrades are available.", gameObject);
        }
        for (int i = 0; i < attributesByLevel.Length; i++)
        {
            if (string.IsNullOrEmpty(attributesByLevel[i].GetDescription()))
            {
                Debug.LogWarning("There is no description for " + name + "'s level " + (i + MinLevel));
            }
        }
    }

    void OnValidate()
    {
        Array.Resize(ref attributesByLevel, Maxlevel - MinLevel + 1);
    }

    void Reset()
    {
        for (int i = 0; i < attributesByLevel.Length; i++)
        {
            attributesByLevel[i].Initialize(i + MinLevel);
        }
    }


    /// <summary>
    /// Attaches a rune to equipment
    /// </summary>
    /// <param name="equipment">Equipment rune is being attached to</param>
    public abstract void Activate(Equipment equipment);

    /// <summary>
    /// Detaches a rune from equipment
    /// </summary>
    /// <param name="equipment">Equipment rune is being detached from</param>
    public abstract void Deactivate(Equipment equipment);

    /// <summary>
    /// Gets the type of this rune
    /// </summary>
    /// <returns>Rune Type</returns>
    public abstract RuneType GetRuneType();

    /// <summary>
    /// Gets the minimum level allowed for this rune
    /// </summary>
    /// <returns>Min level</returns>
    public static int GetMinLevel()
    {
        return MinLevel;
    }

    /// <summary>
    /// Gets the maximum level allowed for this rune
    /// </summary>
    /// <returns>Max level</returns>
    public static int GetMaxLevel()
    {
        return Maxlevel;
    }

    /// <summary>
    /// Gets the base value for the rune
    /// </summary>
    /// <returns>Base value</returns>
    public float GetBaseValue()
    {
        return attributesByLevel[level - MinLevel].GetBaseValue();
    }

    /// <summary>
    /// Gets the special value for the rune
    /// </summary>
    /// <returns>Special value</returns>
    public float GetSpecialValue()
    {
        return attributesByLevel[level - MinLevel].GetSpecialValue();
    }

    /// <summary>
    /// Gets the description for the rune at the specific level
    /// </summary>
    /// <param name="level">Level to get description for</param>
    /// <returns>Description</returns>
    public string GetLevelDescription(int level)
    {
        int index = level - MinLevel;
        if (index > Maxlevel - MinLevel)
        {
            return noMoreUpgradesDescription;
        }
        return attributesByLevel[level - MinLevel].GetDescription();
    }

    /// <summary>
    /// Get cost in prestige to level up this rune
    /// </summary>
    /// <returns>Prestige cost</returns>
    public int GetPrestigeCostToLevelUp()
    {
        return attributesByLevel[level - MinLevel].GetPrestigeCostToLevel();
    }

    /// <summary>
    /// Gets the rune's level
    /// </summary>
    /// <returns>Rune level</returns>
    public int GetLevel()
    {
        return level;
    }

    /// <summary>
    /// Sets the rune's level
    /// </summary>
    /// <param name="level">Level to set</param>
    /// <returns>Whether or not the rune's level was successfully set</returns>
    public void SetLevel(int level)
    {
        if (level < MinLevel || level > Maxlevel)
        {
            Debug.LogError("Attempting to set " + name + " to an invalid level!", gameObject);
            return;
        }
        this.level = level;
    }

    [Serializable]
    private class RuneLevelAttributes
    {
#pragma warning disable 0414
        [HideInInspector]
        [SerializeField]
        private string name;
#pragma warning restore 0414

        private const string RuneDescriptionNamePrefix = "Level ";

        [Tooltip("Description for level")]
        [SerializeField]
        [TextArea(1, 5)]
        private string description;

        [Tooltip("Value used for base attribute")]
        [SerializeField]
        [Range(0.0f, 100.0f)]
        private float baseValue;

        [Tooltip("Value used for special attribute")]
        [SerializeField]
        [Range(0.0f, 100.0f)]
        private float specialValue;

        [Tooltip("Prestige cost to level up from the previous level")]
        [SerializeField]
        [Range(1, 5)]
        private int prestigeCost;

        /// <summary>
        /// Sets the default values for the rune, such as name, prestige cost, and effect values
        /// </summary>
        /// <param name="level">Rune level</param>
        public void Initialize(int level)
        {
            name = RuneDescriptionNamePrefix + level;
            switch (level)
            {
                case 1:
                    prestigeCost = 1;
                    baseValue = 1.0f;
                    specialValue = 0.0f;
                    break;
                case 2:
                    prestigeCost = 1;
                    baseValue = 2.0f;
                    specialValue = 0.0f;
                    break;
                case 3:
                    prestigeCost = 2;
                    baseValue = 3.0f;
                    specialValue = 0.0f;
                    break;
                case 4:
                    prestigeCost = 1;
                    baseValue = 3.0f;
                    specialValue = 1.0f;
                    break;
                case 5:
                    prestigeCost = 1;
                    baseValue = 4.0f;
                    specialValue = 1.0f;
                    break;
                case 6:
                    prestigeCost = 2;
                    baseValue = 5.0f;
                    specialValue = 1.0f;
                    break;
                case 7:
                    prestigeCost = 1;
                    baseValue = 5.0f;
                    specialValue = 2.0f;
                    break;
                case 8:
                    prestigeCost = 1;
                    baseValue = 6.0f;
                    specialValue = 2.0f;
                    break;
                case 9:
                    prestigeCost = 3;
                    baseValue = 7.0f;
                    specialValue = 2.0f;
                    break;
                case 10:
                    prestigeCost = 0;
                    baseValue = 10.0f;
                    specialValue = 4.0f;
                    break;
                default:
                    Debug.LogError("An invalid level was provided!");
                    return;
            }
        }

        /// <summary>
        /// Gets the description for this rune at the level
        /// </summary>
        /// <returns>Description</returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Gets the base value for the rune at its level
        /// </summary>
        /// <returns>Base value</returns>
        public float GetBaseValue()
        {
            return baseValue;
        }

        /// <summary>
        /// Gets the special value for the rune at its level
        /// </summary>
        /// <returns>Special value</returns>
        public float GetSpecialValue()
        {
            return specialValue;
        }

        /// <summary>
        /// Gets the cost in prestige to level up this rune to the next level
        /// </summary>
        /// <returns></returns>
        public int GetPrestigeCostToLevel()
        {
            return prestigeCost;
        }
    }
}