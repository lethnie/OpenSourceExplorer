using System.Collections.Generic;

namespace OpenSource.GitHub.GraphQL
{
    internal class QueryData
    {
        public Search Search { get; set; }
    }

    internal class Search
    {
        public List<Edge> Edges { get; set; }
        public int RepositoryCount { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    internal class PageInfo
    {
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    internal class ArrayResult
    {
        public int TotalCount { get; set; }
    }
}
