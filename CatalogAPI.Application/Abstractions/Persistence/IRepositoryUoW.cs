using CatalogAPI.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CatalogAPI.Application.Abstractions.Persistence
{
    public interface IRepositoryUoW
    {
        IGameRepository GameRepository { get; }
        IUserGameRepository UserGameRepository { get; }

        Task SaveAsync();
        void Commit();
        IDbContextTransaction BeginTransaction();
    }
}