using Domain.Enums;
using Domain.Models;

namespace Persistence.IRepository
{
    public interface IDepthChartQueryRepository
    {
        Task<List<DepthChartEntry>> GetDepthChartEntriesReadOnlyAsync(string teamId);
        Task<DepthChartEntry> GetDepthChartEntryAsync(string teamId, string position, int playerNumber);
        Task<List<Team>> GetTeamsBySportAsync(Sport sport);
    }
}
