using System.Net.Http.Headers;
using HandballResults.Models;

namespace HandballResults.Services
{
    public class ShvResultService : IResultService
    {
        private static readonly Uri ApiBaseUri = new("https://clubapi.handball.ch/rest/v1/");
        private static readonly Uri ClubApiBaseUri = new(ApiBaseUri, "clubs/140631/");
        private static readonly HttpClient HttpClient = new();
        private static readonly HashSet<int> WhitelistedTeamIds = new();
        private static readonly List<int> ScheduledGameStates = new() { 1, 6 };
        private static readonly List<int> PlayedGameStates = new() { 2, 3, 4 };

        public ShvResultService(IConfigurationService configurationService)
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", "MTQwNjMxOjJYeEp0ZmRO");

            foreach (var teamId in configurationService.Get().TeamIdWhiteList)
            {
                System.Diagnostics.Trace.TraceInformation("white listing team {0}", teamId);
                WhitelistedTeamIds.Add(teamId);
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
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Team>>() ?? new List<Team>();
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
            var partialUri = "games";
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

            return await GetGamesAsync(uri, PlayedGameStates, SortOrder.Descending);
        }

        public async Task<IEnumerable<Game>> GetScheduleAsync() => await GetScheduleAsync(0);

        public async Task<IEnumerable<Game>> GetScheduleAsync(int teamId)
        {
            var partialUri = "games";
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

            return await GetGamesAsync(uri, ScheduledGameStates);
        }

        private static async Task<IEnumerable<Game>> GetGamesAsync(Uri uri, ICollection<int> desiredStates,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("fetching games from {0}", uri);
                var response = await HttpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var games = await response.Content.ReadFromJsonAsync<IEnumerable<Game>>();

                    if (games == null)
                    {
                        throw new InvalidOperationException($"{nameof(games)} should not be null");
                    }

                    var filteredByState = FilterGames(games).Where(g => desiredStates.Contains(g.GameStatusId));
                    var sorted = sortOrder == SortOrder.Ascending
                        ? filteredByState.OrderBy(g => g.GameDateTime)
                        : filteredByState.OrderByDescending(g => g.GameDateTime);

                    return sorted.ToList();
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

        public async Task<Group?> GetGroupForTeam(int teamId)
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
                    return await response.Content.ReadFromJsonAsync<Group>() ?? new Group();
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