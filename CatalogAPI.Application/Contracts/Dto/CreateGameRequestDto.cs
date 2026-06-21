namespace CatalogAPI.Application.Contracts.Dto
{
    public class CreateGameRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}