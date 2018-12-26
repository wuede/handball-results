using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("groups")]
    public class GroupController : Controller
    {
        private static readonly IResultService ResultService = new CachedShvResultService();

        [HttpGet]
        [Route("{teamId}")]
        public async Task<ActionResult> Team(int teamId)
        {
            Group group = null;
            ServiceException error = null;

            try
            {
                group = await ResultService.GetGroupForTeam(teamId);
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