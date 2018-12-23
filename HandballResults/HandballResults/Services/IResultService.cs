using System.Collections.Generic;
using System.Threading.Tasks;
using HandballResults.Models;

namespace HandballResults.Services
{
    interface IResultService
    {
        Task<IEnumerable<Game>> GetResultsAsync();
        Task<IEnumerable<Game>> GetResultsAsync(int teamId);

        Task<IEnumerable<Game>> GetSchedule();
        Task<IEnumerable<Game>> GetSchedule(int teamId);

    }
}
