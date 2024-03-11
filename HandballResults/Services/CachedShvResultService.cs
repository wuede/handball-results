using HandballResults.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HandballResults.Services
{

    public class CachedShvResultService : IResultService
    {
        public const string ServiceResolverKey = "cached";
        private const string CacheKeyPrefix = nameof(CachedShvResultService);

        private readonly IResultService resultService;
        private readonly IMemoryCache cache;

        public CachedShvResultService(ShvResultService resultService, IMemoryCache memoryCache)
        {
            this.resultService = resultService;
            cache = memoryCache;
        }

        public async Task<IEnumerable<Game>> GetResultsAsync() => await GetResultsAsync(0);

        public Task<IEnumerable<Game>> GetResultsAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-results-{teamId}";
            var valueFactory = new Func<ICacheEntry, Task<IEnumerable<Game>>>(async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await resultService.GetResultsAsync(teamId);
            });

            return AddOrGetFromCacheAsync(cacheKey, valueFactory);
        }

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-schedule-{teamId}";
            var valueFactory = new Func<ICacheEntry, Task<IEnumerable<Game>>>(async (entry) =>
            {
                var games = await resultService.GetScheduleAsync(teamId);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return games;
            });

            return AddOrGetFromCacheAsync(cacheKey, valueFactory);
        }

        private async Task<IEnumerable<Game>> AddOrGetFromCacheAsync(string cacheKey, Func<ICacheEntry, Task<IEnumerable<Game>>> valueFactory)
        {
            try
            {
                return (await cache.GetOrCreateAsync(cacheKey, valueFactory))!;
            }
            catch (ServiceException)
            {
                // do not cache exception
                cache.Remove(cacheKey);
                throw;
            }
        }

        public Task<Group?> GetGroupForTeam(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-groupByTeam-{teamId}";
            var valueFactory = new Func<ICacheEntry, Task<Group?>>(async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await resultService.GetGroupForTeam(teamId);
            });

            try
            {
                return cache.GetOrCreateAsync(cacheKey, valueFactory)!;
            }
            catch (ServiceException)
            {
                // do not cache exception
                cache.Remove(cacheKey);
                throw;
            }            
        }

        public bool IsTeamWhitelisted(int teamId) => resultService.IsTeamWhitelisted(teamId);
    }
}