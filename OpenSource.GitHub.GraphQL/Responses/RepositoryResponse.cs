using System;
using System.Collections.Generic;

namespace OpenSource.GitHub.GraphQL
{
    internal class Edge
    {
        public string Cursor { get; set; }
        public List<TextMatch> TextMatches { get; set; }
        public RepositoryNode Node { get; set; }
    }

    internal class RepositoryNode
    {
        public string Name { get; set; }
        public Owner Owner { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int ForkCount { get; set; }
        public int StargazerCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool HasIssuesEnabled { get; set; }
        public Languages Languages { get; set; }
        public PrimaryLanguage PrimaryLanguage { get; set; }
        public ArrayResult HelpWantedIssues { get; set; }
        public ArrayResult GoodFirstIssues { get; set; }
    }

    internal class PrimaryLanguage
    {
        public string Name { get; set; }
    }

    internal class Languages : ArrayResult
    {
        public List<LanguageNode> Nodes { get; set; }
    }

    internal class LanguageNode
    {
        public string Name { get; set; }
    }

    internal class Owner
    {
        public string Login { get; set; }
    }

    internal class TextMatch
    {
        public List<Highlight> Highlights { get; set; }
    }

    internal class Highlight
    {
        public string Text { get; set; }
        public int EndIndice { get; set; }
        public int BeginIndice { get; set; }
    }
}
