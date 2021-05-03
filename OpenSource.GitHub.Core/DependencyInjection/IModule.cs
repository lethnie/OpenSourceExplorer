using Microsoft.Extensions.DependencyInjection;
using OpenSource.GitHub.Core.Configuration;

namespace OpenSource.GitHub.Core.DependencyInjection
{
    /// <summary>
    /// Dependency injection module.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Configures collection of service descriptors.
        /// </summary>
        /// <param name="services">Collection of configured services.</param>
        /// <param name="configuration">Application configuration.</param>
        void ConfigureServices(IServiceCollection services, IOpenSourceConfiguration configuration);
    }
}
