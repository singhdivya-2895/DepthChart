using Application.Query;
using AutoMapper;
using Domain.Models;
using Moq;
using Persistence.IRepository;
using FluentAssertions;
using Persistence.Repository;

namespace Application.Tests.QueryHandlers
{
    public class GetFullDepthChartHandlerTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private IMapper _mapper;

        public GetFullDepthChartHandlerTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_ShouldReturnFullDepthChart()
        {
            // Arrange
            var handler = new GetFullDepthChartHandler(_mockTeamRepository.Object, _mapper);

            var request = new GetFullDepthChartRequest
            {
                TeamId = "A"
            };
            var team = new Team { Id = "A" };

            var depthChartEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 0, Player = new Player { Number = 12, Name = "Tom Brady" } },
                new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 1, Player = new Player { Number = 11, Name = "Backup QB 1" } },
                new DepthChartEntry { TeamId = "A", Position = "WR", PositionDepth = 0, Player = new Player { Number = 80, Name = "Mike Evans" } },
                new DepthChartEntry { TeamId = "A", Position = "WR", PositionDepth = 1, Player = new Player { Number = 10, Name = "Chris Godwin" } }
            };
            foreach (var entry in depthChartEntries) { 
                team.AddDepthChartEntry(entry.Position, entry.Player, entry.PositionDepth);
            }

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.TeamId))
                               .ReturnsAsync(team);

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
            var handler = new GetFullDepthChartHandler(_mockTeamRepository.Object, _mapper);

            var request = new GetFullDepthChartRequest
            {
                TeamId = "B" // Non-existent team ID
            };
            var team = new Team { Id = "B" };

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.TeamId))
                               .ReturnsAsync(team);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task Handle_TeamNotExist_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var handler = new GetFullDepthChartHandler(_mockTeamRepository.Object, _mapper);

            var request = new GetFullDepthChartRequest
            {
                TeamId = "B" // Non-existent team ID
            };
            var team = new Team { Id = "B" };

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.TeamId))
                               .ReturnsAsync((Team)null);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
