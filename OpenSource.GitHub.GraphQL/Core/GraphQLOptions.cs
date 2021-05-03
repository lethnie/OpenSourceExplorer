namespace OpenSource.GitHub.GraphQL
{
    /// <summary>
    /// GraphQL API configuration.
    /// </summary>
    public class GraphQLOptions
    {
        /// <summary>
        /// Access token for sending requests to API.
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// API default URL.
        /// </summary>
        public string ApiUrl { get; set; }
    }
}
