namespace CustomUnityLibrary
{
    using UnityEngine;
    /// <summary>
    /// Utility class for Vector2s
    /// </summary>
    public static class Vector2Utility
    {
        /// <summary>
        /// Gets the bezier position of a curve at a particular time
        /// </summary>
        /// <param name="origin">Starting position of curve</param>
        /// <param name="midpoint">Influencing position on curve</param>
        /// <param name="destination">Ending position of curve</param>
        /// <param name="time">Time through curve, where 0.0 is the origin and 1.0 is the destination</param>
        /// <returns>Position on bezier curve at the provided time</returns>
        public static Vector2 Bezier(Vector2 origin, Vector2 midpoint, Vector2 destination, float time)
        {
            time = Mathf.Clamp01(time);
            return (1.0f - time) * ((1 - time) * origin + time * midpoint) + time * ((1 - time) * midpoint + time * destination);
        }
    }
}