using Application.DTO;
using Application.Query;
using AutoMapper;
using Domain.Models;
using FluentAssertions;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.QueryHandlers
{
    public class GetBackupsHandlerTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly IMapper _mapper;

        public GetBackupsHandlerTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_EntryExists_ShouldReturnBackups()
        {
            // Arrange
            var handler = new GetBackupsHandler(_mockTeamRepository.Object, _mapper);

            var teamId = "A";
            var position = "QB";
            var playerNumber = 12;

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
            team.AddDepthChartEntry(position, new Player { Number = 13, Name = "Backup Player 1" }, 1);
            team.AddDepthChartEntry(position, new Player { Number = 14, Name = "Backup Player 2" }, 1);

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(teamId))
                               .ReturnsAsync(team);

            // Act
            var request = new GetBackupsRequest
            {
                TeamId = teamId,
                Position = position,
                PlayerNumber = playerNumber
            };

            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull()
                  .And.HaveCount(2)
                  .And.ContainItemsAssignableTo<PlayerDto>()
                  .And.Contain(p => p.Number == 13 && p.Name == "Backup Player 1")
                  .And.Contain(p => p.Number == 14 && p.Name == "Backup Player 2");
        }

        [Fact]
        public async Task Handle_TeamDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var handler = new GetBackupsHandler(_mockTeamRepository.Object, _mapper);

            var teamId = "A";
            var position = "QB";
            var playerNumber = 99;

            _mockTeamRepository.Setup(repo => repo.GetByIdAsync(teamId))
                               .ReturnsAsync((Team)null);

            // Act
            var request = new GetBackupsRequest
            {
                TeamId = teamId,
                Position = position,
                PlayerNumber = playerNumber
            };

            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
