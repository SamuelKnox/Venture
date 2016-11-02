namespace CustomUnityLibrary
{
    /// <summary>
    /// These are extensions for Floats
    /// </summary>
    public static class FloatExtensions
    {
        private const string FloatToPercentRegex = "{0:0%}";

        /// <summary>
        /// Converts a float to its percentage in string form
        /// </summary>
        /// <param name="percent">Percent to convert</param>
        /// <returns>Percentage as a string</returns>
        public static string ToPercentage(this float percent)
        {
            return string.Format(FloatToPercentRegex, percent);
        }
    }
}