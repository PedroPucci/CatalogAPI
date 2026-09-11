using MongoDB.Driver;

namespace CatalogAPI.Infrastructure.MongoDB
{
    public class MongoDbInitializer
    {
        private readonly MongoDbContext _context;

        public MongoDbInitializer(MongoDbContext context)
        {
            _context = context;
        }

        public async Task InitializeAsync()
        {
            var collection =
                _context.Database.GetCollection<GameCatalogDocument>("gameCatalog");

            var indexKeys = Builders<GameCatalogDocument>
                .IndexKeys
                .Ascending(x => x.GameId);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true
            };

            var indexModel =
                new CreateIndexModel<GameCatalogDocument>(
                    indexKeys,
                    indexOptions);

            await collection.Indexes.CreateOneAsync(indexModel);
        }
    }
}