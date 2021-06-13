using OpenSource.GitHub.Core.Cache;
using OpenSource.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    internal class RepositoryApiService : IRepositoryApiService
    {
        private const int TOTAL_COUNT_CACHE_EXPIRATION_TIME_IN_MINUTES = 60;
        private const int STARS_COUNT_CACHE_EXPIRATION_TIME_IN_MINUTES = 10;
        /// <summary>
        /// Max number of languages to load for a repository.
        /// </summary>
        private const int MAX_LANGUAGES_COUNT = 10;

        private readonly GitHubGraphQLHttpClient _client;
        private readonly ICacheService _cacheService;

        public RepositoryApiService(GitHubGraphQLHttpClient client, ICacheService cacheService)
        {
            this._client = client;
            this._cacheService = cacheService;
        }

        public async Task<List<Repository>> GetRepositoriesAsync(RepositoryParameters parameters,
            int from, int count)
        {
            string query = RepositorySearchToQuery(parameters);
            var searchResult = await this._client.Query<QueryData>(RepositoryQueries.REPOSITORY_SEARCH_QUERY,
                new
                {
                    query = query,
                    first = count,
                    after = GraphQLHelper.GetCursor(from - 1),
                    languageCount = MAX_LANGUAGES_COUNT
                });
            return searchResult.Search.Edges.Select(e => ConvertEdgeToRepository(e)).ToList();
        }

        public async Task<int> GetRepositoryStarsCount(RepositoryParameters parameters, int index)
        {
            string query = RepositorySearchToQuery(parameters);
            // cache stars count value to avoid excessive API requests
            var cacheKey = $"StarsCount-{query}";
            var cachedValue = _cacheService.GetValue<int?>(cacheKey);
            int starsCount;
            if (cachedValue.HasValue)
            {
                starsCount = cachedValue.Value;
            }
            else
            {
                var searchResult = await this._client.Query<QueryData>(RepositoryQueries.REPOSITORY_STARS_QUERY,
                new
                {
                    query = query,
                    first = 1,
                    after = GraphQLHelper.GetCursor(index - 1)
                });
                if (searchResult.Search.Edges.Count == 0)
                {
                    throw new Exception("Repository is not found");
                }
                starsCount = searchResult.Search.Edges[0].Node.StargazerCount;
                _cacheService.SetValue(cacheKey, starsCount, TimeSpan.FromMinutes(STARS_COUNT_CACHE_EXPIRATION_TIME_IN_MINUTES));
            }
            return starsCount;
        }

        public async Task<int> GetRepositoriesCount(RepositoryParameters parameters)
        {
            string query = RepositorySearchToQuery(parameters);
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
            return repositoryCount;
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
            if (search.MinNumberOfStars.HasValue && search.MaxNumberOfStars.HasValue)
            {
                query += $" stars:{search.MinNumberOfStars.Value}..{search.MaxNumberOfStars.Value}";
            }
            else if (search.MinNumberOfStars.HasValue)
            {
                query += $" stars:>={search.MinNumberOfStars.Value}";
            }
            else if (search.MaxNumberOfStars.HasValue)
            {
                query += $" stars:<={search.MaxNumberOfStars.Value}";
            }
            if (search.MinCreated.HasValue && search.MaxCreated.HasValue)
            {
                query += $" created:{search.MinCreated.Value.ToString("yyyy-MM-ddTHH:mm:ss")}..{search.MaxCreated.Value.ToString("yyyy-MM-ddTHH:mm:ss")}";
            }
            else if (search.MinCreated.HasValue)
            {
                query += $" created:>={search.MinCreated.Value.ToString("yyyy-MM-ddTHH:mm:ss")}";
            }
            else if (search.MaxCreated.HasValue)
            {
                query += $" created:<={search.MaxCreated.Value.ToString("yyyy-MM-ddTHH:mm:ss")}";
            }
            // TODO: setting up sorting in UI
            query += " sort:stars-desc sort:updated-desc";

            return query;
        }
    }
}
