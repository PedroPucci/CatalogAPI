using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Domain.Common;
using CatalogAPI.Domain.Entities;

namespace CatalogAPI.Application.Abstractions.Services
{
    public interface IGameService
    {
        Task<Result<GameEntity>> Add(GameResponseDto gameResponse, string userId);
        Task<Result<bool>> Update(int id, UpdateGameRequestDto updateGameRequest);
        Task<Result<bool>> Delete(int id);
        Task<List<GameResponseDto>> Get();
        Task<Result<GameResponseDto>> GetById(int id);
    }
}