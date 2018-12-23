using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("games")]
    public class GamesController : Controller
    {
        private static readonly IResultService ResultService = new CachedShvResultService();

        // GET: games/results
        [Route("results/{teamId?}")]
        public async Task<ActionResult> Results(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException error = null;

            try
            {
                games = await ResultService.GetResultsAsync(teamId);
            }
            catch (ServiceException e)
            {
                error = e;
            }

            return View(new GamesViewModel(games, error));
        }

        // GET: games/results
        [Route("schedule/{teamId?}")]
        public async Task<ActionResult> Schedule(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException error = null;

            try
            {
                games = await ResultService.GetSchedule(teamId);
            }
            catch (ServiceException e)
            {
                error = e;
            }

            return View(new GamesViewModel(games, error));
        }
    }
}