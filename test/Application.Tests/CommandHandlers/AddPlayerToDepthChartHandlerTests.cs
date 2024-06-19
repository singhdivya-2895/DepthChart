using Moq;
using AutoMapper;
using Application.Command;
using Application.DTO;
using Domain.Enums;
using Domain.Models;
using Persistence.IRepository;
using Microsoft.Extensions.Logging;

namespace Application.Tests.CommandHandlers
{
    public class AddPlayerToDepthChartHandlerTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<AddPlayerToDepthChartHandler>> _logger;

        public AddPlayerToDepthChartHandlerTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();
            _logger = new Mock<ILogger<AddPlayerToDepthChartHandler>>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task AddPlayerToDepthChartHandler_NoExistingPlayers_ShouldAddPlayer()
        {
            // Arrange
            var handler = new AddPlayerToDepthChartHandler(_mockTeamRepository.Object, _mapper, _logger.Object);

            var request = new AddPlayerToDepthChartRequest
            {
                DepthChartEntry = new DepthChartEntryDto()
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 0,
                    Player = new PlayerDto { Name = "Tom Brady", Number = 12 }
                }
            };

            var team = new Team("A", "Team A", Sport.NFL);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.DepthChartEntry.TeamId)).ReturnsAsync(team);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(It.Is<Team>(t =>
                t.DepthChartEntries.Any(e =>
                    e.Position == "QB" &&
                    e.Player.Name == "Tom Brady" &&
                    e.Player.Number == 12))), Times.Once);
        }

        [Fact]
        public async Task AddPlayerToDepthChartHandler_ExistingPlayer_PositionDepthMissing_ShouldAddPlayer()
        {
            // Arrange
            var handler = new AddPlayerToDepthChartHandler(_mockTeamRepository.Object, _mapper, _logger.Object);

            var request = new AddPlayerToDepthChartRequest
            {
                DepthChartEntry = new DepthChartEntryDto()
                {
                    TeamId = "A",
                    Position = "QB",
                    Player = new PlayerDto { Name = "Tom Brady", Number = 12 }
                }
            };

            var team = new Team("A", "Team A", Sport.NFL);
            team.AddDepthChartEntry("QB", new Player(1, "Existing Player", 1), 0);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.DepthChartEntry.TeamId)).ReturnsAsync(team);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(It.Is<Team>(t =>
                t.DepthChartEntries.Any(e =>
                    e.Position == "QB" &&
                    e.Player.Name == "Tom Brady" &&
                    e.Player.Number == 12 &&
                    e.PositionDepth == 1))), Times.Once);
        }

        [Fact]
        public async Task AddPlayerToDepthChartHandler_ExistingPlayer_MatchingDepth_ShouldAddPlayer()
        {
            // Arrange
            var handler = new AddPlayerToDepthChartHandler(_mockTeamRepository.Object, _mapper, _logger.Object);

            var request = new AddPlayerToDepthChartRequest
            {
                DepthChartEntry = new DepthChartEntryDto()
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 0,
                    Player = new PlayerDto { Name = "Tom Brady", Number = 12 }
                }
            };

            var team = new Team("A", "Team A", Sport.NFL);
            team.AddDepthChartEntry("QB", new Player(1, "Existing Player", 1), 1);
            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(request.DepthChartEntry.TeamId)).ReturnsAsync(team);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockTeamRepository.Verify(repo => repo.UpdateAsync(It.Is<Team>(t =>
                t.DepthChartEntries.Any(e =>
                    e.Position == "QB" &&
                    e.Player.Name == "Tom Brady" &&
                    e.Player.Number == 12 &&
                    e.PositionDepth == 0))), Times.Once);
        }
    }
}