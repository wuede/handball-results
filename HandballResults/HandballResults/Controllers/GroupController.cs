using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("groups")]
    public class GroupController : Controller
    {
        private readonly IResultService resultService;

        public GroupController(IResultService resultService)
        {
            this.resultService = resultService;
        }

        [HttpGet]
        [Route("{teamId}")]
        public async Task<ActionResult> Team(int teamId)
        {
            Group group = null;
            ServiceException error = null;

            try
            {
                group = await resultService.GetGroupForTeam(teamId);
            }
            catch (ServiceException e)
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