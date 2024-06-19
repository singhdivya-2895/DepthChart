using Application.Command;
using Application.DTO;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using FluentAssertions;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.CommandHandlers
{
    public class AddTeamHandlerTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly IMapper _mapper;

        public AddTeamHandlerTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_ValidTeam_ShouldAddAndReturnTeamDto()
        {
            // Arrange
            var handler = new AddTeamHandler(_mockTeamRepository.Object, _mapper);

            var request = new AddTeamRequest
            {
                TeamDto = new TeamDto { Id = "A", Name = "Team A", Sport = Sport.NFL }
            };

            var teamToAdd = new Team(request.TeamDto.Id, request.TeamDto.Name, request.TeamDto.Sport);

            _mockTeamRepository.Setup(repo => repo.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("A");
            result.Name.Should().Be("Team A");
            result.Sport.Should().Be(Sport.NFL);

            _mockTeamRepository.Verify(repo => repo.AddAsync(It.Is<Team>(t =>
                t.Id == "A" &&
                t.Name == "Team A" &&
                t.Sport == Sport.NFL
            )), Times.Once);
        }

    }
}
