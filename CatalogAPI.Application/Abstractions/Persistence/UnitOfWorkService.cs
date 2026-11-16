using CatalogAPI.Application.Services;

namespace CatalogAPI.Application.Abstractions.Persistence
{
    public class UnitOfWorkService : IUnitOfWorkService
    {
        private readonly IRepositoryUoW _repositoryUoW;
        private GameService? _gameService;

        public UnitOfWorkService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public GameService GameService
        {
            get
            {
                if (_gameService is null)
                {
                    _gameService = new GameService(_repositoryUoW);
                }

                return _gameService;
            }
        }
    }
}