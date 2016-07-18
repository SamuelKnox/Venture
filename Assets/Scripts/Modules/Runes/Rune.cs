using System;
using UnityEngine;

[Serializable]
public abstract class Rune : Item
{
    private const int PrestigeCostToLevelUp = 1;
    private const int MinLevel = 1;
    private const int Maxlevel = 10;
    private const string RuneDescriptionNamePrefix = "Level ";

    [Tooltip("Rune level")]
    [SerializeField]
    [Range(1, 10)]
    protected int level = 1;

    [Tooltip("Rune descriptions by level")]
    [SerializeField]
    private RuneLevelDescription[] descriptionsByLevel = new RuneLevelDescription[Maxlevel - MinLevel + 1];

    [Tooltip("Description used for when there are no more upgrades available")]
    [SerializeField]
    private string noMoreUpgradesDescription;

    void Awake()
    {
        if (string.IsNullOrEmpty(noMoreUpgradesDescription))
        {
            Debug.LogWarning("There is no description for when no more upgrades are available.", gameObject);
        }
        for (int i = 0; i < descriptionsByLevel.Length; i++)
        {
            if (string.IsNullOrEmpty(descriptionsByLevel[i].GetDescription()))
            {
                Debug.LogWarning("There is no description for " + name + "'s level " + (i + MinLevel));
            }
        }
    }

    void OnValidate()
    {
        Array.Resize(ref descriptionsByLevel, Maxlevel - MinLevel + 1);
        for (int i = 0; i < descriptionsByLevel.Length; i++)
        {
            descriptionsByLevel[i].SetName(RuneDescriptionNamePrefix + (i + 1));
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
    /// Gets the description for the rune at the specific level
    /// </summary>
    /// <param name="level">Level to get description for</param>
    /// <returns>Description</returns>
    public string GetDescriptionByLevel(int level)
    {
        int index = level - MinLevel;
        if (index > Maxlevel - MinLevel)
        {
            return noMoreUpgradesDescription;
        }
        return descriptionsByLevel[level - MinLevel].GetDescription();
    }

    /// <summary>
    /// Get cost in prestige to level up this rune
    /// </summary>
    /// <returns>Prestige cost</returns>
    public int GetPrestigeCostToLevelUp()
    {
        return PrestigeCostToLevelUp;
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
    public bool SetLevel(int level)
    {
        if (level < MinLevel || level > Maxlevel)
        {
            return false;
        }
        this.level = level;
        return true;
    }

    [Serializable]
    private class RuneLevelDescription
    {
        [HideInInspector]
        [SerializeField]
        private string name;

        [Tooltip("Description for level")]
        [SerializeField]
        [TextArea(1, 10)]
        private string description;

        /// <summary>
        /// Sets the name of the rune description
        /// </summary>
        /// <param name="name">Name to set</param>
        public void SetName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the description for this rune at the level
        /// </summary>
        /// <returns>Description</returns>
        public string GetDescription()
        {
            return description;
        }
    }
}