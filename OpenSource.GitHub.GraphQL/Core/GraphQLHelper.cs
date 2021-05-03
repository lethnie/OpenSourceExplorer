﻿using System;
using System.Text;

namespace OpenSource.GitHub.GraphQL
{
    internal static class GraphQLHelper
    {
        /// <summary>
        /// Calculates cursor identifier for requested page.
        /// </summary>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Number of requested page.</param>
        /// <returns>Cursor identifier of the last value on previous page, null if current page is the first page.</returns>
        public static string GetAfterCursorForPage(int pageSize, int pageNumber)
        {
            int lastValueIndex = pageSize * (pageNumber - 1);
            if (lastValueIndex <= 0)
            {
                return null;
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"cursor:{lastValueIndex}"));
        }
    }
}
