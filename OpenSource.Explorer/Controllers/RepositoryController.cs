using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenSource.GitHub.GraphQL;
using OpenSource.GitHub.Models;

namespace OpenSource.Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : Controller
    {
        private readonly IRepositoryService _repositoryService;

        public RepositoryController(IRepositoryService repositoryService)
        {
            this._repositoryService = repositoryService;
        }

        // GET api/repository/search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery]RepositoryParameters parameters)
        {
            if (parameters == null ||
                (parameters.HasGoodFirstIssues == null || parameters.HasGoodFirstIssues == false) &&
                (parameters.HasHelpWantedIssues == null || parameters.HasHelpWantedIssues == false) &&
                parameters.Language == null &&
                parameters.LastUpdateAfter == null &&
                parameters.MinNumberOfStars == null &&
                string.IsNullOrEmpty(parameters.Text))
            {
                return BadRequest("Request parameters should be set");
            }
            if (!parameters.PageNumber.HasValue || !parameters.PageSize.HasValue)
            {
                return BadRequest("Pagination parameters should be set");
            }
            var repositoryPage = await this._repositoryService.GetRepositoriesAsync(parameters);
            return Ok(repositoryPage);
        }

        // GET api/repository/random
        [HttpGet("random")]
        public async Task<IActionResult> Random([FromQuery]RepositoryParameters parameters)
        {
            if (parameters == null ||
                (parameters.HasGoodFirstIssues == null || parameters.HasGoodFirstIssues == false) &&
                (parameters.HasHelpWantedIssues == null || parameters.HasHelpWantedIssues == false) &&
                parameters.Language == null &&
                parameters.LastUpdateAfter == null &&
                parameters.MinNumberOfStars == null &&
                string.IsNullOrEmpty(parameters.Text))
            {
                return BadRequest("Request parameters should be set");
            }
            var repository = await this._repositoryService.GetRandomRepositoryAsync(parameters);
            return Ok(new { Repository = repository });
        }
    }
}
