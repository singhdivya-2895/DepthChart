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
using Moq;
using Persistence.IRepository;
using Domain.Models;

namespace Api.Tests
{
    public class DepthChartApiTests : IClassFixture<CustomWebApplicationFactory<Api.Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private Mock<IDepthChartCommandRepository> _mockDepthChartCommandRepository;
        private Mock<IDepthChartQueryRepository> _mockDepthChartQueryRepository;

        public DepthChartApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _mockDepthChartCommandRepository = new Mock<IDepthChartCommandRepository>();
            _mockDepthChartQueryRepository = new Mock<IDepthChartQueryRepository>();
            _factory.DepthChartCommandRepository = _mockDepthChartCommandRepository.Object;
            _factory.DepthChartQueryRepository = _mockDepthChartQueryRepository.Object;
            _client = factory.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetFullDepthChart_ShouldReturnOk()
        {
            var teamId = "TestA";
            _mockDepthChartQueryRepository.Setup(repo => repo.GetDepthChartEntriesAsync(teamId))
                       .ReturnsAsync(new List<DepthChartEntry>
                       {
                           new DepthChartEntry { TeamId = teamId, Position = "QB", PositionDepth = 0, Player = new Player(){ Name = "Player 1", Number = 1} },
                           new DepthChartEntry { TeamId = teamId, Position = "QB", PositionDepth = 1, Player = new Player(){ Name = "Player 2", Number = 2} },
                           new DepthChartEntry { TeamId = teamId, Position = "QB", PositionDepth = 2, Player = new Player(){ Name = "Player 3", Number = 3} }
                       });

            var response = await _client.GetAsync($"/api/depthchart/full?teamId={teamId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var depthChart = await response.Content.ReadFromJsonAsync<Dictionary<string, List<DepthChartEntryDto>>>();
            depthChart.Should().NotBeNull();
            depthChart.Should().ContainKey("QB");
            depthChart["QB"].Count.Should().Be(3);
        }
    }
}
