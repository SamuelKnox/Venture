using System;
using UnityEngine;

[Serializable]
public class RuneLevelIconSelector
{
    [Tooltip("Icons displaying each level for runes")]
    [SerializeField]
    private RuneLevelIcon[] icons = new RuneLevelIcon[Rune.GetMaxLevel() - Rune.GetMinLevel() + 1];

    /// <summary>
    /// Creates the rune level icon selector
    /// </summary>
    public RuneLevelIconSelector()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i] = new RuneLevelIcon(i + Rune.GetMinLevel());
        }
    }

    /// <summary>
    /// Gets the icon for runes at the specified level
    /// </summary>
    /// <param name="level">Level of rune</param>
    public Sprite GetIcon(int level)
    {
        return icons[level - Rune.GetMinLevel()].GetIcon();
    }

    [Serializable]
    private class RuneLevelIcon
    {
#pragma warning disable 0414
        [HideInInspector]
        [SerializeField]
        private string name;
#pragma warning restore 0414

        private const string NamePrefix = "Level ";

        [Tooltip("Icon displaying rune's level")]
        [SerializeField]
        private Sprite icon;

        /// <summary>
        /// Creates the Rune Level Icon
        /// </summary>
        /// <param name="level">Rune's level</param>
        public RuneLevelIcon(int level)
        {
            name = NamePrefix + level;
        }

        /// <summary>
        /// Gets the icon for a rune at this level
        /// </summary>
        /// <returns>Rune level icon</returns>
        public Sprite GetIcon()
        {
            return icon;
        }
    }
}