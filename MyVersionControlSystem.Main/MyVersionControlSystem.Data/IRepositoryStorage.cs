namespace MyVersionControlSystem.Data
{
    using MyVersionControlSystem.Core.Models;
    using System.Collections.Generic;

    public interface IRepositoryStorage
    {
        void SaveRepository(Repository repository);
        Repository? GetRepository(string name);
        bool RepositoryExists(string name);
        List<Repository> GetAllRepositories();

        void SaveCommit(string repositoryName, string branchName, Commit commit);
        Commit? GetCommit(string repositoryName, string commitHash);
        List<Commit> GetCommitsForBranch(string repositoryName, string branchName);

        void SaveBranch(string repositoryName, Branch branch);
        Branch? GetBranch(string repositoryName, string branchName);
        List<Branch> GetBranches(string repositoryName);
        void DeleteBranch(string repositoryName, string branchName);

        string GetFileContent(string repositoryName, string branchName, string filePath);
        void SaveFileContent(string repositoryName, string branchName, string filePath, string content);
        void DeleteFileContent(string repositoryName, string branchName, string filePath);
        bool FileExists(string repositoryName, string branchName, string filePath);

    }
}