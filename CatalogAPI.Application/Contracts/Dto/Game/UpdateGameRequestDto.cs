namespace CatalogAPI.Application.Contracts.Dto.Game
{
    public class UpdateGameRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
    }
}