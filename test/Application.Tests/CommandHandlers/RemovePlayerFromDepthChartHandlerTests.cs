using Application.Command;
using AutoMapper;
using Domain.Models;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.CommandHandlers
{
    public class RemovePlayerFromDepthChartHandlerTests
    {
        private Mock<IDepthChartCommandRepository> _mockCommandRepository;
        private IMapper _mapper;

        public RemovePlayerFromDepthChartHandlerTests()
        {
            _mockCommandRepository = new Mock<IDepthChartCommandRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }


        [Fact]
        public async Task Handle_PlayerExists_ShouldRemoveAndReturnPlayerDto()
        {
            // Arrange
            var handler = new RemovePlayerFromDepthChartHandler(_mockCommandRepository.Object, _mapper);

            var request = new RemovePlayerFromDepthChartRequest
            {
                TeamId = "A",
                PlayerNumber = 12,
                Position = "QB"
            };

            var depthChartEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 0,
                    Player = new Player { Number = 12, Name = "Tom Brady" }
                }
            };

            var entryToRemove = depthChartEntries.First();

            _mockCommandRepository.Setup(repo => repo.GetDepthChartEntriesAsync(request.TeamId, true, request.Position))
                                 .ReturnsAsync(depthChartEntries);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.Number);
            Assert.Equal("Tom Brady", result.Name);

            _mockCommandRepository.Verify(repo => repo.RemovePlayerFromDepthChartAsync(entryToRemove), Times.Once);
        }

        [Fact]
        public async Task Handle_PlayerDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var handler = new RemovePlayerFromDepthChartHandler(_mockCommandRepository.Object, _mapper);

            var request = new RemovePlayerFromDepthChartRequest
            {
                TeamId = "A",
                PlayerNumber = 99, // Non-existent player number
                Position = "QB"
            };

            var depthChartEntries = new List<DepthChartEntry>
            {
                new DepthChartEntry
                {
                    TeamId = "A",
                    Position = "QB",
                    PositionDepth = 0,
                    Player = new Player { Number = 12, Name = "Tom Brady" }
                }
            };

            _mockCommandRepository.Setup(repo => repo.GetDepthChartEntriesAsync(request.TeamId, true, request.Position))
                                 .ReturnsAsync(depthChartEntries);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);

            _mockCommandRepository.Verify(repo => repo.RemovePlayerFromDepthChartAsync(It.IsAny<DepthChartEntry>()), Times.Never);
        }
    }
}
