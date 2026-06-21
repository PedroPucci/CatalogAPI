namespace CatalogAPI.Domain.Entities
{
    public class GameEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModificationDate { get; set; }
    }
}