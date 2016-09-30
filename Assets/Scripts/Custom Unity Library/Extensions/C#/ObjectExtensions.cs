namespace CustomUnityLibrary
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks to see if an object contains a method
        /// </summary>
        /// <param name="source">Object to check</param>
        /// <param name="methodName">Name of method</param>
        /// <returns>Whether or not the object contains the method</returns>
        public static bool ContainsMethod(this object source, string methodName)
        {
            var type = source.GetType();
            return type.GetMethod(methodName) != null;
        }
    }
}