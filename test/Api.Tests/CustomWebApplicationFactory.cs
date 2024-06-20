using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;

namespace Api.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Call the base configuration
            base.ConfigureWebHost(builder);

            builder.ConfigureServices(services =>
            {
                // Create a scope to obtain a reference to the database context (FanDuelMemoryDbContext)
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<FanDuelMemoryDbContext>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    // Seed the database with test data
                    SeedTestData(db);
                }
            });
        }

        private void SeedTestData(FanDuelMemoryDbContext dbContext)
        {
            dbContext.Teams.AddRange(
                new Team { Id = "TestA", Name = "Test Team C", Sport = Sport.TEST },
                new Team { Id = "TestB", Name = "Test Team D", Sport = Sport.TEST }
            );

            dbContext.DepthChartEntries.AddRange(
                new List<DepthChartEntry>
                {
                new DepthChartEntry { TeamId = "TestA", Position = "Pitcher", PositionDepth = 0, Player = new Player(){ Name = "Test Player 1", Number = 1} },
                new DepthChartEntry { TeamId = "TestA", Position = "Pitcher", PositionDepth = 1, Player = new Player(){ Name = "Test Player 2", Number = 2} },
                new DepthChartEntry { TeamId = "TestA", Position = "Pitcher", PositionDepth = 2, Player = new Player(){ Name = "Test Player 3", Number = 3} }
                }
            );

            dbContext.SaveChanges();
        }
    }
}
