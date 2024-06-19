using Application.Command;
using AutoMapper;
using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.CommandHandlers
{
    public class RemovePlayerFromDepthChartHandlerTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<RemovePlayerFromDepthChartHandler>> _logger;

        public RemovePlayerFromDepthChartHandlerTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();
            _logger = new Mock<ILogger<RemovePlayerFromDepthChartHandler>>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_PlayerExists_ShouldRemoveAndReturnPlayerDto()
        {
            // Arrange
            var handler = new RemovePlayerFromDepthChartHandler(_mockTeamRepository.Object, _mapper, _logger.Object);

            var request = new RemovePlayerFromDepthChartRequest
            {
                TeamId = "A",
                PlayerNumber = 12,
                Position = "QB"
            };

            var teamId = request.TeamId;
            var playerNumber = request.PlayerNumber;
            var position = request.Position;

            // Mocking the retrieval of Team with depth chart entries
            var team = new Team
            {
                Id = teamId
            };
            var depthChartEntry = new DepthChartEntry
            {
                TeamId = teamId,
                Position = position,
                PositionDepth = 0,
                Player = new Player { Number = 12, Name = "Tom Brady" }
            };

            team.AddDepthChartEntry(depthChartEntry.Position, depthChartEntry.Player, depthChartEntry.PositionDepth);

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(teamId))
                               .ReturnsAsync(team);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Number.Should().Be(playerNumber);
            result.Name.Should().Be("Tom Brady");

            // Verify removal of player from depth chart
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(
                It.IsAny<Team>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_PlayerDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var handler = new RemovePlayerFromDepthChartHandler(_mockTeamRepository.Object, _mapper, _logger.Object);

            var request = new RemovePlayerFromDepthChartRequest
            {
                TeamId = "A",
                PlayerNumber = 99, // Non-existent player number
                Position = "QB"
            };

            var teamId = request.TeamId;
            var team = new Team
            {
                Id = teamId
            };
            var depthChartEntry = new DepthChartEntry
            {
                TeamId = teamId,
                Position = "QB",
                PositionDepth = 0,
                Player = new Player { Number = 12, Name = "Tom Brady" }
            };

            team.AddDepthChartEntry(depthChartEntry.Position, depthChartEntry.Player, depthChartEntry.PositionDepth);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(teamId))
                               .ReturnsAsync(team);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();

            // Verify that RemovePlayerFromDepthChartAsync was not called
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(
                It.IsAny<Team>()),
                Times.Never
            );
        }
    }
}
