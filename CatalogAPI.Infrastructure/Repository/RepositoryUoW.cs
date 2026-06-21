using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Repositories;
using CatalogAPI.Infrastructure.Connections;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace CatalogAPI.Infrastructure.Repository
{
    public class RepositoryUoW : IRepositoryUoW
    {
        private readonly DataContext _context;
        private bool _disposed = false;
        private IGameRepository? _gameEntityRepository = null;
        private IUserGameRepository? _userGameEntityRepository = null;

        public RepositoryUoW(DataContext context)
        {
            _context = context;
        }

        public IGameRepository GameRepository
        {
            get
            {
                if (_gameEntityRepository is null)
                {
                    _gameEntityRepository = new GameRepository(_context);
                }
                return _gameEntityRepository;
            }
        }

        public IUserGameRepository UserGameRepository
        {
            get
            {
                if (_userGameEntityRepository is null)
                {
                    _userGameEntityRepository = new UserGameRepository(_context);
                }
                return _userGameEntityRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error($"Database connection failed: {ex.Message}");
                throw new ApplicationException("Database is not available. Please check the connection.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}