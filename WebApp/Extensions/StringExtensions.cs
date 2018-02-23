using System.Globalization;

namespace WebApp.Extensions
{
    /// <summary>
    /// Additional string extensions
    /// </summary>
    public static class StringExtensions
    {
        ////////////////////////////////////////////////////////////
        // Public Methods/Atributes
        ////////////////////////////////////////////////////////////

        /// <summary>
        /// Try to parse string to a double
        /// </summary>
        /// <param name="value">String to be parsed</param>
        /// <returns>double or null</returns>
        public static double? ToDouble(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return double.TryParse(value, NumberStyles.Float & ~NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result)
                ? result
                : (double?)null;
        }

        /// <summary>
        /// Try to parse string to a string
        /// </summary>
        /// <param name="value">String to be parsed</param>
        /// <param name="defaultResult">Default value if string can not to be parsed</param>
        /// <returns>parsed value or default</returns>
        public static bool ToBoolean(this string value, bool defaultResult = false)
        {
            return bool.TryParse(value, out var result) 
                ? result 
                : defaultResult;
        }
    }
}
