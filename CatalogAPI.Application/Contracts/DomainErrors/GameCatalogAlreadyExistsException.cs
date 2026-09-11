using CatalogAPI.Shared.Helpers;

namespace CatalogAPI.Application.Contracts.DomainErrors
{
    public class GameCatalogAlreadyExistsException : Exception
    {
        public int GameId { get; }

        public GameCatalogAlreadyExistsException(int gameId)
            : base(GameErrors.Game_Error_CatalogAlreadyExists.Description())
        {
            GameId = gameId;
        }
    }
}