using OpenSource.GitHub.Core;
using OpenSource.GitHub.Models;
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
        private readonly GitHubGraphQLHttpClient _client;

        public RepositoryService(GitHubGraphQLHttpClient client)
        {
            _client = client;
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
                    after = GraphQLHelper.GetAfterCursorForPage(parameters.PageSize, parameters.PageNumber),
                    languageCount = MAX_LANGUAGES_COUNT
                });

            var result = new RepositoryPage();
            result.TotalCount = searchResult.Search.RepositoryCount > MAX_REPOSITORIES_COUNT ?
                MAX_REPOSITORIES_COUNT : searchResult.Search.RepositoryCount;
            result.HasNextPage = searchResult.Search.PageInfo.HasNextPage;
            result.HasPreviousPage = searchResult.Search.PageInfo.HasPreviousPage;
            result.StartCursor = searchResult.Search.PageInfo.StartCursor;
            result.EndCursor = searchResult.Search.PageInfo.EndCursor;
            result.Repositories = searchResult.Search.Edges.Select(e =>
                new Repository
                {
                    Key = e.Cursor,
                    Name = e.Node.Name,
                    Owner = e.Node.Owner.Login,
                    Url = e.Node.Url,
                    Description = e.Node.Description,
                    PrimaryLanguage = e.Node.PrimaryLanguage?.Name,
                    CreatedDate = e.Node.CreatedAt,
                    UpdatedDate = e.Node.UpdatedAt,
                    StarsCount = e.Node.StargazerCount,
                    ForksCount = e.Node.ForkCount,
                    LanguagesTotalCount = e.Node.Languages.TotalCount,
                    Languages = e.Node.Languages.Nodes.Select(l => l.Name).ToList(),
                    HelpWantedIssuesCount = e.Node.HelpWantedIssues.TotalCount,
                    GoodFirstIssuesCount = e.Node.GoodFirstIssues.TotalCount
                }).ToList();

            return result;
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
