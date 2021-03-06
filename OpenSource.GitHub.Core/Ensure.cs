using System;

namespace OpenSource.GitHub.Core
{
    public static class Ensure
    {
        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="name">Argument name.</param>
        public static void ArgumentNotNull(object value, string name)
        {
            if (value != null) return;

            throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="value">Argument value.</param>
        /// <param name="name">Argument name.</param>
        public static void ArgumentNotNullOrEmptyString(string value, string name)
        {
            ArgumentNotNull(value, name);
            if (!string.IsNullOrWhiteSpace(value)) return;

            throw new ArgumentException("String cannot be empty", name);
        }
    }
}
