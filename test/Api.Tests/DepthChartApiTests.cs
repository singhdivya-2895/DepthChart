using Application.DTO;
using Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using Persistence.IRepository;

namespace Api.IntegrationTests
{
    public class DepthChartApiTests : IClassFixture<CustomWebApplicationFactory<Api.Program>>
    {
        private readonly HttpClient _client;

        public DepthChartApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _ = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFullDepthChart_ShouldReturnOk()
        {
            var teamId = "A";
            var response = await _client.GetAsync($"/api/depthchart/full?teamId={teamId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await response.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("QB");
            depthChart["QB"].Count.Should().Be(3);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFullDepthChart_InvalidTeamId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidTeamId = "C";

            // Act
            var response = await _client.GetAsync($"/api/depthchart/full?teamId={invalidTeamId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Be("\"Team does not exist for the Id.\"");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddPlayerToDepthChart_NoExistingPlayers_ShouldReturnOk()
        {
            // Arrange
            var teamId = "B";
            var player = new PlayerDto { Number = 3, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchart", new { teamId, position = "RB", player, positionDepth = 0 });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/full?teamId={teamId}");

            getFullChartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await getFullChartResponse.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("RB");
            depthChart["RB"].Count.Should().Be(1);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddPlayerToDepthChart_WithExistingPlayers_MissingPositionDepth_ShouldReturnOk()
        {
            // Arrange
            var teamId = "A";
            var player = new PlayerDto { Number = 4, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchart", new { teamId, position = "QB", player });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);


            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/full?teamId={teamId}");

            getFullChartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await getFullChartResponse.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("QB");
            depthChart["QB"].Count.Should().Be(4);
            depthChart["QB"][3].PositionDepth.Should().Be(3);

            // Remove the added player
            var removePlayerResponse = await _client.DeleteAsync($"/api/depthchart?teamId={teamId}&position=QB&playerNumber=4");

            removePlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPlayer = await removePlayerResponse.Content.ReadFromJsonAsync<PlayerDto>();
            returnedPlayer.Should().NotBeNull();
            returnedPlayer.Number.Should().Be(4);
            returnedPlayer.Name.Should().Be("New Player");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddPlayerToDepthChart_WithExistingPlayers_WithMatchingPositionDepth_ShouldReturnOk()
        {
            // Arrange
            var teamId = "A";
            var player = new PlayerDto { Number = 4, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchart", new { teamId, position = "QB", player, positionDepth = 1 });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);


            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/full?teamId={teamId}");

            getFullChartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await getFullChartResponse.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("QB");
            depthChart["QB"].Count.Should().Be(4);
            depthChart["QB"].Where(x => x.Player.Number == 4).FirstOrDefault()?.PositionDepth.Should().Be(1);
            depthChart["QB"].Where(x => x.Player.Number == 2).FirstOrDefault()?.PositionDepth.Should().Be(2);

            // Remove the added player
            var removePlayerResponse = await _client.DeleteAsync($"/api/depthchart?teamId={teamId}&position=QB&playerNumber=4");

            removePlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPlayer = await removePlayerResponse.Content.ReadFromJsonAsync<PlayerDto>();
            returnedPlayer.Should().NotBeNull();
            returnedPlayer.Number.Should().Be(4);
            returnedPlayer.Name.Should().Be("New Player");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task RemovePlayerFromDepthChart_ShouldReturnOk()
        {
            // Arrange
            var teamId = "A";
            var playerNumber = 4;
            var position = "RB";
            var requestDto = new DepthChartEntryDto { TeamId = teamId, Position = position, Player = new PlayerDto() { Number = playerNumber, Name = "New Player" } };

            // Act
            var addPlayerResponse = await _client.PostAsJsonAsync("/api/depthchart", requestDto);

            addPlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = await _client.DeleteAsync($"/api/depthchart?teamId={teamId}&position=RB&playerNumber={playerNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPlayer = await response.Content.ReadFromJsonAsync<PlayerDto>();
            returnedPlayer.Should().NotBeNull();
            returnedPlayer.Number.Should().Be(playerNumber);
            returnedPlayer.Name.Should().Be("New Player");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task RemovePlayerFromDepthChart_InvalidPlayer_ShouldReturnNotFound()
        {
            // Arrange
            var teamId = "A";
            var playerNumber = 999;

            // Act
            var response = await _client.DeleteAsync($"/api/depthchart?teamId={teamId}&position=QB&playerNumber={playerNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetBackups_ShouldReturnOk()
        {
            // Arrange
            var teamId = "A";
            var playerNumber = 1;

            // Act
            var response = await _client.GetAsync($"/api/depthchart/backups?teamId={teamId}&position=QB&playerNumber={playerNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var backups = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
            backups.Should().NotBeNull();
            backups.Count.Should().Be(2);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetTeamsBySport_ShouldReturnOk()
        {
            // Arrange
            var sport = Sport.NFL;

            // Act
            var response = await _client.GetAsync($"/api/teams?sport={sport}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedTeams = await response.Content.ReadFromJsonAsync<List<TeamDto>>();
            returnedTeams.Should().NotBeNull();
            returnedTeams.Count.Should().Be(2);
        }
    }
}
