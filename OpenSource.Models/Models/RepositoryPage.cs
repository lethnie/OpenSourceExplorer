using System.Collections.Generic;

namespace OpenSource.GitHub.Models
{
    public class RepositoryPage
    {
        public List<Repository> Repositories { get; set; }
        /// <summary>
        /// Total number of repositories that fit search parameters.
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Identifier of the first repository on page.
        /// </summary>
        public string StartCursor { get; set; }
        /// <summary>
        /// Identifier of the last repository on page.
        /// </summary>
        public string EndCursor { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
