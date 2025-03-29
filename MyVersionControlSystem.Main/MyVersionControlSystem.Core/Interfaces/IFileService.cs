namespace MyVersionControlSystem.Core.Interfaces
{
    public interface IFileService
    {
        string GetFileContent(string repositoryName, string branchName, string filePath);
        void AddFile(string repositoryName, string branchName, string filePath, string content);
        void UpdateFile(string repositoryName, string branchName, string filePath, string content);
        void DeleteFile(string repositoryName, string branchName, string filePath);
        bool FileExists(string repositoryName, string branchName, string filePath);
    }
}