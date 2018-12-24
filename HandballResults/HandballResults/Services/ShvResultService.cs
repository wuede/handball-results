using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HandballResults.Models;


namespace HandballResults.Services
{
    public class ShvResultService : IResultService
    {
        private static readonly Uri ApiBaseUri = new Uri("http://api.handball.ch/rest/v1/");
        private static readonly Uri ClubApiBaseUri = new Uri(ApiBaseUri, "clubs/140631/");
        private static readonly HttpClient HttpClient = new HttpClient();

        static ShvResultService()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "MTQwNjMxOjJYeEp0ZmRO");
        }

        public async Task<IEnumerable<Game>> GetResultsAsync() => await GetResultsAsync(0);

        public async Task<IEnumerable<Game>> GetResultsAsync(int teamId)
        {
            var partialUri = "games?status=played&order=desc";
            if (teamId > 0)
            {
                partialUri = $"teams/{teamId}/{partialUri}";
            }
            var uri = new Uri(ClubApiBaseUri, partialUri);
            return await GetGamesAsync(uri);
        }

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public async Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var partialUri = "games?status=planned&order=asc";
            if (teamId > 0)
            {
                partialUri = $"teams/{teamId}/{partialUri}";
            }
            var uri = new Uri(ClubApiBaseUri, partialUri);
            return await GetGamesAsync(uri);
        }

        private static async Task<IEnumerable<Game>> GetGamesAsync(Uri uri)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("fetching games from {0}", uri);
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<Game>>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("failed to get games {0}: {1}", uri, e);
            }

            throw new ServiceException("failed to get games");
        }

        public async Task<Group> GetGroupForTeam(int teamId)
        {
            var partialUri = $"teams/{teamId}/group";
            var uri = new Uri(ApiBaseUri, partialUri);
            try
            {
                System.Diagnostics.Trace.TraceInformation("fetching group from {0}", uri);
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Group>();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("failed to get group {0}: {1}", uri, e);
            }

            throw new ServiceException("failed to get group");
        }
    }
}