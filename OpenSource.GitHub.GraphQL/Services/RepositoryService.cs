using OpenSource.GitHub.Core;
using OpenSource.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL
{
    internal class RepositoryService : IRepositoryService
    {
        /// <summary>
        /// Github API limitation.
        /// </summary>
        private const int MAX_REPOSITORIES_COUNT = 1000;
        /// <summary>
        /// Default time period to search for repositories with given number of stars.
        /// Set to half a year.
        /// Is used for a workaround for GitHub API limitation of 1000 results for a search.
        /// </summary>
        private const int DEFAULT_HOURS_SEARCH_PERIOD = 24 * 365 / 2;
        private static readonly Random Random = new Random();

        private readonly IRepositoryApiService _repositoryApiService;

        public RepositoryService(IRepositoryApiService repositoryApiService)
        {
            this._repositoryApiService = repositoryApiService;
        }

        /// <summary>
        /// Gets list of repositories according to search parameters.
        /// </summary>
        /// <param name="parameters">Search and pagination parameters.</param>
        /// <returns>Page containing list of repositories.</returns>
        public async Task<RepositoryPage> GetRepositoriesAsync(RepositoryParameters parameters)
        {
            Ensure.ArgumentNotNull(parameters, nameof(parameters));

            var result = new RepositoryPage();
            result.TotalCount = await this._repositoryApiService.GetRepositoriesCount(parameters);
            result.Repositories = new List<Repository>();

            var from = parameters.PageSize.Value * (parameters.PageNumber.Value - 1) + 1;
            var to = Math.Min(result.TotalCount, from + parameters.PageSize.Value - 1);
            var queries = await GetQueries(parameters, from, to, MAX_REPOSITORIES_COUNT);

            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.Repositories.AddRange(repositories);
            }

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

            // cache total count value to avoid excessive API requests
            int repositoryCount = await _repositoryApiService.GetRepositoriesCount(parameters);

            if (repositoryCount == 0)
            {
                return null;
            }

            var index = Random.Next(repositoryCount - 1);
            var queries = await GetQueries(parameters, index, index, MAX_REPOSITORIES_COUNT);
            var queryParams = queries.FirstOrDefault();

            if (queryParams == null)
            {
                throw new Exception("Unexpected exception occured. Try different parameters");
            }

            var repositories = await _repositoryApiService.GetRepositoriesAsync(queryParams.Parameters,
                queryParams.From, 1);

            return repositories.FirstOrDefault();
        }

        /// <summary>
        /// Workaround for GitHub repositories count limit.
        /// Splits requested data into batches by combination of stars count and creation date.
        /// Returns list of query parameters to request data from those batches.
        /// Sorting by stars count is applied, sorting by update/creation dates is not guaranteed.
        /// </summary>
        /// <param name="parameters">Parameters for a query without limitation.</param>
        /// <param name="from">Index of the first requested repository.</param>
        /// <param name="to">Index of the last requested repository.</param>
        /// <param name="max">Defined limit (1000; parametrized for unit testing).</param>
        /// <returns>List of query parameters considering limitation.</returns>
        internal async Task<List<QueryParams>> GetQueries(RepositoryParameters parameters,
            int from, int to, int max)
        {
            var queries = new List<QueryParams>();
            // if search period is inside limitation then just execute search as it is
            if (from <= max && to <= max)
            {
                queries.Add(new QueryParams(parameters.Clone(), from, Math.Min(to, max)));
                return queries;
            }

            while (to > max)
            {
                // find the number of stars for the last repository in the next batch of [max] repositories;
                // calculate the number of repositories in this batch with the number of stars greater than the last
                // to avoid calculating those repositories twice
                var starsCount = await _repositoryApiService.GetRepositoryStarsCount(parameters, max);
                var correctedParameters = parameters.Clone();
                correctedParameters.MinNumberOfStars = starsCount + 1;
                var repositoryCountShift = await _repositoryApiService.GetRepositoriesCount(correctedParameters);

                from = Math.Max(from - repositoryCountShift, 1);
                to -= repositoryCountShift;

                // calculate the number of repositories with the number of stars equal to the last one
                correctedParameters.MinNumberOfStars = starsCount;
                correctedParameters.MaxNumberOfStars = starsCount;
                var repositoryCount = await _repositoryApiService.GetRepositoriesCount(correctedParameters);

                // if the page we're looking for is inside current batch then start to compose correct search queries
                // that would return less than [max] repositories
                if (from <= repositoryCount)
                {
                    DateTime endDate = DateTime.Today.AddDays(1);
                    if (parameters.MaxCreated.HasValue && endDate > parameters.MaxCreated)
                    {
                        endDate = parameters.MaxCreated.Value;
                    }
                    DateTime startDate = endDate.AddHours(-DEFAULT_HOURS_SEARCH_PERIOD);
                    if (parameters.MinCreated.HasValue && startDate < parameters.MinCreated)
                    {
                        startDate = parameters.MinCreated.Value;
                    }
                    correctedParameters.MaxCreated = endDate;

                    // if batch size (repositoryCount) is greater than [max], divide a batch with equal
                    // stars count by created date (constant value, won't change during execution);
                    while (repositoryCount >= max && to >= max)
                    {
                        correctedParameters.MinCreated = parameters.MinCreated;
                        correctedParameters.MaxCreated = endDate;

                        // by default use one year period; if there are more than [max] repositories inside
                        // this period, then divide it by two and try again
                        var addCount = await _repositoryApiService.GetRepositoriesCount(correctedParameters);
                        var hours = DEFAULT_HOURS_SEARCH_PERIOD;

                        while (addCount > max)
                        {
                            startDate = endDate.AddHours(-hours);
                            correctedParameters.MinCreated = startDate;
                            addCount = await _repositoryApiService.GetRepositoriesCount(correctedParameters);
                            hours = (int)Math.Ceiling(hours / ((double)addCount/(double)max));
                        }

                        // if start of the page is inside current batch and the end is not,
                        // add query for the first part of the page to the result
                        if (from <= addCount)
                        {
                            queries.Add(new QueryParams(correctedParameters.Clone(), from, addCount));
                            from = addCount + 1;
                        }

                        // subtract processed amount of repositories
                        repositoryCount -= addCount;
                        from = Math.Max(1, from - addCount);
                        to -= addCount;

                        endDate = startDate.AddSeconds(-1);
                        startDate = endDate.AddHours(-DEFAULT_HOURS_SEARCH_PERIOD);
                        if (parameters.MinCreated.HasValue && startDate < parameters.MinCreated)
                        {
                            startDate = parameters.MinCreated.Value;
                        }
                    }
                    correctedParameters.MinCreated = parameters.MinCreated;
                    correctedParameters.MaxCreated = endDate;
                    queries.Add(new QueryParams(correctedParameters.Clone(), from,
                        Math.Min(to, repositoryCount)));
                    from = Math.Min(to, repositoryCount) + 1;
                    correctedParameters.MaxCreated = parameters.MaxCreated;
                }
                from = Math.Max(1, from - repositoryCount);
                to -= repositoryCount;

                // change query to go to the next batch
                parameters.MaxNumberOfStars = starsCount - 1;
            }
            if (to > 0)
            {
                queries.Add(new QueryParams(parameters.Clone(), from, to));
            }
            return queries;
        }

        internal class QueryParams
        {
            public QueryParams(RepositoryParameters parameters, int from, int to)
            {
                this.Parameters = parameters;
                this.From = from;
                this.To = to;
            }
            public RepositoryParameters Parameters { get; }
            public int From { get; }
            public int To { get; }
        }
    }
}
