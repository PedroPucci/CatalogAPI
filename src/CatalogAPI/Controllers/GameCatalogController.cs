
using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Search;
using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Application.Contracts.Dto.GameCatalog;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers
{
    [ApiController]
    [Route("api/game-catalog")]
    public class GameCatalogController : ControllerBase
    {
        private readonly IGameCatalogRepository _repository;
        private readonly IGameSearchIndexer _searchIndexer;
        private readonly IGameSearchService _searchService;
        private readonly ILogger<GameCatalogController> _logger;

        public GameCatalogController(
            IGameCatalogRepository repository,
            IGameSearchIndexer searchIndexer,
            IGameSearchService searchService,
            ILogger<GameCatalogController> logger)
        {
            _repository = repository;
            _searchIndexer = searchIndexer;
            _searchService = searchService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateGameCatalogRequest request)
        {
            var catalog = new GameCatalogDto
            {
                GameId = request.GameId,
                Genre = request.Genre,
                Developer = request.Developer,
                Publisher = request.Publisher,
                Platforms = request.Platforms,
                Tags = request.Tags
            };

            await _repository.AddAsync(catalog);

            try
            {
                await _searchIndexer.IndexAsync(catalog.GameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to index game {GameId} in OpenSearch.",
                    catalog.GameId);
            }

            var response = MapToResponse(catalog);

            return CreatedAtAction(
                nameof(GetByGameId),
                new { gameId = catalog.GameId },
                response);
        }

        [HttpPut("{gameId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Update(
            int gameId,
            [FromBody] UpdateGameCatalogRequest request)
        {
            var catalog = new GameCatalogDto
            {
                GameId = gameId,
                Genre = request.Genre,
                Developer = request.Developer,
                Publisher = request.Publisher,
                Platforms = request.Platforms,
                Tags = request.Tags
            };

            var updated = await _repository.UpdateAsync(
                gameId,
                catalog);

            if (!updated)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Game catalog not found."
                });
            }

            try
            {
                await _searchIndexer.IndexAsync(gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to synchronize game {GameId} with OpenSearch.",
                    gameId);

                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status503ServiceUnavailable,
                        Message = "Game catalog updated, but search synchronization failed."
                    });
            }

            return Ok(MapToResponse(catalog));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _repository.GetAllAsync();

            var response = documents
                .Select(MapToResponse)
                .ToList();

            return Ok(response);
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] GameSearchFilterDto filters,
            CancellationToken cancellationToken)
        {
            var documents = await _searchService.SearchAsync(
                filters,
                cancellationToken);

            var response = documents
                .Select(MapToResponse)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{gameId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ErrorResponse),
            StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByGameId(int gameId)
        {
            var document = await _repository.GetByGameIdAsync(gameId);

            if (document is null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Game catalog not found."
                });
            }

            return Ok(MapToResponse(document));
        }

        private static GameCatalogResponse MapToResponse(
            GameCatalogDto document)
        {
            return new GameCatalogResponse
            {
                GameId = document.GameId,
                Genre = document.Genre,
                Developer = document.Developer,
                Publisher = document.Publisher,
                Platforms = document.Platforms,
                Tags = document.Tags
            };
        }
    }
}
