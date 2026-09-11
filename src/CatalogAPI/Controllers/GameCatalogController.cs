using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Contracts.Dto.GameCatalog;
using CatalogAPI.Application.Contracts.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CatalogAPI.Controllers
{
    [ApiController]
    [Route("api/game-catalog")]
    public class GameCatalogController : ControllerBase
    {
        private readonly IGameCatalogRepository _repository;

        public GameCatalogController(IGameCatalogRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateGameCatalogRequest request)
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

            var response = new GameCatalogResponse
            {
                GameId = catalog.GameId,
                Genre = catalog.Genre,
                Developer = catalog.Developer,
                Publisher = catalog.Publisher,
                Platforms = catalog.Platforms,
                Tags = catalog.Tags
            };

            return CreatedAtAction(
                nameof(GetByGameId),
                new { gameId = catalog.GameId },
                response);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _repository.GetAllAsync();

            var response = documents.Select(document => new GameCatalogResponse
            {
                GameId = document.GameId,
                Genre = document.Genre,
                Developer = document.Developer,
                Publisher = document.Publisher,
                Platforms = document.Platforms,
                Tags = document.Tags
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{gameId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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

            var response = new GameCatalogResponse
            {
                GameId = document.GameId,
                Genre = document.Genre,
                Developer = document.Developer,
                Publisher = document.Publisher,
                Platforms = document.Platforms,
                Tags = document.Tags
            };

            return Ok(response);
        }
    }
}