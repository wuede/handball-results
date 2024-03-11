using HandballResults.Extensions;
using HandballResults.Models;
using HandballResults.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HandballResults.Controllers
{
    [Route("groups")]
    public class GroupController : Controller
    {
        private readonly IResultService resultService;

        public GroupController(ResultServiceResolver resultServiceResolver)
        {
            this.resultService = resultServiceResolver.Invoke(CachedShvResultService.ServiceResolverKey);
        }

        [HttpGet]
        [Route("{teamId}")]
        [OutputCache(Duration = 1500, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult> Team(int teamId)
        {
            Group? group = null;
            ServiceException? error = null;

            try
            {
                group = await resultService.GetGroupForTeam(teamId);
            }
            catch (ServiceException? e)
            {
                error = e;
            }

            var viewModel = new GroupViewModel(group, error);
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewModel);
            }

            return View(viewModel);
        }
    }
}