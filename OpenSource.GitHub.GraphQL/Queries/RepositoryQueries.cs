namespace OpenSource.GitHub.GraphQL
{
    internal class RepositoryQueries
    {
        /// <summary>
        /// Template of the query to search for repositories.
        /// </summary>
        internal const string REPOSITORY_SEARCH_QUERY = @"query($query: String!, $first: Int, $last: Int, $languageCount: Int!, $after: String, $before: String) {
  search(type: REPOSITORY, query: $query, first: $first, last: $last, after: $after, before: $before) {
    edges {
      cursor,
      textMatches {
        highlights {
          text,
          endIndice,
          beginIndice
        }
      },
      node {
        ... on Repository {
          name,
          owner { login },
          url,
          description,
          stargazerCount,
          forkCount,
          createdAt,
          updatedAt,
          hasIssuesEnabled,
          primaryLanguage { name },
          languages(first: $languageCount,  orderBy: {field: SIZE, direction: DESC}) {
            nodes { ... on Language { name } },
            totalCount
          },
          helpWantedIssues: issues(first: 1, filterBy: { labels: ""help wanted"" }) {
            totalCount
          },
          goodFirstIssues: issues(first: 1, filterBy: { labels: ""good first issue"" }) {
            totalCount
          }
        }
      }
    },
    repositoryCount
  }
}";

        /// <summary>
        /// Template of the query to get number of repositories for query.
        /// </summary>
        internal const string REPOSITORIES_COUNT_QUERY = @"query($query: String!) {
  search(type: REPOSITORY, query: $query, first: 1) {
    repositoryCount
  }
}";

        /// <summary>
        /// Template of the query to get repositories stars count.
        /// </summary>
        internal const string REPOSITORY_STARS_QUERY = @"query($query: String!, $first: Int, $after: String) {
  search(type: REPOSITORY, query: $query, first: $first, after: $after) {
    edges {
      node {
        ... on Repository {
          stargazerCount
        }
      }
    }
  }
}";
    }
}
