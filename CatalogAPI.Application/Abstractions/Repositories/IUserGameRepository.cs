using CatalogAPI.Domain.Entities;

namespace CatalogAPI.Application.Abstractions.Repositories
{
    public interface IUserGameRepository
    {
        Task<UserGameEntity> Add(UserGameEntity userGameEntity);
    }
}