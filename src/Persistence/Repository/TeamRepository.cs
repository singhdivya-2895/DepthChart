using Domain.Models;
using Persistence.Context;
using Persistence.IRepository;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Persistence.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly FanDuelMemoryDbContext _context;

        public TeamRepository(FanDuelMemoryDbContext context)
        {
            _context = context;
        }

        public async Task<Team> GetByIdAsync(string teamId)
        {
            return await _context.Teams
                .Where(x => x.Id == teamId)
                .Include(t => t.DepthChartEntries)
                .ThenInclude(e => e.Player)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Team team)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Team>> GetTeamsBySportAsync(Sport sport)
        {
            return await _context.Teams
                .Where(t => t.Sport == sport)
                .ToListAsync();
        }
    }
}
