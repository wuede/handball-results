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

        public CachedShvResultService(ShvResultService resultService)
        {
            this.resultService = resultService;
            cache = MemoryCache.Default;
        }

        public async Task<IEnumerable<Game>> GetResultsAsync() => await GetResultsAsync(0);

        public Task<IEnumerable<Game>> GetResultsAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-results-${teamId}";
            var valueFactory = new Func<Task<IEnumerable<Game>>>(async () => await resultService.GetResultsAsync(teamId));

            return AddOrGetFromCacheAsync(cacheKey, valueFactory);
        }

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-schedule-${teamId}";
            var valueFactory = new Func<Task<IEnumerable<Game>>>(async () => await resultService.GetScheduleAsync(teamId));

            return AddOrGetFromCacheAsync(cacheKey, valueFactory);
        }

        private async Task<IEnumerable<Game>> AddOrGetFromCacheAsync(string cacheKey, Func<Task<IEnumerable<Game>>> valueFactory)
        {
            try
            {
                return await cache.AddOrGetFromCache(cacheKey, valueFactory,
                    DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
            }
            catch (ServiceException)
            {
                // do not cache exception
                cache.Remove(cacheKey);
                throw;
            }
        }

        public Task<Group> GetGroupForTeam(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-groupByTeam-{teamId}";
            var valueFactory = new Func<Task<Group>>(async () => await resultService.GetGroupForTeam(teamId));

            try
            {
                return cache.AddOrGetFromCache(cacheKey, valueFactory,
                    DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
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