using System.Collections.Generic;

namespace OpenSource.GitHub.Core.DependencyInjection
{
    /// <summary>
    /// Dependency injection modules settings.
    /// </summary>
    public class ModulesOptions
    {
        /// <summary>
        /// List of modules full types names.
        /// </summary>
        public List<string> Modules { get; set; }
    }
}
