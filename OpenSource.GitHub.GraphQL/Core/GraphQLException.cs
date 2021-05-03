using GraphQL;
using OpenSource.GitHub.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenSource.GitHub.GraphQL
{
    /// <summary>
    /// Represents errors that occur during GraphQL query execution.
    /// </summary>
    public class GraphQLException : Exception
    {
        /// <summary>
        /// GraphQL query execution error details.
        /// </summary>
        public List<GraphQLErrorData> Errors { get; }

        internal GraphQLException(GraphQLError[] errors) : base(GetMessage(errors))
        {
            this.Errors = errors.Select(e => new GraphQLErrorData(e)).ToList();
        }

        private static string GetMessage(GraphQLError[] errors)
        {
            Ensure.ArgumentNotNull(errors, nameof(errors));
            return string.Join(" ", errors.Select(e => e.Message));
        }
    }
}
