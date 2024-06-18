using Domain.Enums;
using Domain.Models;

namespace Persistence.IRepository
{
    public interface IDepthChartQueryRepository
    {
        Task<List<DepthChartEntry>> GetDepthChartEntriesAsync(int teamId);
        Task<DepthChartEntry> GetDepthChartEntryAsync(int teamId, string position, int playerNumber);
        Task<List<Team>> GetTeamsBySportAsync(Sport sport);
    }
}
