
using CatalogAPI.Application.Abstractions.Search;
using CatalogAPI.Application.Contracts.Dto.GameCatalog;
using OpenSearch.Client;

namespace CatalogAPI.Infrastructure.Search
{
    public class OpenSearchGameSearchService : IGameSearchService
    {
        private const string IndexName = "game-catalog";

        private readonly IOpenSearchClient _client;

        public OpenSearchGameSearchService(IOpenSearchClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyCollection<GameCatalogDto>> SearchByGenreAsync(
            string genre,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(genre))
            {
                throw new ArgumentException(
                    "Genre cannot be null or empty.",
                    nameof(genre));
            }

            var response = await _client.SearchAsync<GameSearchDocument>(
                descriptor => descriptor
                    .Index(IndexName)
                    .Size(100)
                    .Query(query => query
                        .Match(match => match
                            .Field(game => game.Genre)
                            .Query(genre))),
                cancellationToken);

            if (!response.IsValid)
            {
                throw new InvalidOperationException(
                    "Failed to search games in OpenSearch: " +
                    response.DebugInformation);
            }

            return MapDocuments(response.Documents);
        }

        public async Task<IReadOnlyCollection<GameCatalogDto>> SearchAsync(
            GameSearchFilterDto filters,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filters);

            var conditions = new List<QueryContainer>();

            if (!string.IsNullOrWhiteSpace(filters.Genre))
            {
                conditions.Add(new TermQuery
                {
                    Field = "genre.keyword",
                    Value = filters.Genre.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(filters.Developer))
            {
                conditions.Add(new TermQuery
                {
                    Field = "developer.keyword",
                    Value = filters.Developer.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(filters.Publisher))
            {
                conditions.Add(new TermQuery
                {
                    Field = "publisher.keyword",
                    Value = filters.Publisher.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(filters.Platform))
            {
                conditions.Add(new TermQuery
                {
                    Field = "platforms.keyword",
                    Value = filters.Platform.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(filters.Tag))
            {
                conditions.Add(new TermQuery
                {
                    Field = "tags.keyword",
                    Value = filters.Tag.Trim()
                });
            }

            var response = await _client.SearchAsync<GameSearchDocument>(
                descriptor => descriptor
                    .Index(IndexName)
                    .Size(100)
                    .Query(query => query.Bool(boolean => boolean
                        .Filter(conditions.ToArray())
                        .Must(must => string.IsNullOrWhiteSpace(filters.Query)
                            ? must.MatchAll()
                            : must.MultiMatch(multi => multi
                                .Query(filters.Query.Trim())
                                .Fields(fields => fields
                                    .Field(game => game.Genre, 3.0)
                                    .Field(game => game.Developer, 2.0)
                                    .Field(game => game.Publisher, 2.0)
                                    .Field(game => game.Platforms, 2.0)
                                    .Field(game => game.Tags, 3.0))
                                .Fuzziness(Fuzziness.Auto)))))
                    .Sort(sort => sort
                        .Descending(SortSpecialField.Score)),
                cancellationToken);

            if (!response.IsValid)
            {
                throw new InvalidOperationException(
                    "Failed to search games in OpenSearch: " +
                    response.DebugInformation);
            }

            return MapDocuments(response.Documents);
        }

        private static IReadOnlyCollection<GameCatalogDto> MapDocuments(
            IEnumerable<GameSearchDocument> documents)
        {
            return documents
                .Select(game => new GameCatalogDto
                {
                    GameId = game.GameId,
                    Genre = game.Genre,
                    Developer = game.Developer,
                    Publisher = game.Publisher,
                    Platforms = game.Platforms,
                    Tags = game.Tags
                })
                .ToList();
        }
    }
}
