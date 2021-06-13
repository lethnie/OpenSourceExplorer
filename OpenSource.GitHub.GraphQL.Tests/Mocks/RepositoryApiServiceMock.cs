using OpenSource.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL.Tests.Mocks
{
    internal class RepositoryApiServiceMock : IRepositoryApiService
    {
        private List<Repository> _repositories;

        public RepositoryApiServiceMock(List<Repository> repositories)
        {
            this._repositories = repositories;
        }
        public Task<List<Repository>> GetRepositoriesAsync(RepositoryParameters parameters, int from, int count)
        {
            return Task.FromResult(this._repositories
                .Where(r => CheckRepository(r, parameters))
                .OrderByDescending(r => r.StarsCount)
                .ThenByDescending(r => r.UpdatedDate)
                .Skip(from - 1)
                .Take(count)
                .ToList());
        }

        public Task<int> GetRepositoriesCount(RepositoryParameters parameters)
        {
            return Task.FromResult(this._repositories
                .Where(r => CheckRepository(r, parameters))
                .Count());
        }

        public Task<int> GetRepositoryStarsCount(RepositoryParameters parameters, int index)
        {
            return Task.FromResult(this._repositories
                .Where(r => CheckRepository(r, parameters))
                .OrderByDescending(r => r.StarsCount)
                .ThenByDescending(r => r.UpdatedDate)
                .ToList()[index - 1].StarsCount);
        }

        private bool CheckRepository(Repository repository, RepositoryParameters parameters)
        {
            if (parameters.HasGoodFirstIssues.HasValue && parameters.HasGoodFirstIssues.Value)
            {
                if (repository.GoodFirstIssuesCount == 0)
                {
                    return false;
                }
            }
            if (parameters.HasHelpWantedIssues.HasValue && parameters.HasHelpWantedIssues.Value)
            {
                if (repository.HelpWantedIssuesCount == 0)
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(parameters.Language))
            {
                if (!repository.PrimaryLanguage.Equals(parameters.Language, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(parameters.Text))
            {
                if (!repository.Description.Contains(parameters.Text, StringComparison.CurrentCultureIgnoreCase) &&
                    !repository.Name.Contains(parameters.Text, StringComparison.CurrentCultureIgnoreCase) &&
                    !repository.Owner.Contains(parameters.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }
            }
            if (parameters.LastUpdateAfter.HasValue)
            {
                if (repository.UpdatedDate < parameters.LastUpdateAfter.Value)
                {
                    return false;
                }
            }
            if (parameters.MinCreated.HasValue)
            {
                if (repository.CreatedDate < parameters.MinCreated.Value)
                {
                    return false;
                }
            }
            if (parameters.MaxCreated.HasValue)
            {
                if (repository.CreatedDate > parameters.MaxCreated.Value)
                {
                    return false;
                }
            }
            if (parameters.MinNumberOfStars.HasValue)
            {
                if (repository.StarsCount < parameters.MinNumberOfStars.Value)
                {
                    return false;
                }
            }
            if (parameters.MaxNumberOfStars.HasValue)
            {
                if (repository.StarsCount > parameters.MaxNumberOfStars.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
