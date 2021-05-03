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
          createdAt ,
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
    repositoryCount,
    pageInfo {
      startCursor,
      endCursor,
      hasNextPage,
      hasPreviousPage
    }
  }
}";
    }
}
