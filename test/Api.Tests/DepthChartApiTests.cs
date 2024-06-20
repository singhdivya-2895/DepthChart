using Application.DTO;
using Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using Persistence.IRepository;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;

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
            var teamId = "TestA";
            var response = await _client.GetAsync($"/api/depthchart/{teamId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await response.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("Pitcher");
            depthChart["Pitcher"].Count.Should().Be(3);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFullDepthChart_InvalidTeamId_ShouldReturnNotFound()
        {
            // Arrange
            var invalidTeamId = "TestC";

            // Act
            var response = await _client.GetAsync($"/api/depthchart/{invalidTeamId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage.Should().Be("\"Team does not exist for the Id.\"");
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddPlayerToDepthChart_InvalidPosition_ShouldReturnBadRequest()
        {
            // Arrange
            var teamId = "";
            var player = new PlayerDto { Number = 0, Name = "" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchartentry", new { teamId, position = "", player, positionDepth = 0 });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().ContainKey("TeamId");
            errorResponse.Errors.Should().ContainKey("Position");
            errorResponse.Errors.Should().ContainKey("Player.Name");
            errorResponse.Errors.Should().ContainKey("Player.Number");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddPlayerToDepthChart_NoExistingPlayers_ShouldReturnOk()
        {
            // Arrange
            var teamId = "TestB";
            var player = new PlayerDto { Number = 3, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchartentry", new { teamId, position = "RB", player, positionDepth = 0 });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/{teamId}");

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
            var teamId = "TestA";
            var player = new PlayerDto { Number = 4, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchartentry", new { teamId, position = "Pitcher", player });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);


            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/{teamId}");

            getFullChartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await getFullChartResponse.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("Pitcher");
            depthChart["Pitcher"].Count.Should().Be(4);
            depthChart["Pitcher"][3].PositionDepth.Should().Be(3);

            // Remove the added player
            var removePlayerResponse = await _client.DeleteAsync($"/api/depthchartentry/{teamId}?position=Pitcher&playerNumber=4");

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
            var teamId = "TestA";
            var player = new PlayerDto { Number = 4, Name = "New Player" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/depthchartentry", new { teamId, position = "Pitcher", player, positionDepth = 1 });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);


            // Check the depth chart now
            var getFullChartResponse = await _client.GetAsync($"/api/depthchart/{teamId}");

            getFullChartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var depthChart = await getFullChartResponse.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("Pitcher");
            depthChart["Pitcher"].Count.Should().Be(4);
            depthChart["Pitcher"].Where(x => x.Player.Number == 4).FirstOrDefault()?.PositionDepth.Should().Be(1);
            depthChart["Pitcher"].Where(x => x.Player.Number == 2).FirstOrDefault()?.PositionDepth.Should().Be(2);

            // Remove the added player
            var removePlayerResponse = await _client.DeleteAsync($"/api/depthchartentry/{teamId}?position=Pitcher&playerNumber=4");

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
            var teamId = "TestA";
            var playerNumber = 4;
            var position = "RB";
            var requestDto = new DepthChartEntryDto { TeamId = teamId, Position = position, Player = new PlayerDto() { Number = playerNumber, Name = "New Player" } };

            // Act
            var addPlayerResponse = await _client.PostAsJsonAsync("/api/depthchartentry", requestDto);

            addPlayerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var response = await _client.DeleteAsync($"/api/depthchartentry/{teamId}?position=RB&playerNumber={playerNumber}");

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
            var teamId = "TestA";
            var playerNumber = 999;

            // Act
            var response = await _client.DeleteAsync($"/api/depthchartentry/{teamId}?position=Pitcher&playerNumber={playerNumber}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetBackups_ShouldReturnOk()
        {
            // Arrange
            var teamId = "TestA";
            var playerNumber = 1;

            // Act
            var response = await _client.GetAsync($"/api/depthchart/{teamId}/backups?position=Pitcher&playerNumber={playerNumber}");

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
            var sport = Sport.TEST;

            // Act
            var response = await _client.GetAsync($"/api/teams/{sport}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedTeams = await response.Content.ReadFromJsonAsync<List<TeamDto>>();
            returnedTeams.Should().NotBeNull();
            returnedTeams.Count.Should().Be(2);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddTeam_InvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange

            // Act
            var response = await _client.PostAsJsonAsync($"/api/team", new { id = "", name = "", sport = "TEST" });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            errorResponse.Should().NotBeNull();
            errorResponse.Errors.Should().ContainKey("Id");
            errorResponse.Errors.Should().ContainKey("Name");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddTeam_NewTeam_ShouldReturnOK()
        {
            // Arrange

            // Act
            var response = await _client.PostAsJsonAsync($"/api/team", new { id = "NewTeam", name = "New Team", sport = "TEST" });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await _client.GetAsync($"/api/teams/TEST");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedTeams = await getResponse.Content.ReadFromJsonAsync<List<TeamDto>>();
            returnedTeams.Should().NotBeNull();
            returnedTeams.Count.Should().Be(3);
        }
    }
}
