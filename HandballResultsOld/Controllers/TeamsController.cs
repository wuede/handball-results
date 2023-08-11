using System.Net;
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
        [OutputCache(Duration = 1500, VaryByParam = "*")]
        public ActionResult Index(int teamId)
        {
            if (!resultService.IsTeamWhitelisted(teamId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Team Id {teamId} is not supported");
            }

            return View(new TeamsViewModel(teamId));
        }
    }
}