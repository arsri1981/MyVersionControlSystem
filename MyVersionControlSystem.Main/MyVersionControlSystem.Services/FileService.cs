namespace MyVersionControlSystem.Services
{
    using MyVersionControlSystem.Core.Interfaces;
    using MyVersionControlSystem.Data;
    using System;

    public class FileService : IFileService
    {
        private readonly IRepositoryStorage _repositoryStorage;

        public FileService(IRepositoryStorage repositoryStorage)
        {
            _repositoryStorage = repositoryStorage;
        }

        public void AddFile(string repositoryName, string branchName, string filePath, string content)
        {
            _repositoryStorage.SaveFileContent(repositoryName, branchName, filePath, content);
        }

        public void DeleteFile(string repositoryName, string branchName, string filePath)
        {
            _repositoryStorage.DeleteFileContent(repositoryName, branchName, filePath);
        }

        public string GetFileContent(string repositoryName, string branchName, string filePath)
        {
            return _repositoryStorage.GetFileContent(repositoryName, branchName, filePath);
        }

        public void UpdateFile(string repositoryName, string branchName, string filePath, string content)
        {
            _repositoryStorage.SaveFileContent(repositoryName, branchName, filePath, content);
        }

        public bool FileExists(string repositoryName, string branchName, string filePath)
        {
            return _repositoryStorage.FileExists(repositoryName, branchName, filePath);
        }
    }
}