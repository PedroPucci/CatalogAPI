
namespace CatalogAPI.Application.Contracts.Dto.GameCatalog
{
    public class GameSearchFilterDto
    {
        public string? Query { get; set; }

        public string? Genre { get; set; }

        public string? Developer { get; set; }

        public string? Publisher { get; set; }

        public string? Platform { get; set; }

        public string? Tag { get; set; }
    }
}