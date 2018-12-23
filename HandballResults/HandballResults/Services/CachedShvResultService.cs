using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using HandballResults.Models;
using HandballResults.Util;

namespace HandballResults.Services
{

    public class CachedShvResultService : IResultService
    {
        private const string CacheKeyPrefix = nameof(CachedShvResultService);

        private readonly IResultService resultService;
        private readonly ObjectCache cache;

        public CachedShvResultService()
        {
            resultService = new ShvResultService();
            cache = MemoryCache.Default;
        }

        public async Task<IEnumerable<Game>> GetResultsAsync() => await GetResultsAsync(0);

        public Task<IEnumerable<Game>> GetResultsAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-results-${teamId}";
            var valueFactory = new Func<Task<IEnumerable<Game>>>(async () => await resultService.GetResultsAsync(teamId));

            return cache.AddOrGetFromCache(cacheKey, valueFactory, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
        }

        public async Task<IEnumerable<Game>> GetSchedule() => await GetSchedule(0);

        public Task<IEnumerable<Game>> GetSchedule(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-schedule-${teamId}";
            var valueFactory = new Func<Task<IEnumerable<Game>>>(async () => await resultService.GetSchedule(teamId));

            return cache.AddOrGetFromCache(cacheKey, valueFactory, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
        }
    }
}