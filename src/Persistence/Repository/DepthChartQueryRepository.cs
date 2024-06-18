using Domain.Enums;
using Domain.Models;
using Persistence.IRepository;
using System.Data;
using System;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class DepthChartQueryRepository : IDepthChartQueryRepository
    {

        private readonly FanDuelMemoryDbContext _dbContext;

        public DepthChartQueryRepository(FanDuelMemoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DepthChartEntry>> GetDepthChartEntriesAsync(int teamId)
        {
            var entries = await _dbContext.DepthChartEntries
                                            .Where(d => d.TeamId == teamId)
                                            .ToListAsync();

            return entries;
        }

        public async Task<DepthChartEntry> GetDepthChartEntryAsync(int teamId,string position, int playerId)
        {
            var entry = await _dbContext.DepthChartEntries
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(d => d.TeamId == teamId && d.Player.Id == playerId && d.Position == position);

            return entry;
        }

        public async Task<List<Team>> GetTeamsBySportAsync(Sport sport)
        {
            var teams = await _dbContext.Teams
                                       .AsNoTracking()
                                       .Where(t => t.Sport == sport)
                                       .ToListAsync();

            return teams;
        }
    }
}
