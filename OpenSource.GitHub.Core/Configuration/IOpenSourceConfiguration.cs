namespace OpenSource.GitHub.Core.Configuration
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public interface IOpenSourceConfiguration
    {
        /// <summary>
        /// Get configuration for a class.
        /// </summary>
        /// <typeparam name="TOptions">Configuration section description.</typeparam>
        /// <returns>Configuration.</returns>
        TOptions Get<TOptions>() where TOptions : class;
    }
}
