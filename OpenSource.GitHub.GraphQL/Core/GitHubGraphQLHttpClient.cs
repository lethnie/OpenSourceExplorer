using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    internal class GitHubGraphQLHttpClient : IDisposable
    {
        private readonly GraphQLHttpClient _graphQLClient;

        public GitHubGraphQLHttpClient(GraphQLOptions configuration)
        {
            _graphQLClient = new GraphQLHttpClient(configuration.ApiUrl, new NewtonsoftJsonSerializer());
            _graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {configuration.AccessToken}");
        }

        public async Task<TResponse> Query<TResponse>(string query, object variables = null)
        {
            var issuesRequest = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };
            var result = await this._graphQLClient.SendQueryAsync<TResponse>(issuesRequest);

            if (result.Errors != null && result.Errors.Length > 0)
            {
                throw new GraphQLException(result.Errors);
            }

            return result.Data;
        }

        public void Dispose()
        {
            this._graphQLClient.Dispose();
        }
    }
}
