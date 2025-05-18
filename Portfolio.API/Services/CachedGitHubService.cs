using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitHub.Service.Interfaces;
using GitHub.Service.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Portfolio.API.Services
{
    public class CachedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _cache;
        private const string PortfolioCacheKey = "portfolio";
        private const string LastActivityCacheKey = "lastActivity";
        private DateTimeOffset _lastCacheRefresh = DateTimeOffset.MinValue;

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache cache)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
        {
            // Check if we need to refresh the cache based on user activity
            await RefreshCacheIfNeeded();

            // Try to get from cache first
            if (_cache.TryGetValue(PortfolioCacheKey, out IEnumerable<RepositoryInfo>? cachedPortfolio) && cachedPortfolio != null)
            {
                return cachedPortfolio;
            }

            // If not in cache, get from service and cache it
            var portfolio = await _gitHubService.GetPortfolioAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // Cache for 10 minutes

            _cache.Set(PortfolioCacheKey, portfolio, cacheOptions);
            _lastCacheRefresh = DateTimeOffset.Now;

            return portfolio;
        }

        public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? name = null, string? language = null, string? username = null)
        {
            // Don't cache search results as they can vary widely
            return await _gitHubService.SearchRepositoriesAsync(name, language, username);
        }

        public async Task<DateTimeOffset> GetLatestActivityDateAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(LastActivityCacheKey, out DateTimeOffset cachedLastActivity))
            {
                return cachedLastActivity;
            }

            // If not in cache, get from service and cache it
            var lastActivity = await _gitHubService.GetLatestActivityDateAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes

            _cache.Set(LastActivityCacheKey, lastActivity, cacheOptions);

            return lastActivity;
        }

        private async Task RefreshCacheIfNeeded()
        {
            // Get the latest activity date
            var latestActivity = await GetLatestActivityDateAsync();

            // If there's new activity since our last cache refresh, invalidate the cache
            if (latestActivity > _lastCacheRefresh)
            {
                _cache.Remove(PortfolioCacheKey);
            }
        }
    }
}
