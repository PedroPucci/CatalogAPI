using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Domain.Entities;

namespace CatalogAPI.Application.Abstractions.Repositories
{
    public interface IGameRepository
    {
        Task<GameEntity> Add(GameEntity gameEntity);
        GameEntity Update(GameEntity gameEntity);
        Task<bool> Delete(int id);
        Task<List<GameResponseDto>> Get();
        Task<GameEntity?> GetById(int id);
    }
}