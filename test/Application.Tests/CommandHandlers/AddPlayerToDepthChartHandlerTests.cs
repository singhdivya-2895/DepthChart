using Application.Command;
using Application.DTO;
using AutoMapper;
using Domain.Models;
using Moq;
using Persistence.IRepository;

namespace Application.Tests.CommandHandlers
{
    public class AddPlayerToDepthChartHandlerTests
    {
        private Mock<IDepthChartCommandRepository> _mockCommandRepository;
        private IMapper _mapper;

        public AddPlayerToDepthChartHandlerTests()
        {
            _mockCommandRepository = new Mock<IDepthChartCommandRepository>();

            // Initialize AutoMapper
            _mapper = AutoMapperSetup.Initialize();
        }

        [Fact]
        public async Task AddPlayerToDepthChartHandler_NoExistingPlayers_ShouldAddPlayer()
        {
            // Arrange

            var handler = new AddPlayerToDepthChartHandler(_mockCommandRepository.Object, _mapper);

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

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockCommandRepository.Verify(repo => repo.AddPlayerToDepthChartAsync(It.IsAny<DepthChartEntry>()), Times.Once);
        }

        [Fact]
        public async Task AddPlayerToDepthChartHandler_ExistingPlayer_PositionDepthMissing_ShouldAddPlayer()
        {
            // Arrange


            var handler = new AddPlayerToDepthChartHandler(_mockCommandRepository.Object, _mapper);

            var request = new AddPlayerToDepthChartRequest
            {
                DepthChartEntry = new DepthChartEntryDto()
                {
                    TeamId = "A",
                    Position = "QB",
                    Player = new PlayerDto { Name = "Tom Brady", Number = 12 }
                }
            };
            _mockCommandRepository.Setup(repo => repo.GetDepthChartEntriesAsync(request.DepthChartEntry.TeamId, It.IsAny<bool>(), It.IsAny<string>()))
                              .ReturnsAsync(new List<DepthChartEntry>() { new DepthChartEntry()
                                            {
                                                TeamId = "A",
                                                Position = "QB",
                                                PositionDepth = 0,
                                                Player = new Player { Name = "Existing Player", Number = 1 }
                                            }});

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockCommandRepository.Verify(repo => repo.AddPlayerToDepthChartAsync(
                It.Is<DepthChartEntry>(entry =>
                    entry.TeamId == "A" &&
                    entry.Position == "QB" &&
                    entry.PositionDepth == 1 &&
                    entry.Player.Name == "Tom Brady" &&
                    entry.Player.Number == 12
                )),
                Times.Once
            );
        }



        [Fact]
        public async Task AddPlayerToDepthChartHandler_ExistingPlayer_MatchingDepth_ShouldAddPlayer()
        {
            // Arrange


            var handler = new AddPlayerToDepthChartHandler(_mockCommandRepository.Object, _mapper);

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
            _mockCommandRepository.Setup(repo => repo.GetDepthChartEntriesAsync(request.DepthChartEntry.TeamId, It.IsAny<bool>(), It.IsAny<string>()))
                              .ReturnsAsync(new List<DepthChartEntry>() { new DepthChartEntry()
                                            {
                                                TeamId = "A",
                                                Position = "QB",
                                                PositionDepth = 1,
                                                Player = new Player { Name = "Existing Player", Number = 1 }
                                            }});

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            _mockCommandRepository.Verify(repo => repo.AddPlayerToDepthChartAsync(
                It.Is<DepthChartEntry>(entry =>
                    entry.TeamId == "A" &&
                    entry.Position == "QB" &&
                    entry.PositionDepth == 0 &&
                    entry.Player.Name == "Tom Brady" &&
                    entry.Player.Number == 12
                )),
                Times.Once
            );
        }
    }
}
