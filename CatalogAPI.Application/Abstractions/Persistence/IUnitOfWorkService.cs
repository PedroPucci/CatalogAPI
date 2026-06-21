using CatalogAPI.Application.Services;

namespace CatalogAPI.Application.Abstractions.Persistence
{
    public interface IUnitOfWorkService
    {
        GameService GameService { get; }
    }
}