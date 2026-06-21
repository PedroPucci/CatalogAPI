using CatalogAPI.Application.Services;

namespace CatalogAPI.Application.Abstractions.Persistence
{
    public class UnitOfWorkService : IUnitOfWorkService
    {
        private readonly IRepositoryUoW _repositoryUoW;
        private GameService gameService;

        public UnitOfWorkService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public GameService GameService
        {
            get
            {
                if (gameService is null)
                    gameService = new GameService(_repositoryUoW);
                return gameService;
            }
        }
    }
}