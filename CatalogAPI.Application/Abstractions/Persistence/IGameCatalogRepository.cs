using CatalogAPI.Application.Contracts.Dto.GameCatalog;

namespace CatalogAPI.Application.Abstractions.Persistence
{
    public interface IGameCatalogRepository
    {
        Task AddAsync(GameCatalogDto document);
        Task<GameCatalogDto?> GetByGameIdAsync(int gameId);
        Task<List<GameCatalogDto>> GetAllAsync();
    }
}