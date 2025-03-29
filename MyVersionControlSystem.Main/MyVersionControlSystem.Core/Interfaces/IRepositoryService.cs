namespace MyVersionControlSystem.Core.Interfaces
{
    using MyVersionControlSystem.Core.Models;

    public interface IRepositoryService
    {
        Repository CreateRepository(string name, string path);
        Repository? GetRepository(string name);
        bool RepositoryExists(string name);
        List<Repository> GetAllRepositories();
    }
}