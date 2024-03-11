using System.Net.Http.Headers;
using HandballResults.Models;

namespace HandballResults.Services
{
    public class ShvResultService : IResultService
    {
        public const string HttpClientName = "ShvApiClient";

        private static readonly Uri ApiBaseUri = new("https://clubapi.handball.ch/rest/v1/");
        private static readonly Uri ClubApiBaseUri = new(ApiBaseUri, "clubs/140631/");
        private static readonly List<int> ScheduledGameStates = new() { 1, 6 };
        private static readonly List<int> PlayedGameStates = new() { 2, 3, 4 };

        private readonly ILogger<ShvResultService> logger;
        private readonly HttpClient httpClient;
        private readonly HashSet<int> whitelistedTeamIds = new();

        public ShvResultService(ILogger<ShvResultService> logger, IConfigurationService configurationService, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;

            var configuration = configurationService.Get();
            httpClient = httpClientFactory.CreateClient(HttpClientName);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", configuration.ShvApiKey);

            foreach (var teamId in configuration.TeamIdWhiteList)
            {
                logger.LogInformation("white listing team {0}", teamId);
                whitelistedTeamIds.Add(teamId);
            }
        }

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            const string partialUri = "teams";
            var uri = new Uri(ClubApiBaseUri, partialUri);

            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var teams = await response.Content.ReadFromJsonAsync<List<Team>>() ?? new List<Team>();
                    logger.LogInformation("received {0} teams from {1}", teams.Count, uri);
                    return teams;
                }

                logger.LogError("SHV API responded with {0}: {1}", response.StatusCode,
                    response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to get teams from {0}", uri);
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

        private async Task<IEnumerable<Game>> GetGamesAsync(Uri uri, ICollection<int> desiredStates,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var games = await response.Content.ReadFromJsonAsync<List<Game>>();

                    if (games == null)
                    {
                        throw new InvalidOperationException($"{nameof(games)} should not be null");
                    }

                    logger.LogInformation("received {0} unfiltered games from {1}", games.Count, uri);

                    var filteredByState = FilterGames(games).Where(g => desiredStates.Contains(g.GameStatusId));
                    var sorted = (sortOrder == SortOrder.Ascending
                        ? filteredByState.OrderBy(g => g.GameDateTime)
                        : filteredByState.OrderByDescending(g => g.GameDateTime)).ToList();

                    logger.LogInformation("filtered and sorted {0} games", sorted.Count);
                    return sorted;
                }

                logger.LogError("SHV API responded with {0}: {1}", response.StatusCode,
                    response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to get games {0}", uri);
            }

            throw new ServiceException("failed to get games");
        }

        private IEnumerable<Game> FilterGames(IEnumerable<Game> games)
        {
            return games.Where(g => whitelistedTeamIds.Contains(g.TeamAId) || whitelistedTeamIds.Contains(g.TeamBId))
                .ToList();
        }

        public async Task<Group?> GetGroupForTeam(int teamId)
        {
            ValidateWhitelistedTeam(teamId);
            var partialUri = $"teams/{teamId}/group";
            var uri = new Uri(ApiBaseUri, partialUri);
            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var group = await response.Content.ReadFromJsonAsync<Group>() ?? new Group();
                    logger.LogInformation("successfully received group {0} from {1}", group.GroupId, uri);
                    return group;
                }

                var content = response.Content.ReadAsStringAsync();
                logger.LogError("SHV API responded with {0}: {1}", response.StatusCode, content);
            }
            catch (Exception e)
            {
                logger.LogError(e, "failed to get group from {0}", uri);
            }

            throw new ServiceException("failed to get group");
        }

        private void ValidateWhitelistedTeam(int teamId)
        {
            if (!whitelistedTeamIds.Contains(teamId))
            {
                throw new ArgumentException($"Team Id {teamId} is not supported");
            }
        }

        public bool IsTeamWhitelisted(int teamId)
        {
            return whitelistedTeamIds.Contains(teamId);
        }
    }
}