using System;
using UnityEngine;

[Serializable]
public class RoonLevelIconSelector
{
    [Tooltip("Icons displaying each level for roons")]
    [SerializeField]
    private RoonLevelIcon[] icons = new RoonLevelIcon[Roon.GetMaxLevel() - Roon.GetMinLevel() + 1];

    /// <summary>
    /// Creates the roon level icon selector
    /// </summary>
    public RoonLevelIconSelector()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i] = new RoonLevelIcon(i + Roon.GetMinLevel());
        }
    }

    /// <summary>
    /// Gets the icon for roons at the specified level
    /// </summary>
    /// <param name="level">Level of roon</param>
    public Sprite GetIcon(int level)
    {
        return icons[level - Roon.GetMinLevel()].GetIcon();
    }

    [Serializable]
    private class RoonLevelIcon
    {
#pragma warning disable 0414
        [HideInInspector]
        [SerializeField]
        private string name;
#pragma warning restore 0414

        private const string NamePrefix = "Level ";

        [Tooltip("Icon displaying roon's level")]
        [SerializeField]
        private Sprite icon;

        /// <summary>
        /// Creates the Roon Level Icon
        /// </summary>
        /// <param name="level">Roon's level</param>
        public RoonLevelIcon(int level)
        {
            name = NamePrefix + level;
        }

        /// <summary>
        /// Gets the icon for a roon at this level
        /// </summary>
        /// <returns>Roon level icon</returns>
        public Sprite GetIcon()
        {
            return icon;
        }
    }
}