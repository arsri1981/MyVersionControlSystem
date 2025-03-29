namespace MyVersionControlSystem.Services
{
    using MyVersionControlSystem.Core.Interfaces;
    using MyVersionControlSystem.Core.Models;
    using MyVersionControlSystem.Data;

    public class RepositoryService : IRepositoryService
    {
        private readonly IRepositoryStorage _repositoryStorage;

        public RepositoryService(IRepositoryStorage repositoryStorage)
        {
            _repositoryStorage = repositoryStorage;
        }

        public Repository CreateRepository(string name, string path)
        {
            var repository = new Repository(name, path);
            _repositoryStorage.SaveRepository(repository);
            return repository;
        }

        public Repository? GetRepository(string name)
        {
            return _repositoryStorage.GetRepository(name);
        }

        public bool RepositoryExists(string name)
        {
            return _repositoryStorage.RepositoryExists(name);
        }

        public List<Repository> GetAllRepositories()
        {
            return _repositoryStorage.GetAllRepositories();
        }
    }
}