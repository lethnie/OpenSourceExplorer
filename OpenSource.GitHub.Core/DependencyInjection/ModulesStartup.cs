using Microsoft.Extensions.DependencyInjection;
using OpenSource.GitHub.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenSource.GitHub.Core.DependencyInjection
{
    /// <summary>
    /// Modules startup.
    /// </summary>
    public class ModulesStartup
    {
        private readonly IEnumerable<IModule> _modules;
        private readonly IOpenSourceConfiguration _configuration;

        /// <summary>
        /// Loading modules specified in configuration.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        public ModulesStartup(IOpenSourceConfiguration configuration)
        {
            Ensure.ArgumentNotNull(configuration, nameof(configuration));
            this._configuration = configuration;

            var options = configuration.Get<ModulesOptions>();

            this._modules = options.Modules.Select(moduleTypeName =>
            {
                var type = Type.GetType(moduleTypeName);

                if (type == null)
                {
                    throw new TypeLoadException($"Cannot load type \"{moduleTypeName}\"");
                }

                var module = (IModule)Activator.CreateInstance(type);
                return module;
            });
        }

        /// <summary>
        /// Configures services for all modules.
        /// </summary>
        /// <param name="services">Collection of configured services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            foreach (IModule module in this._modules)
            {
                module.ConfigureServices(services, this._configuration);
            }
        }
    }
}
