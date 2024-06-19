using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Domain.Models;
using Persistence.Repository;
using Domain.Enums;

namespace Persistence.Tests;
public class TeamRepositoryTests
{
    private DbContextOptions<FanDuelMemoryDbContext> _options;

    public TeamRepositoryTests()
    {
        // Initialize a new in-memory database context options
        _options = new DbContextOptionsBuilder<FanDuelMemoryDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_WithValidTeamId_ShouldReturnTeamWithDepthChartEntries()
    {
        // Arrange
        var teamId = "A";
        using (var context = new FanDuelMemoryDbContext(_options))
        {

            var team = new Domain.Models.Team { Id = teamId, Name = "Team A" };
            var depthChartEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { Position = "QB", PositionDepth = 1, Player = new Player { Name = "Player 1" } },
                new DepthChartEntry { Position = "RB", PositionDepth = 2, Player = new Player { Name = "Player 2" } }
            };
            foreach (var depthChartEntry in depthChartEntries)
            {
                team.AddDepthChartEntry(depthChartEntry.Position, depthChartEntry.Player, depthChartEntry.PositionDepth);
            }

            context.Teams.Add(team);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);
            var result = await repository.GetByIdAsync(teamId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(teamId);
            result.Name.Should().Be("Team A");
            result.Sport.Should().Be(Sport.NFL);
            result.DepthChartEntries.Should().HaveCount(2);
            result.DepthChartEntries.Should().Contain(e => e.Position == "QB" && e.PositionDepth == 1);
            result.DepthChartEntries.Should().Contain(e => e.Position == "RB" && e.PositionDepth == 2);
        }
    }

    [Fact]
    public async Task AddAsync_ShouldAddTeamToDbContext()
    {
        // Arrange
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);

            var team = new Team
            {
                Id = "B",
                Name = "Team B",
                Sport = Sport.MLB
            };

            // Act
            await repository.AddAsync(team);

            // Assert
            var addedTeam = await context.Teams.FindAsync("B");
            addedTeam.Should().NotBeNull();
            addedTeam.Name.Should().Be("Team B");
            addedTeam.Sport.Should().Be(Sport.MLB);
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTeamInDbContext()
    {
        // Arrange
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);

            var team = new Team
            {
                Id = "C",
                Name = "Team C",
                Sport = Sport.NHL
            };

            context.Teams.Add(team);
            await context.SaveChangesAsync();

            // Act
            team.Name = "Updated Team C";
            await repository.UpdateAsync(team);

            // Assert
            var updatedTeam = await context.Teams.FindAsync("C");
            updatedTeam.Should().NotBeNull();
            updatedTeam.Name.Should().Be("Updated Team C");
        }
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveTeamFromDbContext()
    {
        // Arrange
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);

            var team = new Team
            {
                Id = "D",
                Name = "Team D",
                Sport = Sport.NBA
            };

            context.Teams.Add(team);
            await context.SaveChangesAsync();

            // Act
            await repository.RemoveAsync(team);

            // Assert
            var removedTeam = await context.Teams.FindAsync("D");
            removedTeam.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetTeamsBySportAsync_ShouldReturnTeamsBySport()
    {
        // Arrange
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);

            var teams = new List<Team>
            {
                new Team { Id = "E", Name = "Team E", Sport = Sport.NFL },
                new Team { Id = "F", Name = "Team F", Sport = Sport.NFL },
                new Team { Id = "G", Name = "Team G", Sport = Sport.MLB }
            };

            context.Teams.AddRange(teams);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new FanDuelMemoryDbContext(_options))
        {
            var repository = new TeamRepository(context);
            var result = await repository.GetTeamsBySportAsync(Sport.NFL);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Id == "E" && t.Name == "Team E");
            result.Should().Contain(t => t.Id == "F" && t.Name == "Team F");
        }
    }
}
