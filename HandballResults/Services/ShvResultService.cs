using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HandballResults.Config;
using HandballResults.Models;

namespace HandballResults.Services
{
    public class ShvResultService : IResultService
    {
        private static readonly Uri ApiBaseUri = new Uri("https://clubapi.handball.ch/rest/v1/");
        private static readonly Uri ClubApiBaseUri = new Uri(ApiBaseUri, "clubs/140631/");
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly HashSet<int> WhitelistedTeamIds = new HashSet<int>();

        static ShvResultService()
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "MTQwNjMxOjJYeEp0ZmRO");

            var teamWhitelist = HandballResultsConfig.GetTeamWhitelist();
            foreach (TeamElement team in teamWhitelist)
            {
                System.Diagnostics.Trace.TraceInformation("whitelisting team {0}", team.Id);
                WhitelistedTeamIds.Add(team.Id);
            }
        }

        public static async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            const string partialUri = "teams";
            var uri = new Uri(ClubApiBaseUri, partialUri);

            try
            {
                System.Diagnostics.Trace.TraceInformation("fetching teams from {0}", uri);
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<Team>>();
                }

                System.Diagnostics.Trace.TraceError("SHV API responded with {0}: {1}", response.StatusCode,
                    response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("failed to get teams {0}: {1}", uri, e);
            }

            throw new ServiceException("failed to get teams");
        }

        public async Task<IEnumerable<Game>> GetResultsAsync() => await GetResultsAsync(0);

        public async Task<IEnumerable<Game>> GetResultsAsync(int teamId)
        {
            var partialUri = "games?status=played&order=desc";
            Uri uri;
            if (teamId > 0)
            {
                ValidateWhitelistedTeam(teamId);
                partialUri = $"teams/{teamId}/{partialUri}";
                uri = new Uri(ApiBaseUri, partialUri);
            }
            else
            {
                uri = new Uri(ClubApiBaseUri, partialUri);
            }

            return await GetGamesAsync(uri);
        }

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public async Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var partialUri = "games?status=planned&order=asc";
            Uri uri;
            if (teamId > 0)
            {
                ValidateWhitelistedTeam(teamId);
                partialUri = $"teams/{teamId}/{partialUri}";
                uri = new Uri(ApiBaseUri, partialUri);
            }
            else
            {
                uri = new Uri(ClubApiBaseUri, partialUri);
            }

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
                    var games = await response.Content.ReadAsAsync<IEnumerable<Game>>();

                    var filtered = FilterGames(games);
                    return filtered.OrderBy(g => g.GameDateTime);
                }

                System.Diagnostics.Trace.TraceError("SHV API responded with {0}: {1}", response.StatusCode,
                    response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("failed to get games {0}: {1}", uri, e);
            }

            throw new ServiceException("failed to get games");
        }

        private static IEnumerable<Game> FilterGames(IEnumerable<Game> games)
        {
            return games.Where(g => WhitelistedTeamIds.Contains(g.TeamAId) || WhitelistedTeamIds.Contains(g.TeamBId))
                .ToList();
        }

        public async Task<Group> GetGroupForTeam(int teamId)
        {
            ValidateWhitelistedTeam(teamId);
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

                var content = response.Content.ReadAsStringAsync();
                System.Diagnostics.Trace.TraceError("SHV API responded with {0}: {1}", response.StatusCode, content);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("failed to get group {0}: {1}", uri, e);
            }

            throw new ServiceException("failed to get group");
        }

        private static void ValidateWhitelistedTeam(int teamId)
        {
            if (!WhitelistedTeamIds.Contains(teamId))
            {
                throw new ArgumentException($"Team Id {teamId} is not supported");
            }
        }

        public bool IsTeamWhitelisted(int teamId)
        {
            return WhitelistedTeamIds.Contains(teamId);
        }
    }
}