using OpenSource.GitHub.Models;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    /// <summary>
    /// Service to load repositories data.
    /// </summary>
    public interface IRepositoryService
    {
        /// <summary>
        /// Gets list of repositories according to search parameters.
        /// </summary>
        /// <param name="parameters">Search and pagination parameters.</param>
        /// <returns>Page containing list of repositories.</returns>
        Task<RepositoryPage> GetRepositoriesAsync(RepositoryParameters parameters);

        /// <summary>
        /// Gets random repository according to search parameters.
        /// </summary>
        /// <param name="parameters">Search parameters.</param>
        /// <returns>Random repository.</returns>
        Task<Repository> GetRandomRepositoryAsync(RepositoryParameters parameters);
    }
}
