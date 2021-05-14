using OpenSource.GitHub.Core;
using OpenSource.GitHub.Core.Cache;
using OpenSource.GitHub.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    internal class RepositoryService : IRepositoryService
    {
        /// <summary>
        /// Max number of languages to load for a repository.
        /// </summary>
        private const int MAX_LANGUAGES_COUNT = 10;
        /// <summary>
        /// Github API limitation.
        /// TODO: User filters as a workaround.
        /// </summary>
        private const int MAX_REPOSITORIES_COUNT = 1000;
        private const int TOTAL_COUNT_CACHE_EXPIRATION_TIME_IN_MINUTES = 60;

        private readonly GitHubGraphQLHttpClient _client;
        private readonly ICacheService _cacheService;

        public RepositoryService(GitHubGraphQLHttpClient client, ICacheService cacheService)
        {
            this._client = client;
            this._cacheService = cacheService;
        }

        /// <summary>
        /// Gets list of repositories according to search parameters.
        /// </summary>
        /// <param name="parameters">Search and pagination parameters.</param>
        /// <returns>Page containing list of repositories.</returns>
        public async Task<RepositoryPage> GetRepositoriesAsync(RepositoryParameters parameters)
        {
            Ensure.ArgumentNotNull(parameters, nameof(parameters));

            var searchResult = await this._client.Query<QueryData>(RepositoryQueries.REPOSITORY_SEARCH_QUERY,
                new
                {
                    query = RepositorySearchToQuery(parameters),
                    first = parameters.PageSize * parameters.PageNumber > MAX_REPOSITORIES_COUNT ?
                        parameters.PageSize - (parameters.PageSize * parameters.PageNumber - MAX_REPOSITORIES_COUNT):
                        parameters.PageSize,
                    after = GraphQLHelper.GetAfterCursorForPage(parameters.PageSize.Value, parameters.PageNumber.Value),
                    languageCount = MAX_LANGUAGES_COUNT
                });

            var result = new RepositoryPage();
            result.TotalCount = searchResult.Search.RepositoryCount > MAX_REPOSITORIES_COUNT ?
                MAX_REPOSITORIES_COUNT : searchResult.Search.RepositoryCount;
            result.HasNextPage = searchResult.Search.PageInfo.HasNextPage;
            result.HasPreviousPage = searchResult.Search.PageInfo.HasPreviousPage;
            result.StartCursor = searchResult.Search.PageInfo.StartCursor;
            result.EndCursor = searchResult.Search.PageInfo.EndCursor;
            result.Repositories = searchResult.Search.Edges.Select(e => ConvertEdgeToRepository(e)).ToList();

            return result;
        }

        /// <summary>
        /// Gets random repository according to search parameters.
        /// </summary>
        /// <param name="parameters">Search parameters.</param>
        /// <returns>Random repository.</returns>
        public async Task<Repository> GetRandomRepositoryAsync(RepositoryParameters parameters)
        {
            Ensure.ArgumentNotNull(parameters, nameof(parameters));

            var query = RepositorySearchToQuery(parameters);

            // cache total count value to avoid excessive API requests
            var cacheKey = $"TotalCount-{query}";
            var cachedValue = _cacheService.GetValue<int?>(cacheKey);
            int repositoryCount;
            if (cachedValue.HasValue)
            {
                repositoryCount = cachedValue.Value;
            }
            else
            {
                var totalCountResult = await this._client.Query<QueryData>(RepositoryQueries.REPOSITORIES_COUNT_QUERY,
                    new { query = query });
                repositoryCount = totalCountResult.Search.RepositoryCount;
                _cacheService.SetValue(cacheKey, repositoryCount, TimeSpan.FromMinutes(TOTAL_COUNT_CACHE_EXPIRATION_TIME_IN_MINUTES));
            }

            if (repositoryCount == 0)
            {
                return null;
            }

            var searchResult = await this._client.Query<QueryData>(RepositoryQueries.REPOSITORY_SEARCH_QUERY,
                new
                {
                    query = RepositorySearchToQuery(parameters),
                    first = 1,
                    after = GraphQLHelper.GetRandomCursor(Math.Min(repositoryCount, MAX_REPOSITORIES_COUNT)),
                    languageCount = MAX_LANGUAGES_COUNT
                });

            var repository = searchResult.Search.Edges.FirstOrDefault();
            if (repository == null)
            {
                return null;
            }

            return ConvertEdgeToRepository(repository);
        }

        private Repository ConvertEdgeToRepository(Edge edge)
        {
            return new Repository
            {
                Key = edge.Cursor,
                Name = edge.Node.Name,
                Owner = edge.Node.Owner.Login,
                Url = edge.Node.Url,
                Description = edge.Node.Description,
                PrimaryLanguage = edge.Node.PrimaryLanguage?.Name,
                CreatedDate = edge.Node.CreatedAt,
                UpdatedDate = edge.Node.UpdatedAt,
                StarsCount = edge.Node.StargazerCount,
                ForksCount = edge.Node.ForkCount,
                LanguagesTotalCount = edge.Node.Languages.TotalCount,
                Languages = edge.Node.Languages.Nodes.Select(l => l.Name).ToList(),
                HelpWantedIssuesCount = edge.Node.HelpWantedIssues.TotalCount,
                GoodFirstIssuesCount = edge.Node.GoodFirstIssues.TotalCount
            };
        }

        private string RepositorySearchToQuery(RepositoryParameters search)
        {
            string query = string.Empty;
            if (!string.IsNullOrEmpty(search.Text))
            {
                query += search.Text;
            }
            if (!string.IsNullOrEmpty(search.Language))
            {
                query += $" language:{search.Language}";
            }
            if (search.LastUpdateAfter.HasValue)
            {
                query += $" pushed:>={search.LastUpdateAfter.Value.ToString("yyyy-MM-dd")}";
            }
            if (search.HasGoodFirstIssues.HasValue && search.HasGoodFirstIssues.Value)
            {
                query += $" good-first-issues:>0";
            }
            if (search.HasHelpWantedIssues.HasValue && search.HasHelpWantedIssues.Value)
            {
                query += $" help-wanted-issues:>0";
            }
            if (search.MinNumberOfStars.HasValue)
            {
                query += $" stars:>{search.MinNumberOfStars.Value}";
            }
            // TODO: setting up sorting in UI
            query += " sort:stars-desc";

            return query;
        }
    }
}
