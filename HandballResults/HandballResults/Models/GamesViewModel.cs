using System.Collections.Generic;
using HandballResults.Services;

namespace HandballResults.Models
{
    public class GamesViewModel
    {
        public IEnumerable<Game> Games { get; set; }
        public ServiceException Error { get; set; }

        public GamesViewModel(IEnumerable<Game> games, ServiceException error)
        {
            Games = games;
            Error = error;
        }
    }
}