namespace CatalogAPI.Application.Contracts.Dto.GameCatalog
{
    public class CreateGameCatalogRequest
    {
        public int GameId { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Developer { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public List<string> Platforms { get; set; } = [];
        public List<string> Tags { get; set; } = [];
    }
}