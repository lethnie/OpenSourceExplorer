using NUnit.Framework;
using OpenSource.GitHub.GraphQL.Tests.Mocks;
using OpenSource.GitHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSource.GitHub.GraphQL.Tests
{
    public class RepositoryServiceTests
    {
        private RepositoryService _repositoryService;
        private RepositoryApiServiceMock _repositoryApiService;
        private List<Repository> _repositories;

        [SetUp]
        public void Setup()
        {
            this._repositories = GetRepositories();
            this._repositoryApiService = new RepositoryApiServiceMock(this._repositories);
            this._repositoryService = new RepositoryService(this._repositoryApiService);
        }

        /// <summary>
        /// Simple case: search for repositories from the first batch (no need for additional queries).
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_LessThanMax()
        {
            var parameters = new RepositoryParameters();
            int from = 0;
            int to = 3;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);
            Assert.AreEqual(1, queries.Count);
            Assert.AreEqual(from, queries[0].From);
            Assert.AreEqual(to, queries[0].To);
            Assert.AreEqual(parameters.MinNumberOfStars, queries[0].Parameters.MinNumberOfStars);
            Assert.AreEqual(parameters.MaxNumberOfStars, queries[0].Parameters.MaxNumberOfStars);
            Assert.AreEqual(parameters.MinCreated, queries[0].Parameters.MinCreated);
            Assert.AreEqual(parameters.MaxCreated, queries[0].Parameters.MaxCreated);
        }

        /// <summary>
        /// Search for repositories from two batches.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_TwoBatches()
        {
            var parameters = new RepositoryParameters();
            int from = 2;
            int to = 4;
            int max = 3;
            int starsCount = 3;
            parameters.MinNumberOfStars = starsCount;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            Assert.AreEqual(to - from + 1, result.Count);
            Assert.AreEqual(result.Count, result.Count(r => r.StarsCount == starsCount));
        }

        /// <summary>
        /// Search for repositories from another batch: additional query with number of stars.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SimpleStarsCountSkip()
        {
            var parameters = new RepositoryParameters();
            int max = 3;
            int from = 8;
            int to = from + max - 1;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);
            Assert.AreEqual(1, queries.Count);
            Assert.AreEqual(1, queries[0].From);
            Assert.AreEqual(max, queries[0].To);
            Assert.IsNull(parameters.MinNumberOfStars);
            Assert.AreEqual(parameters.MaxNumberOfStars, 2);
            Assert.IsNull(parameters.MinCreated);
            Assert.IsNull(parameters.MaxCreated);
        }

        /// <summary>
        /// Search for all repositories.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SelectAll()
        {
            var parameters = new RepositoryParameters();
            int from = 1;
            int to = this._repositories.Count;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            var repos = _repositories.OrderByDescending(r => r.StarsCount).ThenByDescending(r => r.UpdatedDate);

            Assert.AreEqual(to - from + 1, result.Count);
            CollectionAssert.AreEquivalent(this._repositories, result);
        }

        /// <summary>
        /// Search for all repositories with set min number of stars.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SelectAllWithMinStars()
        {
            var parameters = new RepositoryParameters();
            parameters.MinNumberOfStars = 2;
            var expectedResult = this._repositories.Where(r => r.StarsCount >= 2).ToList();
            int from = 1;
            int to = expectedResult.Count;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            var repos = _repositories.OrderByDescending(r => r.StarsCount).ThenByDescending(r => r.UpdatedDate);

            Assert.AreEqual(to - from + 1, result.Count);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        /// <summary>
        /// Search for all repositories with set max number of stars.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SelectAllWithMaxStars()
        {
            var parameters = new RepositoryParameters();
            parameters.MaxNumberOfStars = 2;
            var expectedResult = this._repositories.Where(r => r.StarsCount <= 2).ToList();
            int from = 1;
            int to = expectedResult.Count;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            var repos = _repositories.OrderByDescending(r => r.StarsCount).ThenByDescending(r => r.UpdatedDate);

            Assert.AreEqual(to - from + 1, result.Count);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        /// <summary>
        /// Search for all repositories with set min created date.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SelectAllWithMinCreated()
        {
            var parameters = new RepositoryParameters();
            parameters.MinCreated = DateTime.Now.AddMonths(-3);
            var expectedResult = this._repositories.Where(r => r.CreatedDate >= parameters.MinCreated).ToList();
            int from = 1;
            int to = expectedResult.Count;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            var repos = _repositories.OrderByDescending(r => r.StarsCount).ThenByDescending(r => r.UpdatedDate);

            Assert.AreEqual(to - from + 1, result.Count);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        /// <summary>
        /// Search for all repositories with set max created date.
        /// </summary>
        [Test]
        public async Task RepositoryService_GetQueries_SelectAllWithMaxCreated()
        {
            var parameters = new RepositoryParameters();
            parameters.MaxCreated = DateTime.Now.AddMonths(-3);
            var expectedResult = this._repositories.Where(r => r.CreatedDate <= parameters.MaxCreated).ToList();
            int from = 1;
            int to = expectedResult.Count;
            int max = 3;
            var queries = await this._repositoryService.GetQueries(parameters, from, to, max);

            var result = new List<Repository>();
            foreach (var query in queries)
            {
                var repositories = await this._repositoryApiService.GetRepositoriesAsync(query.Parameters,
                    query.From, (query.To - query.From + 1));
                result.AddRange(repositories);
            }

            var repos = _repositories.OrderByDescending(r => r.StarsCount).ThenByDescending(r => r.UpdatedDate);

            Assert.AreEqual(to - from + 1, result.Count);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        private List<Repository> GetRepositories()
        {
            var result = new List<Repository>();
            result.Add(new Repository
            {
                Name = "Repository1",
                UpdatedDate = DateTime.Now,
                CreatedDate = DateTime.Now.AddYears(-1),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository2",
                UpdatedDate = DateTime.Now.AddSeconds(-1),
                CreatedDate = DateTime.Now.AddMonths(-1),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository3",
                GoodFirstIssuesCount = 1,
                UpdatedDate = DateTime.Now.AddSeconds(-2),
                CreatedDate = DateTime.Now.AddYears(-2),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository4",
                UpdatedDate = DateTime.Now.AddSeconds(-1),
                CreatedDate = DateTime.Now.AddMonths(-2).AddSeconds(-2),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository5",
                UpdatedDate = DateTime.Now.AddSeconds(-3),
                CreatedDate = DateTime.Now.AddDays(-23),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository6",
                UpdatedDate = DateTime.Now.AddSeconds(-4),
                CreatedDate = DateTime.Now.AddMonths(-2),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository7",
                UpdatedDate = DateTime.Now.AddSeconds(-1),
                CreatedDate = DateTime.Now.AddMonths(-4),
                StarsCount = 3
            });
            result.Add(new Repository
            {
                Name = "Repository8",
                UpdatedDate = DateTime.Now.AddSeconds(-5),
                CreatedDate = DateTime.Now.AddMonths(-7),
                StarsCount = 2
            });
            result.Add(new Repository
            {
                Name = "Repository9",
                UpdatedDate = DateTime.Now.AddSeconds(-6),
                CreatedDate = DateTime.Now.AddDays(-73),
                StarsCount = 2
            });
            result.Add(new Repository
            {
                Name = "Repository10",
                UpdatedDate = DateTime.Now.AddSeconds(-7),
                CreatedDate = DateTime.Now.AddHours(-17),
                StarsCount = 2
            });
            result.Add(new Repository
            {
                Name = "Repository11",
                UpdatedDate = DateTime.Now.AddSeconds(-7),
                CreatedDate = DateTime.Now.AddHours(-17),
                StarsCount = 1
            });
            result.Add(new Repository
            {
                Name = "Repository12",
                UpdatedDate = DateTime.Now.AddDays(-1),
                CreatedDate = DateTime.Now.AddYears(-1),
                StarsCount = 1
            });
            return result;
        }
    }
}