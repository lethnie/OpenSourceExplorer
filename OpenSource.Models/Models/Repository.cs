using System;
using System.Collections.Generic;

namespace OpenSource.GitHub.Models
{
    public class Repository
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public int ForksCount { get; set; }
        public int StarsCount { get; set; }
        public string PrimaryLanguage { get; set; }
        /// <summary>
        /// List of top MAX_LANGUAGES_COUNT languages most used in repository.
        /// </summary>
        public List<string> Languages { get; set; }
        public int LanguagesTotalCount { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int HelpWantedIssuesCount { get; set; }
        public int GoodFirstIssuesCount { get; set; }
    }
}
