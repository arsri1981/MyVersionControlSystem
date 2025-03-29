namespace MyVersionControlSystem.Services
{
    using MyVersionControlSystem.Core.Interfaces;
    using MyVersionControlSystem.Core.Models;
    using MyVersionControlSystem.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BranchService : IBranchService
    {
        private readonly IRepositoryStorage _repositoryStorage;
        private readonly IFileService _fileService;

        public BranchService(IRepositoryStorage repositoryStorage, IFileService fileService)
        {
            _repositoryStorage = repositoryStorage;
            _fileService = fileService;
        }

        public Branch CreateBranch(string repositoryName, string branchName, string? sourceBranchName = null)
        {
            if (string.IsNullOrEmpty(repositoryName))
            {
                throw new ArgumentException("Repository name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentException("Branch name cannot be null or empty.");
            }

            var repository = _repositoryStorage.GetRepository(repositoryName);
            if (repository == null)
            {
                throw new ArgumentException($"Repository '{repositoryName}' not found.");
            }

            if (_repositoryStorage.GetBranch(repositoryName, branchName) != null)
            {
                throw new ArgumentException($"Branch '{branchName}' already exists in repository '{repositoryName}'.");
            }

            var newBranch = new Branch(branchName, repositoryName);

            if (sourceBranchName != null)
            {
                var sourceBranch = _repositoryStorage.GetBranch(repositoryName, sourceBranchName);
                if (sourceBranch == null)
                {
                    throw new ArgumentException($"Source branch '{sourceBranchName}' not found in repository '{repositoryName}'.");
                }

                // Copy files from the source branch to the new branch
                CopyFilesFromBranch(repositoryName, sourceBranchName, branchName);

                //Set HeadCommitHash
                newBranch.HeadCommitHash = sourceBranch.HeadCommitHash;
            }

            _repositoryStorage.SaveBranch(repositoryName, newBranch);
            return newBranch;
        }

        public Branch? GetBranch(string repositoryName, string branchName)
        {
            return _repositoryStorage.GetBranch(repositoryName, branchName);
        }

        public List<Branch> GetBranches(string repositoryName)
        {
            return _repositoryStorage.GetBranches(repositoryName);
        }

        public void DeleteBranch(string repositoryName, string branchName)
        {
            var branch = _repositoryStorage.GetBranch(repositoryName, branchName);
            if (branch == null)
            {
                throw new ArgumentException($"Branch '{branchName}' not found in repository '{repositoryName}'.");
            }

            _repositoryStorage.DeleteBranch(repositoryName, branchName);
        }

        public void MergeBranch(string repositoryName, string sourceBranchName, string targetBranchName)
        {
            var sourceBranch = _repositoryStorage.GetBranch(repositoryName, sourceBranchName);
            if (sourceBranch == null)
            {
                throw new ArgumentException($"Source branch '{sourceBranchName}' not found in repository '{repositoryName}'.");
            }

            var targetBranch = _repositoryStorage.GetBranch(repositoryName, targetBranchName);
            if (targetBranch == null)
            {
                throw new ArgumentException($"Target branch '{targetBranchName}' not found in repository '{repositoryName}'.");
            }

            // Get all files from the source branch
            var sourceFiles = GetAllFilesFromBranch(repositoryName, sourceBranchName);

            // Get all files from the target branch
            var targetFiles = GetAllFilesFromBranch(repositoryName, targetBranchName);

            // Identify files that have been added, modified, or deleted in the source branch
            var addedFiles = sourceFiles.Except(targetFiles).ToList();
            var modifiedFiles = sourceFiles.Intersect(targetFiles).ToList();
            var deletedFiles = targetFiles.Except(sourceFiles).ToList();

            // Merge the changes into the target branch
            foreach (var file in addedFiles)
            {
                var content = _fileService.GetFileContent(repositoryName, sourceBranchName, file);
                _fileService.AddFile(repositoryName, targetBranchName, file, content);
            }

            foreach (var file in modifiedFiles)
            {
                var content = _fileService.GetFileContent(repositoryName, sourceBranchName, file);
                _fileService.UpdateFile(repositoryName, targetBranchName, file, content);
            }

            foreach (var file in deletedFiles)
            {
                _fileService.DeleteFile(repositoryName, targetBranchName, file);
            }
        }

        // Helper method to copy files from one branch to another
        private void CopyFilesFromBranch(string repositoryName, string sourceBranchName, string targetBranchName)
        {
            var sourceFiles = GetAllFilesFromBranch(repositoryName, sourceBranchName);

            foreach (var file in sourceFiles)
            {
                var content = _fileService.GetFileContent(repositoryName, sourceBranchName, file);
                _fileService.AddFile(repositoryName, targetBranchName, file, content);
            }
        }

        // Helper method to get all files from a branch
        private List<string> GetAllFilesFromBranch(string repositoryName, string branchName)
        {
            // This is a simplified implementation that only returns the root-level files.
            // A more complete implementation would recursively search through all directories.
            string branchDirectory = Path.Combine("C:\\MyVersionControl", repositoryName, "branches", branchName);

            if (!Directory.Exists(branchDirectory))
            {
                return new List<string>();
            }

            return Directory.GetFiles(branchDirectory).Select(Path.GetFileName).ToList()!;
        }
    }
}