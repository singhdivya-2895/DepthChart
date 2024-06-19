using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class FanDuelMemoryDbContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<DepthChartEntry> DepthChartEntries { get; set; }
        public DbSet<Player> Players { get; set; }

        public FanDuelMemoryDbContext(DbContextOptions<FanDuelMemoryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(t => t.DepthChartEntries)
                .WithOne()
                .HasForeignKey(e => e.TeamId);

            modelBuilder.Entity<DepthChartEntry>()
                .OwnsOne(e => e.Player);

            base.OnModelCreating(modelBuilder);
        }
    }
}
