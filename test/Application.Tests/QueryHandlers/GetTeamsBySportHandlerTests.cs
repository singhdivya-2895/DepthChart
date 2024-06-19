﻿using Application.Query;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Moq;
using Persistence.IRepository;
using FluentAssertions;

namespace Application.Tests.QueryHandlers
{
    public class GetTeamsBySportHandlerTests
    {
        private Mock<IDepthChartQueryRepository> _mockQueryRepository;
        private IMapper _mapper;

        public GetTeamsBySportHandlerTests()
        {
            _mockQueryRepository = new Mock<IDepthChartQueryRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task Handle_SportExists_ShouldReturnTeams()
        {
            // Arrange
            var handler = new GetTeamsBySportHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetTeamsBySportRequest
            {
                Sport = Sport.NFL // Replace with the appropriate sport value being tested
            };

            var teams = new List<Team>
        {
            new Team { Id = "A", Name = "Team A", Sport = Sport.NFL },
            new Team { Id = "B", Name = "Team B", Sport = Sport.NFL }
        };

            _mockQueryRepository.Setup(repo => repo.GetTeamsBySportAsync(request.Sport))
                               .ReturnsAsync(teams);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull()
                          .And.HaveCount(2)
                          .And.OnlyContain(t => t.Sport == Sport.NFL) // Ensure all returned teams have the correct sport
                          .And.Contain(t => t.Id == "A" && t.Name == "Team A")
                          .And.Contain(t => t.Id == "B" && t.Name == "Team B");
        }

        [Fact]
        public async Task Handle_SportDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var handler = new GetTeamsBySportHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetTeamsBySportRequest
            {
                Sport = Sport.MLB // Use a sport that does not exist in the mock setup
            };

            _mockQueryRepository.Setup(repo => repo.GetTeamsBySportAsync(request.Sport))
                               .ReturnsAsync(new List<Team>());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}