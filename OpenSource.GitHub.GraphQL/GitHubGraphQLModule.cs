using Microsoft.Extensions.DependencyInjection;
using OpenSource.GitHub.Core.Configuration;
using OpenSource.GitHub.Core.DependencyInjection;

namespace OpenSource.GitHub.GraphQL
{
    /// <summary>
    /// GraphQL client module.
    /// </summary>
    public class GitHubGraphQLModule : IModule
    {
        /// <summary>
        /// Configures collection of service descriptors.
        /// </summary>
        /// <param name="services">Collection of configured services.</param>
        /// <param name="configuration">Application configuration.</param>
        public void ConfigureServices(IServiceCollection services, IOpenSourceConfiguration configuration)
        {
            var graphQLConfiguration = configuration.Get<GraphQLOptions>();

            services.AddSingleton(_ => new GitHubGraphQLHttpClient(graphQLConfiguration));
            services.AddScoped<IRepositoryService, RepositoryService>();
        }
    }
}
