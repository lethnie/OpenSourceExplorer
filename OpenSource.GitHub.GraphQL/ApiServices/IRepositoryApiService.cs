using OpenSource.GitHub.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    internal interface IRepositoryApiService
    {
        Task<int> GetRepositoryStarsCount(RepositoryParameters parameters, int index);
        Task<List<Repository>> GetRepositoriesAsync(RepositoryParameters parameters, int from, int count);
        Task<int> GetRepositoriesCount(RepositoryParameters parameters);
    }
}