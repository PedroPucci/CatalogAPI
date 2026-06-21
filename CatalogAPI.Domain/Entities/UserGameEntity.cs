namespace CatalogAPI.Domain.Entities
{
    public class UserGameEntity
    {

        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int GameId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public GameEntity? Game { get; set; }
    }
}