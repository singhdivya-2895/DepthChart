using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class FanDuelMemoryDbContext : DbContext
    {
        public DbSet<DepthChartEntry> DepthChartEntries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }

        public FanDuelMemoryDbContext(DbContextOptions<FanDuelMemoryDbContext> options) : base(options)
        {
        }
    }
}
