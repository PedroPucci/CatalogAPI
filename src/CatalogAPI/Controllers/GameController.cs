using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Contracts.Dto;
using CatalogAPI.Domain.Entities;
using CatalogAPI.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace CatalogAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly IUnitOfWorkService _uow;
        private readonly OrderPlacedEventPublisher _orderPlacedEventPublisher;

        public GameController(
            IUnitOfWorkService uow,
            OrderPlacedEventPublisher orderPlacedEventPublisher)
        {
            _uow = uow;
            _orderPlacedEventPublisher = orderPlacedEventPublisher;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("{gameId:int}/purchase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Purchase([FromRoute] int gameId)
        {
            var result = await _uow.GameService.GetById(gameId);

            if (!result.Success || result.Data is null)
                return NotFound(result);

            var game = result.Data;

            if (!game.IsActive)
                return BadRequest("Game is not active.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Usuário não autenticado.");

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("E-mail do usuário não encontrado no token.");

            await _orderPlacedEventPublisher.Publish(
                userId,
                email,
                game.Id,
                (decimal)game.Price);

            return Ok(new
            {
                UserId = userId,
                GameId = game.Id,
                Price = game.Price,
                Status = "OrderPlaced"
            });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateGameRequestDto updateGameRequest)
        {
            var result = await _uow.GameService.Update(id, updateGameRequest);

            if (!result.Success)
                return NotFound(result);

            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _uow.GameService.Delete(id);
            return NoContent();
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<GameEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var result = await _uow.GameService.Get();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GameEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _uow.GameService.GetById(id);
            return Ok(result);
        }
    }
}