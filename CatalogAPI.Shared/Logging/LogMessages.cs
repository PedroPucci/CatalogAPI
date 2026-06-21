using CatalogAPI.Domain.Entities;

namespace CatalogAPI.Shared.Logging
{
    public static class LogMessages
    {
        #region Game Validation

        public static string InvalidGameInputs() => "Invalid game data.";

        #endregion

        #region Game Not Found

        public static string CannotPerformActionOnGame(string action, int gameId) => $"Cannot {action} Game. Game with id {gameId} was not found.";

        #endregion

        #region Game CRUD

        public static string AddingGameError(Exception ex) => $"Error adding Game. Details: {ex.Message}";
        public static string AddingGameSuccess(GameEntity gameEntity) => $"Game name:{gameEntity.Name} added successfully.";

        public static string UpdatingErrorGame(Exception ex) => $"Error updating Game. Details: {ex.Message}";
        public static string UpdatingSuccessGame(GameEntity gameEntity) => $"Game name:{gameEntity.Name} updated successfully.";

        public static string DeleteGameError(Exception ex) => $"Error deleting Game. Details: {ex.Message}";
        public static string DeleteGameSuccess(GameEntity gameEntity) => $"Game name:{gameEntity.Name} - id:{gameEntity.Id} deleted successfully.";

        public static string GetAllGameError(Exception ex) => $"Error retrieving Games list. Details: {ex.Message}";
        public static string GetAllGameSuccess() => "Games retrieved successfully.";

        public static string GetByGameIdError(Exception ex) => $"Error retrieving Game by id. Details: {ex.Message}";
        public static string GetByGameIdSuccess(GameEntity gameEntity) => $"Game name:{gameEntity.Name} retrieved successfully.";

        #endregion
    }
}