using Domain.Models;

namespace Persistence.IRepository
{
    public interface IDepthChartCommandRepository
    {
        Task AddTeamAsync(Team team);
        Task AddPlayerToDepthChartAsync(DepthChartEntry depthChartEntry);
        Task RemovePlayerFromDepthChartAsync(DepthChartEntry depthChartEntry);
    }
}
