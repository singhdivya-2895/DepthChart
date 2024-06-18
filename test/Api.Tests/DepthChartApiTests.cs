using Application.DTO;
using Domain.Enums;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Api.Tests
{
    public class DepthChartApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DepthChartApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFullDepthChart_ShouldReturnOk()
        {
            await InitializeData();

            var response = await _client.GetAsync("/api/depthchart/full?teamId=A");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var depthChart = await response.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("QB");
            depthChart["QB"].Count.Should().Be(3);
        }
        private async Task InitializeData()
        {
            var teamResponse = await _client.PostAsJsonAsync("/api/teams", new TeamDto { Id = "A", Name = "Team A", Sport = Sport.NFL });
            teamResponse.EnsureSuccessStatusCode();

            var players = new[]
            {
                new PlayerDto { Number = 12, Name = "Tom Brady" },
                new PlayerDto { Number = 11, Name = "Blaine Gabbert" },
                new PlayerDto { Number = 2, Name = "Kyle Trask" },
                new PlayerDto { Number = 13, Name = "Mike Evans" },
                new PlayerDto { Number = 1, Name = "Jaelon Darden" },
                new PlayerDto { Number = 10, Name = "Scott Miller" }
            };

            foreach (var player in players)
            {
                var playerResponse = await _client.PostAsJsonAsync("/api/players", player);
                playerResponse.EnsureSuccessStatusCode();
            }

            var depthChartEntries = new[]
            {
                new { teamId = 1, position = "QB", player = players[0], positionDepth = 0 },
                new { teamId = 1, position = "QB", player = players[1], positionDepth = 1 },
                new { teamId = 1, position = "QB", player = players[2], positionDepth = 2 },
                new { teamId = 1, position = "WR", player = players[3], positionDepth = 0 },
                new { teamId = 1, position = "WR", player = players[4], positionDepth = 1 },
                new { teamId = 1, position = "WR", player = players[5], positionDepth = 2 }
            };

            foreach (var entry in depthChartEntries)
            {
                var depthChartResponse = await _client.PostAsJsonAsync("/api/depthchart/add", entry);
                depthChartResponse.EnsureSuccessStatusCode();
            }
        }
    }
}
