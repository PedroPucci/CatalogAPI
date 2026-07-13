using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Services;
using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Application.Validators;
using CatalogAPI.Domain.Common;
using CatalogAPI.Domain.Entities;
using CatalogAPI.Shared.Logging;
using Serilog;

namespace CatalogAPI.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public GameService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public async Task<Result<GameEntity>> Add(CreateGameRequestDto gameResponse,string userId)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var gameEntity = new GameEntity
                {
                    Name = gameResponse.Name,
                    Description = gameResponse.Description,
                    Price = gameResponse.Price,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                var validationResult = await IsValidGameRequest(gameEntity);

                if (!validationResult.Success)
                {
                    await transaction.RollbackAsync();
                    Log.Warning(LogMessages.InvalidGameInputs());
                    return Result<GameEntity>.Error(validationResult.Message);
                }

                var savedGame = await _repositoryUoW.GameRepository.Add(gameEntity);

                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.AddingGameSuccess(savedGame));
                return Result<GameEntity>.Ok(savedGame);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(LogMessages.AddingGameError(ex));
                return Result<GameEntity>.Error($"Error to add a new Game: {ex.Message}");
            }
        }

        public async Task<Result<bool>> Update(int id, UpdateGameRequestDto updateGameRequest)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var game = await _repositoryUoW.GameRepository.GetById(id);

                if (game is null)
                {
                    var message = LogMessages.CannotPerformActionOnGame("update", id);
                    Log.Error(message);
                    return Result<bool>.Error(message);
                }

                game.Name = updateGameRequest.Name;
                game.Description = updateGameRequest.Description;
                game.ModificationDate = DateTime.UtcNow;
                game.IsActive = updateGameRequest.IsActive;

                var isValid = await IsValidGameRequest(game);
                if (!isValid.Success)
                    return Result<bool>.Error(isValid.Message);

                _repositoryUoW.GameRepository.Update(game);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.UpdatingSuccessGame(game));
                return Result<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.UpdatingErrorGame(ex));
                throw new InvalidOperationException($"Failed to update game with id {id}. See logs for details.", ex);
            }
        }

        public async Task<Result<bool>> Delete(int id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var game = await _repositoryUoW.GameRepository.GetById(id);

                if (game is null)
                {
                    transaction.Rollback();

                    var message = LogMessages.CannotPerformActionOnGame("retrieve", id);
                    Log.Error(message);

                    return Result<bool>.Error(message);
                }

                game.IsActive = false;
                game.ModificationDate = DateTime.UtcNow;

                _repositoryUoW.GameRepository.Update(game);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.DeleteGameSuccess(game));
                return Result<bool>.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.DeleteGameError(ex));
                throw new InvalidOperationException($"Failed to delete game with id {id}. See logs for details.", ex);
            }
        }

        public async Task<List<GameResponseDto>> Get()
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                List<GameResponseDto> gameEntities = await _repositoryUoW.GameRepository.Get();
                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetAllGameSuccess());
                return gameEntities;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetAllGameError(ex));
                throw new InvalidOperationException("Error to loading the list Games. See logs for details.", ex);
            }
        }

        public async Task<Result<GameResponseDto>> GetById(int id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var game = await _repositoryUoW.GameRepository.GetById(id);

                if (game is null)
                {
                    transaction.Rollback();

                    var message = LogMessages.CannotPerformActionOnGame("retrieve", id);
                    Log.Error(message);

                    return Result<GameResponseDto>.Error(message);
                }

                var gameResponse = new GameResponseDto
                {
                    Id = game.Id,
                    Name = game.Name ?? string.Empty,
                    Description = game.Description,
                    Price = game.Price,
                    IsActive = game.IsActive,
                    CreateDate = game.CreateDate,
                    ModificationDate = game.ModificationDate
                };

                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetByGameIdSuccess(game));
                return Result<GameResponseDto>.Ok(gameResponse);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetByGameIdError(ex));
                throw new InvalidOperationException("Error retrieving the game. See inner exception for details.", ex);
            }
        }

        private async Task<Result<GameEntity>> IsValidGameRequest(GameEntity gameEntity)
        {
            var requestValidator = await new GameRequestValidator().ValidateAsync(gameEntity);

            if (!requestValidator.IsValid)
            {
                string errorMessage = string.Join(" ", requestValidator.Errors.Select(e => e.ErrorMessage));
                errorMessage = errorMessage.Replace(Environment.NewLine, "");
                return Result<GameEntity>.Error(errorMessage);
            }

            return Result<GameEntity>.Ok();
        }
    }
}