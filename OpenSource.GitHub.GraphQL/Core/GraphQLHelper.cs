using System;
using System.Text;

namespace OpenSource.GitHub.GraphQL
{
    internal static class GraphQLHelper
    {
        private const string CURSOR_PREFIX = "cursor:";

        /// <summary>
        /// Gets cursor for given index.
        /// </summary>
        /// <param name="index">Element index.</param>
        /// <returns>Cursor value.</returns>
        public static string GetCursor(int index)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CURSOR_PREFIX}{index}"));
        }
    }
}
