using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Contracts.Dto.GameCatalog;
using CatalogAPI.Application.Contracts.DomainErrors;
using MongoDB.Driver;

namespace CatalogAPI.Infrastructure.MongoDB
{
    public class GameCatalogRepository : IGameCatalogRepository
    {
        private readonly IMongoCollection<GameCatalogDocument> _collection;

        public GameCatalogRepository(MongoDbContext context)
        {
            _collection = context.Database
                .GetCollection<GameCatalogDocument>("gameCatalog");
        }

        public async Task AddAsync(GameCatalogDto catalog)
        {
            var document = new GameCatalogDocument
            {
                GameId = catalog.GameId,
                Genre = catalog.Genre,
                Developer = catalog.Developer,
                Publisher = catalog.Publisher,
                Platforms = catalog.Platforms,
                Tags = catalog.Tags
            };

            try
            {
                await _collection.InsertOneAsync(document);
            }
            catch (MongoWriteException ex)
                when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new GameCatalogAlreadyExistsException(catalog.GameId);
            }
        }

        public async Task<GameCatalogDto?> GetByGameIdAsync(int gameId)
        {
            var document = await _collection
                .Find(x => x.GameId == gameId)
                .FirstOrDefaultAsync();

            if (document is null)
                return null;

            return new GameCatalogDto
            {
                GameId = document.GameId,
                Genre = document.Genre ?? string.Empty,
                Developer = document.Developer ?? string.Empty,
                Publisher = document.Publisher ?? string.Empty,
                Platforms = document.Platforms ?? [],
                Tags = document.Tags ?? []
            };
        }

        public async Task<List<GameCatalogDto>> GetAllAsync()
        {
            var documents = await _collection
                .Find(_ => true)
                .ToListAsync();

            return documents
                .Select(document => new GameCatalogDto
                {
                    GameId = document.GameId,
                    Genre = document.Genre ?? string.Empty,
                    Developer = document.Developer ?? string.Empty,
                    Publisher = document.Publisher ?? string.Empty,
                    Platforms = document.Platforms ?? [],
                    Tags = document.Tags ?? []
                })
                .ToList();
        }
    }
}