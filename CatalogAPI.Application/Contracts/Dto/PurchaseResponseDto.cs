namespace CatalogAPI.Application.Contracts.Dto
{
    public class PurchaseResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public int GameId { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
    }
}