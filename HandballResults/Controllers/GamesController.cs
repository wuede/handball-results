﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using HandballResults.Models;
using HandballResults.Services;

namespace HandballResults.Controllers
{
    [RoutePrefix("games")]
    public class GamesController : Controller
    {
        private readonly IResultService resultService;

        public GamesController(IResultService resultService)
        {
            this.resultService = resultService;
        }

        // GET: games/results
        [Route("results/{teamId?}")]
        [OutputCache(Duration = 1500, VaryByParam = "*")]
        public async Task<ActionResult> Results(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException error = null;

            try
            {
                games = await resultService.GetResultsAsync(teamId);
            }
            catch (ServiceException e)
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
        [OutputCache(Duration = 1500, VaryByParam = "*")]
        public async Task<ActionResult> Schedule(int teamId = 0)
        {
            IEnumerable<Game> games = new List<Game>();
            ServiceException error = null;

            try
            {
                games = await resultService.GetScheduleAsync(teamId);
            }
            catch (ServiceException e)
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