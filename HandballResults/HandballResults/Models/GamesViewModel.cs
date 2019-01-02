using System.Collections.Generic;
using System.Linq;
using HandballResults.Services;

namespace HandballResults.Models
{
    public class GamesViewModel
    {
        public IEnumerable<Game> Games { get; set; }
        public bool Minimized { get; set; }
        public ServiceException Error { get; set; }

        public GamesViewModel(IEnumerable<Game> games, bool minimized, ServiceException error)
        {
            Games = games;
            Minimized = minimized;
            Error = error;
        }

        public IEnumerable<Game> GetVisibleGames()
        {
            if (Minimized)
            {
                return Games.Take(3);
            }

            return Games;
        }
    }
}