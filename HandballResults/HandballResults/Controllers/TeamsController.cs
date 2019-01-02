using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("teams")]
    public class TeamsController : Controller
    {
        private readonly IResultService resultService;

        public TeamsController(IResultService resultService)
        {
            this.resultService = resultService;
        }

        // GET: teams/{teamId}
        [Route("{teamId}")]
        public async Task<ActionResult> Index(int teamId, bool minimizedSchedule = true, bool minimizedResults = true)
        {
            if (!await resultService.IsTeamSupportedAsync(teamId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Team Id {teamId} is not supported");
            }

            return View(new TeamsViewModel(teamId, minimizedSchedule, minimizedResults));
        }
    }
}