using System.Text.Json;
using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Contracts.Dto.GameCatalog;
using CatalogAPI.Application.Contracts.DomainErrors;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace CatalogAPI.Infrastructure.MongoDB
{
    public class GameCatalogRepository : IGameCatalogRepository
    {
        private readonly IMongoCollection<GameCatalogDocument> _collection;
        private readonly IDistributedCache _cache;

        private const string CacheKeyPrefix = "GameCatalog:";

        public GameCatalogRepository(
            MongoDbContext context,
            IDistributedCache cache)
        {
            _collection = context.Database
                .GetCollection<GameCatalogDocument>("gameCatalog");

            _cache = cache;
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

                var cacheKey = GetCacheKey(catalog.GameId);

                await _cache.RemoveAsync(cacheKey);

                await _cache.RemoveAsync(
                    GetAllCacheKey());
            }
            catch (MongoWriteException ex)
                when (ex.WriteError?.Category ==
                      ServerErrorCategory.DuplicateKey)
            {
                throw new GameCatalogAlreadyExistsException(
                    catalog.GameId);
            }
        }

        public async Task<GameCatalogDto?> GetByGameIdAsync(int gameId)
        {
            var cacheKey = GetCacheKey(gameId);

            var cachedCatalog =
                await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedCatalog))
            {
                return JsonSerializer.Deserialize<GameCatalogDto>(
                    cachedCatalog);
            }

            var document = await _collection
                .Find(x => x.GameId == gameId)
                .FirstOrDefaultAsync();

            if (document is null)
                return null;

            var catalog = MapToDto(document);

            var cacheOptions =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10)
                };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(catalog),
                cacheOptions);

            return catalog;
        }

        public async Task<List<GameCatalogDto>> GetAllAsync()
        {
            var cacheKey = GetAllCacheKey();

            var cachedCatalogs =
                await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrWhiteSpace(cachedCatalogs))
            {
                return JsonSerializer.Deserialize<
                           List<GameCatalogDto>>(
                           cachedCatalogs)
                       ?? [];
            }

            var documents = await _collection
                .Find(_ => true)
                .ToListAsync();

            var catalogs = documents
                .Select(MapToDto)
                .ToList();

            var cacheOptions =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10)
                };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(catalogs),
                cacheOptions);

            return catalogs;
        }

        private static GameCatalogDto MapToDto(
            GameCatalogDocument document)
        {
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

        private static string GetCacheKey(int gameId)
        {
            return $"{CacheKeyPrefix}{gameId}";
        }

        private static string GetAllCacheKey()
        {
            return $"{CacheKeyPrefix}All";
        }
    }
}