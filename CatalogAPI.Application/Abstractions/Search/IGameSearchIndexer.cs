namespace CatalogAPI.Application.Abstractions.Search
{
    public interface IGameSearchIndexer
    {
        Task IndexAsync(
            int gameId,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int gameId,
            CancellationToken cancellationToken = default);
    }
}
