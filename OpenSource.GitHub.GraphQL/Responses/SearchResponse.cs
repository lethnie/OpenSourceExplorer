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
    }

    internal class ArrayResult
    {
        public int TotalCount { get; set; }
    }
}
