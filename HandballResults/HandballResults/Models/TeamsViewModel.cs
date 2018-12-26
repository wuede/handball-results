using System.Collections.Generic;
using HandballResults.Services;

namespace HandballResults.Models
{
    public class TeamsViewModel
    {
        public int TeamId { get; set; }

        public IEnumerable<Game> Schedule { get; set; }

        public IEnumerable<Game> Results { get; set; }

        public Group Group { get; }

        public ServiceException Error { get; set; }

        public TeamsViewModel(IEnumerable<Game> schedule, IEnumerable<Game> results, Group group, ServiceException error)
        {
            Schedule = schedule;
            Results = results;
            Group = group;
            Error = error;
        }

        public TeamsViewModel(int teamId)
        {
            TeamId = teamId;
        }
    }
}