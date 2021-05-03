using Microsoft.Extensions.Configuration;
using OpenSource.GitHub.Core.Configuration;
using OpenSource.GitHub.Core.DependencyInjection;
using OpenSource.GitHub.GraphQL;

namespace OpenSource.Explorer.Configuration
{
    public class OpenSourceConfiguration : IOpenSourceConfiguration
    {
        private readonly IConfiguration _configuration;

        public OpenSourceConfiguration(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public TOptions Get<TOptions>() where TOptions : class
        {
            if (typeof(TOptions) == typeof(ModulesOptions))
            {
                return this._configuration.Get<TOptions>();
            }
            if (typeof(TOptions) == typeof(GraphQLOptions))
            {
                return this._configuration.GetSection("GraphQLApi").Get<TOptions>();
            }
            return this._configuration.Get<TOptions>();
        }
    }
}
