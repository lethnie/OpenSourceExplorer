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
                parameters.HasGoodFirstIssues == null &&
                parameters.HasHelpWantedIssues == null &&
                parameters.Language == null &&
                parameters.LastUpdateAfter == null &&
                parameters.MinNumberOfStars == null &&
                string.IsNullOrEmpty(parameters.Text))
            {
                return BadRequest("Request parameters should be set");
            }
            var repositoryPage = await this._repositoryService.GetRepositoriesAsync(parameters);
            return Ok(repositoryPage);
        }
    }
}
