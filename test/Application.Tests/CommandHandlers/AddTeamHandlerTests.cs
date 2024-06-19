using Application.Command;
using Application.DTO;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.CommandHandlers
{
    public class AddTeamHandlerTests
    {
        private Mock<IDepthChartCommandRepository> _mockCommandRepository;
        private IMapper _mapper;

        public AddTeamHandlerTests()
        {
            _mockCommandRepository = new Mock<IDepthChartCommandRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_ValidTeam_ShouldAddAndReturnTeamDto()
        {
            // Arrange
            var handler = new AddTeamHandler(_mockCommandRepository.Object, _mapper);

            var request = new AddTeamRequest
            {
                teamDto = new TeamDto { Id = "A", Name = "Team A", Sport = Sport.NFL }
            };

            var teamToAdd = new Team { Id = "A", Name = "Team A", Sport = Sport.NFL };

            _mockCommandRepository.Setup(repo => repo.AddTeamAsync(teamToAdd)).Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A", result.Id);
            Assert.Equal("Team A", result.Name);
            Assert.Equal(Sport.NFL, result.Sport);

            _mockCommandRepository.Verify(repo => repo.AddTeamAsync(It.Is<Team>(entry =>
                    entry.Id == "A" &&
                    entry.Name == "Team A" &&
                    entry.Sport == Sport.NFL
                )), Times.Once);
        }
    }
}
