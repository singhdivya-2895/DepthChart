using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence.Context;
using Persistence.IRepository;
using Persistence.Repository;
using FluentAssertions;

namespace Persistence.Tests
{
    public class DepthChartCommandRepositoryTests
    {
        private readonly Mock<FanDuelMemoryDbContext> _dbContextMock;
        private readonly IDepthChartCommandRepository _repository;

        public DepthChartCommandRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FanDuelMemoryDbContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
                            .Options;

            _dbContextMock = new Mock<FanDuelMemoryDbContext>(options);
            _repository = new DepthChartCommandRepository(_dbContextMock.Object);
        }

        [Fact]
        public async Task AddPlayerToDepthChartAsync_ValidEntry_ShouldAddEntry()
        {
            // Arrange
            var entryToAdd = new DepthChartEntry
            {
                Id = 1,
                TeamId = "A",
                Position = "QB",
                PositionDepth = 0,
                Player = new Player { Id = 1, Name = "Test Player", Number = 99 }
            };
            var mockDbSet = new Mock<DbSet<DepthChartEntry>>();
            _dbContextMock.Setup(db => db.DepthChartEntries).Returns(mockDbSet.Object);

            // Act
            await _repository.AddPlayerToDepthChartAsync(entryToAdd);

            // Assert
            _dbContextMock.Verify(db => db.DepthChartEntries.AddAsync(It.IsAny<DepthChartEntry>(), It.IsAny<CancellationToken>()), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Additional assertion: Ensure the added entry is the same as what was passed to AddAsync
            _dbContextMock.Object.DepthChartEntries.Should().Contain(entryToAdd);
        }

        [Fact]
        public async Task RemovePlayerFromDepthChartAsync_ExistingEntry_ShouldRemoveEntry()
        {
            // Arrange
            var entryToRemove = new DepthChartEntry
            {
                Id = 1,
                TeamId = "A",
                Position = "QB",
                PositionDepth = 0,
                Player = new Player { Id = 1, Name = "Test Player", Number = 99 }
            };

            _dbContextMock.Setup(db => db.DepthChartEntries.FindAsync(entryToRemove.Id))
                          .ReturnsAsync(entryToRemove);

            // Act
            await _repository.RemovePlayerFromDepthChartAsync(entryToRemove);

            // Assert
            _dbContextMock.Verify(db => db.DepthChartEntries.Remove(It.IsAny<DepthChartEntry>()), Times.Once);
            _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Additional assertion: Ensure the removed entry is the same as what was passed to Remove
            _dbContextMock.Object.DepthChartEntries.Should().NotContain(entryToRemove);
        }
    }
}
