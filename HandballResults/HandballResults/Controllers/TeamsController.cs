using System.Web.Mvc;
using HandballResults.Models;

namespace HandballResults.Controllers
{
    [RoutePrefix("teams")]
    public class TeamsController : Controller
    {
        // GET: teams/{teamId}
        [Route("{teamId}")]
        public ActionResult Index(int teamId, bool minimizedSchedule = true, bool minimizedResults = true)
        {
            return View(new TeamsViewModel(teamId, minimizedSchedule, minimizedResults));
        }
    }
}