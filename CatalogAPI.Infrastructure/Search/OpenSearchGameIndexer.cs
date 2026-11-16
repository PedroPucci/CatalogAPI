
using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Search;
using OpenSearch.Client;

namespace CatalogAPI.Infrastructure.Search
{
    public class OpenSearchGameIndexer : IGameSearchIndexer
    {
        private const string IndexName = "game-catalog";

        private readonly IOpenSearchClient _client;
        private readonly IGameCatalogRepository _repository;

        public OpenSearchGameIndexer(
            IOpenSearchClient client,
            IGameCatalogRepository repository)
        {
            _client = client;
            _repository = repository;
        }

        public async Task IndexAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            var catalog = await _repository
                .GetByGameIdAsync(gameId);

            if (catalog is null)
            {
                throw new InvalidOperationException(
                    $"Game catalog {gameId} was not found.");
            }

            var document = new GameSearchDocument
            {
                GameId = catalog.GameId,
                Genre = catalog.Genre,
                Developer = catalog.Developer,
                Publisher = catalog.Publisher,
                Platforms = catalog.Platforms,
                Tags = catalog.Tags
            };

            var response = await _client.IndexAsync(
                document,
                descriptor => descriptor
                    .Index(IndexName)
                    .Id(gameId),
                cancellationToken);

            if (!response.IsValid)
            {
                throw new InvalidOperationException(
                    $"Failed to index game {gameId}: " +
                    response.DebugInformation);
            }
        }

        public async Task DeleteAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync<GameSearchDocument>(
                gameId,
                descriptor => descriptor.Index(IndexName),
                cancellationToken);

            if (!response.IsValid &&
                response.ServerError?.Status != 404)
            {
                throw new InvalidOperationException(
                    $"Failed to delete game {gameId}: " +
                    response.DebugInformation);
            }
        }
    }
}