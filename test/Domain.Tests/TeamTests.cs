using Domain.Models;
using FluentAssertions;
namespace Domain.Tests
{
    public class TeamTests
    {
        [Fact]
        public void AddPlayerToDepthChart_ShouldAddPlayerAtSpecifiedDepth()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player = new Player { Name = "Tom Brady", Number = 12 };

            // Act
            team.AddDepthChartEntry("QB", player, 0);

            // Assert
            team.DepthChartEntries.Should().HaveCount(1);
            team.DepthChartEntries.First().Position.Should().Be("QB");
            team.DepthChartEntries.First().PositionDepth.Should().Be(0);
            team.DepthChartEntries.First().Player.Should().Be(player);
        }

        [Fact]
        public void AddPlayerToDepthChart_ShouldShiftPlayersWhenDepthIsSpecified()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player1 = new Player { Name = "Player 1", Number = 1 };
            var player2 = new Player { Name = "Player 2", Number = 2 };
            var player3 = new Player { Name = "Player 3", Number = 3 };

            team.AddDepthChartEntry("QB", player1, 0);
            team.AddDepthChartEntry("QB", player2, 1);

            // Act
            team.AddDepthChartEntry("QB", player3, 1);

            // Assert
            team.DepthChartEntries.Should().HaveCount(3);
            var depthChartEntries = team.DepthChartEntries.OrderBy(x => x.PositionDepth).ToList();
            depthChartEntries[0].Player.Should().Be(player1);
            depthChartEntries[1].Player.Should().Be(player3);
            depthChartEntries[2].Player.Should().Be(player2);
            depthChartEntries[1].PositionDepth.Should().Be(1);
            depthChartEntries[2].PositionDepth.Should().Be(2);
        }

        [Fact]
        public void RemovePlayerFromDepthChart_ShouldRemovePlayerAndShiftBackups()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player1 = new Player { Name = "Player 1", Number = 1 };
            var player2 = new Player { Name = "Player 2", Number = 2 };
            var player3 = new Player { Name = "Player 3", Number = 3 };

            team.AddDepthChartEntry("QB", player1, 0);
            team.AddDepthChartEntry("QB", player2, 1);
            team.AddDepthChartEntry("QB", player3, 2);

            // Act
            team.RemovePlayerFromDepthChart("QB", 2);

            // Assert
            team.DepthChartEntries.Should().HaveCount(2);
            var depthChartEntries = team.DepthChartEntries.OrderBy(x => x.PositionDepth).ToList();
            depthChartEntries[0].Player.Should().Be(player1);
            depthChartEntries[1].Player.Should().Be(player3);
            depthChartEntries[1].PositionDepth.Should().Be(1);
        }

        [Fact]
        public void GetBackups_ShouldReturnBackupsForGivenPlayerAndPosition()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player1 = new Player { Name = "Player 1", Number = 1 };
            var player2 = new Player { Name = "Player 2", Number = 2 };
            var player3 = new Player { Name = "Player 3", Number = 3 };

            team.AddDepthChartEntry("QB", player1, 0);
            team.AddDepthChartEntry("QB", player2, 1);
            team.AddDepthChartEntry("QB", player3, 2);

            // Act
            var backups = team.GetBackups("QB", 1);

            // Assert
            backups.Should().HaveCount(2);
            backups.Should().Contain(player2);
            backups.Should().Contain(player3);
        }

        [Fact]
        public void AddPlayerToDepthChart_ShouldAddPlayersAtDifferentDepthsAndPositions()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var qbPlayer = new Player { Name = "Tom Brady", Number = 12 };
            var rbPlayer = new Player { Name = "Ezekiel Elliott", Number = 21 };

            // Act
            team.AddDepthChartEntry("QB", qbPlayer, 0);
            team.AddDepthChartEntry("RB", rbPlayer, 1);

            // Assert
            team.DepthChartEntries.Should().HaveCount(2);
            team.DepthChartEntries.Should().Contain(e => e.Position == "QB" && e.Player == qbPlayer);
            team.DepthChartEntries.Should().Contain(e => e.Position == "RB" && e.Player == rbPlayer);
        }

        [Fact]
        public void RemovePlayerFromDepthChart_ShouldDoNothingIfPlayerNotFound()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };

            // Act
            team.RemovePlayerFromDepthChart("QB", 99); // Non-existing player

            // Assert
            team.DepthChartEntries.Should().BeEmpty();
        }

        [Fact]
        public void GetBackups_ShouldReturnEmptyListIfPlayerNotFound()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };

            // Act
            var backups = team.GetBackups("QB", 99); // Non-existing player

            // Assert
            backups.Should().BeEmpty();
        }

        [Fact]
        public void AddPlayerToDepthChart_ShouldHandleDuplicatePlayers()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player = new Player { Name = "Tom Brady", Number = 12 };

            // Act
            team.AddDepthChartEntry("QB", player, 0);
            team.AddDepthChartEntry("QB", player, 1); // Adding the same player again

            // Assert
            team.DepthChartEntries.Should().HaveCount(2);
        }

        [Fact]
        public void AddPlayerToDepthChart_ShouldHandleEmptyTeamId()
        {
            // Arrange
            var team = new Team { Id = "", Name = "Team A" };
            var player = new Player { Name = "Tom Brady", Number = 12 };

            // Act
            team.AddDepthChartEntry("QB", player, 0);

            // Assert
            team.DepthChartEntries.Should().HaveCount(1);
            team.DepthChartEntries.First().TeamId.Should().Be("");
        }

        [Fact]
        public void AddPlayerToDepthChart_ShouldShiftPlayersCorrectlyWhenAddingInMiddle()
        {
            // Arrange
            var team = new Team { Id = "A", Name = "Team A" };
            var player1 = new Player { Name = "Player 1", Number = 1 };
            var player2 = new Player { Name = "Player 2", Number = 2 };
            var player3 = new Player { Name = "Player 3", Number = 3 };

            team.AddDepthChartEntry("QB", player1, 0);
            team.AddDepthChartEntry("QB", player2, 1);

            // Act
            team.AddDepthChartEntry("QB", player3, 1);

            // Assert
            team.DepthChartEntries.Should().HaveCount(3);
            var depthChartEntries = team.DepthChartEntries.OrderBy(x => x.PositionDepth).ToList();
            depthChartEntries[0].Player.Should().Be(player1);
            depthChartEntries[1].Player.Should().Be(player3);
            depthChartEntries[2].Player.Should().Be(player2);
        }
    }
}