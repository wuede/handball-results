﻿using System;
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

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-schedule-${teamId}";
            var valueFactory = new Func<Task<IEnumerable<Game>>>(async () => await resultService.GetScheduleAsync(teamId));

            return cache.AddOrGetFromCache(cacheKey, valueFactory, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
        }

        public Task<Group> GetGroupForTeam(int teamId)
        {
            var cacheKey = $"{CacheKeyPrefix}-groupByTeam-${teamId}";
            var valueFactory = new Func<Task<Group>>(async () => await resultService.GetGroupForTeam(teamId));

            return cache.AddOrGetFromCache(cacheKey, valueFactory, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(30)));
        }
    }
}