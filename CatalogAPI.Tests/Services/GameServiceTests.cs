using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Repositories;
using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Application.Services;
using CatalogAPI.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace CatalogAPI.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IRepositoryUoW> _repositoryUoWMock;
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IUserGameRepository> _userGameRepositoryMock;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _repositoryUoWMock = new Mock<IRepositoryUoW>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            _userGameRepositoryMock = new Mock<IUserGameRepository>();

            _repositoryUoWMock.Setup(x => x.GameRepository)
                .Returns(_gameRepositoryMock.Object);

            _repositoryUoWMock.Setup(x => x.UserGameRepository)
                .Returns(_userGameRepositoryMock.Object);

            _gameService = new GameService(_repositoryUoWMock.Object);
        }

        [Fact]
        public async Task Add_Should_Return_Success_When_Game_Is_Valid()
        {
            var transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(transactionMock.Object);

            _gameRepositoryMock
                .Setup(x => x.Add(It.IsAny<GameEntity>()))
                .ReturnsAsync((GameEntity game) => game);

            _userGameRepositoryMock
                .Setup(x => x.Add(It.IsAny<UserGameEntity>()))
                .ReturnsAsync((UserGameEntity userGame) => userGame);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var gameResponse = new GameResponseDto
            {
                Name = "God of War",
                Description = "Action game"
            };

            var userId = "3c0eaaab-3285-479b-9c94-3b348eb97204";

            var result = await _gameService.Add(gameResponse, userId);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(gameResponse.Name);

            _gameRepositoryMock.Verify(x => x.Add(It.IsAny<GameEntity>()), Times.Once);
            _userGameRepositoryMock.Verify(x => x.Add(It.IsAny<UserGameEntity>()), Times.Once);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Exactly(2));
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Add_Should_Return_Error_When_Game_Is_Invalid()
        {
            var gameResponse = new GameResponseDto
            {
                Name = "",
                Description = ""
            };

            var result = await _gameService.Add(gameResponse, "user-123");

            result.Success.Should().BeFalse();

            _gameRepositoryMock.Verify(x => x.Add(It.IsAny<GameEntity>()), Times.Never);
            _userGameRepositoryMock.Verify(x => x.Add(It.IsAny<UserGameEntity>()), Times.Never);
        }

        [Fact]
        public async Task Update_Should_Return_Success_When_Game_Exists()
        {
            var transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(transactionMock.Object);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var game = new GameEntity
            {
                Id = 1,
                Name = "Old Game",
                Description = "Old Description",
                IsActive = true
            };

            var updateDto = new UpdateGameRequestDto
            {
                Name = "Updated Game",
                Description = "Updated Description",
                IsActive = true
            };

            _gameRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync(game);

            var result = await _gameService.Update(1, updateDto);

            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();

            _gameRepositoryMock.Verify(x => x.Update(It.IsAny<GameEntity>()), Times.Once);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Return_Error_When_Game_Does_Not_Exist()
        {
            var updateDto = new UpdateGameRequestDto
            {
                Name = "Updated Game",
                Description = "Updated Description",
                IsActive = true
            };

            _gameRepositoryMock.Setup(x => x.GetById(1))
                .ReturnsAsync((GameEntity?)null);

            var result = await _gameService.Update(1, updateDto);

            result.Success.Should().BeFalse();

            _gameRepositoryMock.Verify(x => x.Update(It.IsAny<GameEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task Delete_Should_Return_Success_When_Game_Exists()
        {
            var transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(transactionMock.Object);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var game = new GameEntity
            {
                Id = 1,
                Name = "God of War",
                Description = "Action game",
                IsActive = true
            };

            _gameRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync(game);

            var result = await _gameService.Delete(1);

            result.Success.Should().BeTrue();
            game.IsActive.Should().BeFalse();

            _gameRepositoryMock.Verify(x => x.Update(game), Times.Once);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Should_Return_Error_When_Game_Does_Not_Exist()
        {
            var transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(transactionMock.Object);

            _gameRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync((GameEntity?)null);

            var result = await _gameService.Delete(1);

            result.Success.Should().BeFalse();

            _gameRepositoryMock.Verify(x => x.Update(It.IsAny<GameEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            transactionMock.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Get_Should_Return_Game_List_When_Games_Exist()
        {
            var games = new List<GameResponseDto>
            {
                new GameResponseDto
                {
                    Name = "God of War",
                    Description = "Action game",
                    Price = 99.9,
                    IsActive = true
                }
            };

            _gameRepositoryMock.Setup(x => x.Get())
                .ReturnsAsync(games);

            var result = await _gameService.Get();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("God of War");

            _gameRepositoryMock.Verify(x => x.Get(), Times.Once);
        }

        [Fact]
        public async Task Get_Should_Return_Empty_List_When_No_Games_Exist()
        {
            _gameRepositoryMock.Setup(x => x.Get())
                .ReturnsAsync(new List<GameResponseDto>());

            var result = await _gameService.Get();

            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _gameRepositoryMock.Verify(x => x.Get(), Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Return_Success_When_Game_Exists()
        {
            var game = new GameEntity
            {
                Id = 1,
                Name = "God of War",
                Description = "Action game",
                IsActive = true
            };

            _gameRepositoryMock.Setup(x => x.GetById(1))
                .ReturnsAsync(game);

            var result = await _gameService.GetById(1);

            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be(game.Name);
            result.Data.Description.Should().Be(game.Description);

            _gameRepositoryMock.Verify(x => x.GetById(1), Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Return_Error_When_Game_Does_Not_Exist()
        {
            var transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(transactionMock.Object);

            _gameRepositoryMock
                .Setup(x => x.GetById(1))
                .ReturnsAsync((GameEntity?)null);

            var result = await _gameService.GetById(1);

            result.Success.Should().BeFalse();

            _gameRepositoryMock.Verify(x => x.GetById(1), Times.Once);
            transactionMock.Verify(x => x.Rollback(), Times.Once);
        }
    }
}