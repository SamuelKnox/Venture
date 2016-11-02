namespace CustomUnityLibrary
{
    using UnityEngine;

    /// <summary>
    /// Extension methods for the Color class
    /// </summary>
    public static class ColorExtensions
    {
        private const int ColorComponents = 4;

        /// <summary>
        /// Divides a color by another color, component-wise
        /// </summary>
        /// <param name="color">Color to divide</param>
        /// <param name="other">Color to divide by</param>
        /// <returns>New color</returns>
        public static Color DivideBy(this Color color, Color other)
        {
            for (int i = 0; i < ColorComponents; i++)
            {
                if (other[i] == 0.0f)
                {
                    Debug.LogError("Cannot divide by a color with a 0 component!" + other);
                    continue;
                }
                color[i] /= other[i];
            }
            return color;
        }
    }
}