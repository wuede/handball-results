using HandballResults.Extensions;
using HandballResults.Models;
using HandballResults.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HandballResults.Controllers
{

    [Route("games")]
    public class GamesController : Controller
    {
        private readonly IResultService resultService;

        public GamesController(ResultServiceResolver resultServiceResolver)
        {
            this.resultService = resultServiceResolver.Invoke(CachedShvResultService.ServiceResolverKey);
        }

        // GET: games/results
        [Route("results/{teamId?}")]
        [OutputCache(Duration = 1500, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult> Results(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException? error = null;

            try
            {
                games = await resultService.GetResultsAsync(teamId);
            }
            catch (ServiceException? e)
            {
                error = e;
            }

            var viewModel = new GamesViewModel(games, error);
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewModel);
            }
 
            return View(viewModel);
        }

        // GET: games/results
        [Route("schedule/{teamId?}")]
        [OutputCache(Duration = 1500, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult> Schedule(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException? error = null;

            try
            {
                games = await resultService.GetScheduleAsync(teamId);
            }
            catch (ServiceException? e)
            {
                error = e;
            }

            var viewModel = new GamesViewModel(games, error);
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewModel);
            }

            return View(viewModel);
        }
    }
}