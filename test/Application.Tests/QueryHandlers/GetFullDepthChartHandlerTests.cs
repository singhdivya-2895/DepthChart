using Application.Query;
using AutoMapper;
using Domain.Models;
using Moq;
using Persistence.IRepository;
using FluentAssertions;

namespace Application.Tests.QueryHandlers
{
    public class GetFullDepthChartHandlerTests
    {
        private Mock<IDepthChartQueryRepository> _mockQueryRepository;
        private IMapper _mapper;

        public GetFullDepthChartHandlerTests()
        {
            _mockQueryRepository = new Mock<IDepthChartQueryRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_ShouldReturnFullDepthChart()
        {
            // Arrange
            var handler = new GetFullDepthChartHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetFullDepthChartRequest
            {
                TeamId = "A"
            };

            var depthChartEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 0, Player = new Player { Number = 12, Name = "Tom Brady" } },
                new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 1, Player = new Player { Number = 11, Name = "Backup QB 1" } },
                new DepthChartEntry { TeamId = "A", Position = "WR", PositionDepth = 0, Player = new Player { Number = 80, Name = "Mike Evans" } },
                new DepthChartEntry { TeamId = "A", Position = "WR", PositionDepth = 1, Player = new Player { Number = 10, Name = "Chris Godwin" } }
            };

            _mockQueryRepository.Setup(repo => repo.GetDepthChartEntriesReadOnlyAsync(request.TeamId))
                               .ReturnsAsync(depthChartEntries);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull()
                          .And.HaveCount(2) // Expecting 2 positions: "QB" and "WR"
                          .And.ContainKey("QB")
                          .WhoseValue.Should().HaveCount(2) // Expecting 2 entries for "QB"
                          .And.Contain(dto => dto.Player.Name == "Tom Brady" && dto.Player.Number == 12) // Check first QB
                          .And.Contain(dto => dto.Player.Name == "Backup QB 1" && dto.Player.Number == 11); // Check second QB

            result.Should().NotBeNull()
                          .And.HaveCount(2)
                          .And.ContainKey("WR")
                          .WhoseValue.Should().HaveCount(2) // Expecting 2 entries for "WR"
                          .And.Contain(dto => dto.Player.Name == "Mike Evans" && dto.Player.Number == 80) // Check first WR
                          .And.Contain(dto => dto.Player.Name == "Chris Godwin" && dto.Player.Number == 10); // Check second WR
        }

        [Fact]
        public async Task Handle_NoEntriesFound_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var handler = new GetFullDepthChartHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetFullDepthChartRequest
            {
                TeamId = "B" // Non-existent team ID
            };

            _mockQueryRepository.Setup(repo => repo.GetDepthChartEntriesReadOnlyAsync(request.TeamId))
                               .ReturnsAsync(new List<DepthChartEntry>());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
        }
    }
}
