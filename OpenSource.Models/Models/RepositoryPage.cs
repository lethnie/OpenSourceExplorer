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
    }
}
