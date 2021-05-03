using GraphQL;
using System.Collections.Generic;

namespace OpenSource.GitHub.GraphQL
{
    public class GraphQLErrorData
    {
        public string Message { get; }
        /// <summary>
        /// Path to a place that caused an error.
        /// </summary>
        public List<object> Path { get; }

        internal GraphQLErrorData(GraphQLError error)
        {
            this.Message = error.Message;
            this.Path = error.Path;
        }
    }
}
