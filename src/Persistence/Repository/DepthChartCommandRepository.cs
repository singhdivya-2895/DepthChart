using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.IRepository;

namespace Persistence.Repository
{
    public class DepthChartCommandRepository : IDepthChartCommandRepository
    {
        private readonly FanDuelMemoryDbContext _context;

        public DepthChartCommandRepository(FanDuelMemoryDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepthChartEntry>> GetDepthChartEntriesAsync(string teamId, bool includePlayers = false, string postion = "")
        {
            IQueryable<DepthChartEntry> query = _context.DepthChartEntries
                                                    .Where((d => d.TeamId == teamId))
                                                    .AsQueryable();

            if (postion != String.Empty)
            {
                query = query.Where(d => d.Position == postion);
            }

            if (includePlayers)
            {
                query = query.Include(d => d.Player);
            }

            return await query.ToListAsync();
        }

        public async Task AddPlayerToDepthChartAsync(DepthChartEntry depthChartEntry)
        {
            await _context.DepthChartEntries.AddAsync(depthChartEntry);
            await _context.SaveChangesAsync();
        }

        public async Task AddTeamAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePlayerFromDepthChartAsync(DepthChartEntry depthChartEntry)
        {
            _context.DepthChartEntries.Remove(depthChartEntry);
            await _context.SaveChangesAsync();
        }
    }
}
