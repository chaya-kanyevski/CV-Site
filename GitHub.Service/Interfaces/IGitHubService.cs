using System.Collections.Generic;
using System.Threading.Tasks;
using GitHub.Service.Models;

namespace GitHub.Service.Interfaces
{
    public interface IGitHubService
    {
        Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync();
        Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? name = null, string? language = null, string? username = null);
        Task<DateTimeOffset> GetLatestActivityDateAsync();
    }
}