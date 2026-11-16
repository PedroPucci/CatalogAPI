using CatalogAPI.Application.Contracts.Dto.GameCatalog;

namespace CatalogAPI.Application.Abstractions.Search
{
    public interface IGameSearchService
    {
        Task<IReadOnlyCollection<GameCatalogDto>> SearchByGenreAsync(
            string genre,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<GameCatalogDto>> SearchAsync(
            GameSearchFilterDto filters,
            CancellationToken cancellationToken = default);
    }
}
