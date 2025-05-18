using System.Threading.Tasks;
using GitHub.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public SearchController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchRepositories(
            [FromQuery] string? name = null,
            [FromQuery] string? language = null,
            [FromQuery] string? username = null)
        {
            var repositories = await _gitHubService.SearchRepositoriesAsync(name, language, username);
            return Ok(repositories);
        }
    }
}