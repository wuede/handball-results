using HandballResults.Models;
using HandballResults.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HandballResults.Controllers
{
    [Route("teams")]
    public class TeamsController : Controller
    {
        private readonly IResultService resultService;

        public TeamsController(ResultServiceResolver resultServiceResolver)
        {
            this.resultService = resultServiceResolver.Invoke(CachedShvResultService.ServiceResolverKey);
        }

        // GET: teams/{teamId}
        [HttpGet]
        [Route("{teamId}")]
        [OutputCache(Duration = 1500, VaryByQueryKeys = new[] { "*" })]
        public ActionResult Index(int teamId)
        {
            if (!resultService.IsTeamWhitelisted(teamId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = $"Team Id {teamId} is not supported" });
            }

            return View(new TeamsViewModel(teamId));
        }
    }
}