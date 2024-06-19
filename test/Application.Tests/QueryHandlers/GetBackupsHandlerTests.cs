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
        private Mock<IDepthChartQueryRepository> _mockQueryRepository;
        private IMapper _mapper;

        public GetBackupsHandlerTests()
        {
            _mockQueryRepository = new Mock<IDepthChartQueryRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }
        [Fact]
        public async Task Handle_EntryExists_ShouldReturnBackups()
        {
            // Arrange
            var handler = new GetBackupsHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetBackupsRequest
            {
                TeamId = "A",
                Position = "QB",
                PlayerNumber = 12
            };

            var entry = new DepthChartEntry
            {
                TeamId = "A",
                Position = "QB",
                PositionDepth = 0,
                Player = new Player { Number = 12, Name = "Tom Brady" }
            };

            var entries = new List<DepthChartEntry>
            {
                entry,
                new DepthChartEntry
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 1,
                    Player = new Player { Number = 13, Name = "Backup Player 1" }
                },
                new DepthChartEntry
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 2,
                    Player = new Player { Number = 14, Name = "Backup Player 2" }
                }
            };

            _mockQueryRepository.Setup(repo => repo.GetDepthChartEntryAsync(request.TeamId, request.Position, request.PlayerNumber))
                               .ReturnsAsync(entry);

            _mockQueryRepository.Setup(repo => repo.GetDepthChartEntriesReadOnlyAsync(request.TeamId))
                               .ReturnsAsync(entries);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull()
                          .And.HaveCount(2)
                          .And.ContainItemsAssignableTo<PlayerDto>()
                          .And.Contain(p => p.Number == 13 && p.Name == "Backup Player 1")
                          .And.Contain(p => p.Number == 14 && p.Name == "Backup Player 2");
        }

        [Fact]
        public async Task Handle_EntryDoesNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var handler = new GetBackupsHandler(_mockQueryRepository.Object, _mapper);

            var request = new GetBackupsRequest
            {
                TeamId = "A",
                Position = "QB",
                PlayerNumber = 99 // Non-existent player number
            };

            _mockQueryRepository.Setup(repo => repo.GetDepthChartEntryAsync(request.TeamId, request.Position, request.PlayerNumber))
                               .ReturnsAsync((DepthChartEntry)null);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
