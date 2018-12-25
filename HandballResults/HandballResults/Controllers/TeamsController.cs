using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("teams")]
    public class TeamsController : Controller
    {

        private static readonly IResultService ResultService = new CachedShvResultService();

        // GET: teams/{teamId}
        [Route("{teamId}")]
        public async Task<ActionResult> Index(int teamId)
        {
            IEnumerable<Game> schedule = new List<Game>();
            IEnumerable<Game> results = new List<Game>();
            Group group = null;
            ServiceException error = null;

            try
            {
                schedule = await ResultService.GetScheduleAsync(teamId);
                results = await ResultService.GetResultsAsync(teamId);
                group = await ResultService.GetGroupForTeam(teamId);
            }
            catch (ServiceException e)
            {
                error = e;
            }

            return View(new TeamsViewModel(schedule, results, group, error));
        }
    }
}