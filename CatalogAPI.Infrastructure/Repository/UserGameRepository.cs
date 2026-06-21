using CatalogAPI.Application.Abstractions.Repositories;
using CatalogAPI.Domain.Entities;
using CatalogAPI.Infrastructure.Connections;

namespace CatalogAPI.Infrastructure.Repository
{
    public class UserGameRepository : IUserGameRepository
    {
        private readonly DataContext _context;

        public UserGameRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserGameEntity> Add(UserGameEntity userGameEntity)
        {
            var result = await _context.UserGames.AddAsync(userGameEntity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}