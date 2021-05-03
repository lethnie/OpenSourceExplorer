using System;

namespace OpenSource.GitHub.Models
{
    public class RepositoryParameters
    {
        public string Language { get; set; }
        public string Text { get; set; }
        public bool? HasHelpWantedIssues { get; set; }
        public bool? HasGoodFirstIssues { get; set; }
        public DateTime? LastUpdateAfter { get; set; }
        public int? MinNumberOfStars { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
