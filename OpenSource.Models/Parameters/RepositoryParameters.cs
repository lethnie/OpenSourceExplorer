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
        public int? MaxNumberOfStars { get; set; }
        public DateTime? MinCreated { get; set; }
        public DateTime? MaxCreated { get; set; }

        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }

        public RepositoryParameters Clone()
        {
            return new RepositoryParameters
            {
                Language = this.Language,
                MinCreated = this.MinCreated,
                MaxCreated = this.MaxCreated,
                HasGoodFirstIssues = this.HasGoodFirstIssues,
                HasHelpWantedIssues = this.HasHelpWantedIssues,
                LastUpdateAfter = this.LastUpdateAfter,
                MaxNumberOfStars = this.MaxNumberOfStars,
                MinNumberOfStars = this.MinNumberOfStars,
                Text = this.Text,
                PageNumber = this.PageNumber,
                PageSize = this.PageSize
            };
        }
    }
}
