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
        private static readonly HashSet<int> ExcludedTeamIds = new HashSet<int>();

        private static Task<Dictionary<int, Team>> _supportedTeamsFetchTask;

        static ShvResultService()
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "MTQwNjMxOjJYeEp0ZmRO");
            StartFetchingSupportedTeams();

            var excludedTeams = HandballResultsConfig.GetExcludedTeams();
            foreach (TeamElement team in excludedTeams)
            {
                System.Diagnostics.Trace.TraceInformation("excluding team {0}", team.Id);
                ExcludedTeamIds.Add(team.Id);
            }
        }

        private static void StartFetchingSupportedTeams()
        {
            _supportedTeamsFetchTask = GetTeamsAsync().ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    return t.Result.Where(team => !ExcludedTeamIds.Contains(team.TeamId)).GroupBy(team => team.TeamId)
                        .ToDictionary(g => g.Key, g => g.Last());
                }

                return new Dictionary<int, Team>();
            });
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
                await ValidateTeamAsync(teamId);
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
                await ValidateTeamAsync(teamId);
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

                    var filtered = await FilterGamesAsync(games);
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

        private static async Task<IEnumerable<Game>> FilterGamesAsync(IEnumerable<Game> games)
        {
            var supportedTeamNames = (await EnsureSupportedTeamsAsync()).Values.Select(t => t.TeamName).ToList();
            return games.Where(g =>
                supportedTeamNames.Contains(g.TeamAName) || supportedTeamNames.Contains(g.TeamBName)).ToList();
        }

        public async Task<Group> GetGroupForTeam(int teamId)
        {
            await ValidateTeamAsync(teamId);
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

        private static async Task ValidateTeamAsync(int teamId)
        {
            var supportedTeams = await EnsureSupportedTeamsAsync();
            if (!supportedTeams.Any() || !supportedTeams.ContainsKey(teamId))
            {
                throw new ArgumentException($"Team Id {teamId} is not supported");
            }
        }

        public async Task<bool> IsTeamSupportedAsync(int teamId)
        {
            var supportedTeams = await EnsureSupportedTeamsAsync();
            return supportedTeams.ContainsKey(teamId);
        }


        private static async Task<Dictionary<int, Team>> EnsureSupportedTeamsAsync()
        {
            var supportedTeams = await _supportedTeamsFetchTask;
            if (supportedTeams.Any())
            {
                return supportedTeams;
            }
            {
                var attempt = 1;
                while (attempt++ < 6)
                {
                    StartFetchingSupportedTeams();
                    supportedTeams = await _supportedTeamsFetchTask;
                    if (supportedTeams.Any())
                    {
                        return supportedTeams;
                    }
                }
            }

            System.Diagnostics.Trace.TraceWarning("Giving up fetching supported teams");
            return new Dictionary<int, Team>();
        }
    }
}