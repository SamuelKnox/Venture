using UnityEngine;

public static class ColorUtility
{
    private const float MinColorComponent = 0.001f;
    private const int ColorComponents = 4;

    /// <summary>
    /// Removes zeroes from a Color to prevent division of zero
    /// </summary>
    /// <param name="color">Color to remove zeroes from</param>
    /// <returns>Color without zero components</returns>
    public static Color GetMinColor(Color color)
    {
        for (int i = 0; i < ColorComponents; i++)
        {
            color[i] = Mathf.Max(MinColorComponent, color[i]);
        }
        return color;
    }
}