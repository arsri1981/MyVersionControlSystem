namespace MyVersionControlSystem.Services
{
    using MyVersionControlSystem.Core.Interfaces;
    using MyVersionControlSystem.Core.Models;
    using MyVersionControlSystem.Core.Utils;
    using MyVersionControlSystem.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CommitService : ICommitService
    {
        private readonly IRepositoryStorage _repositoryStorage;
        private readonly IBranchService _branchService;
        private readonly IFileService _fileService;

        public CommitService(IRepositoryStorage repositoryStorage, IBranchService branchService, IFileService fileService)
        {
            _repositoryStorage = repositoryStorage;
            _branchService = branchService;
            _fileService = fileService;
        }

        public Commit CreateCommit(string repositoryName, string branchName, string message, List<FileChange> fileChanges)
        {
            var branch = _branchService.GetBranch(repositoryName, branchName);
            if (branch == null)
            {
                throw new ArgumentException($"Branch '{branchName}' not found in repository '{repositoryName}'.");
            }

            var commit = new Commit(message, Environment.UserName);

            // Get the hash of the previous commit, if it exists
            commit.PreviousCommitHash = branch.HeadCommitHash;

            //Generate hash from commit properties
            string commitData = $"{commit.Message}{commit.Timestamp}{commit.Author}{commit.PreviousCommitHash}";
            commit.Hash = StringHasher.GenerateSHA256Hash(commitData);

            commit.FileChanges = fileChanges;

            //Save File changes
            foreach (var fileChange in fileChanges)
            {
                if (fileChange.ChangeType == "Added")
                {
                    _fileService.AddFile(repositoryName, branchName, fileChange.FilePath, fileChange.Content!);
                }
                else if (fileChange.ChangeType == "Modified")
                {
                    _fileService.UpdateFile(repositoryName, branchName, fileChange.FilePath, fileChange.Content!);
                }
                else if (fileChange.ChangeType == "Deleted")
                {
                    _fileService.DeleteFile(repositoryName, branchName, fileChange.FilePath);
                }
            }

            // Save the commit to the storage
            _repositoryStorage.SaveCommit(repositoryName, branchName, commit);

            // Update the branch's HEAD to point to the new commit
            branch.HeadCommitHash = commit.Hash;
            _repositoryStorage.SaveBranch(repositoryName, branch);

            return commit;
        }

        public Commit? GetCommit(string repositoryName, string commitHash)
        {
            return _repositoryStorage.GetCommit(repositoryName, commitHash);
        }

        public List<Commit> GetCommitsForBranch(string repositoryName, string branchName)
        {
            //Implement Commit List Ordering later by "Timestamp" property and the "PreviousCommitHash"
            return _repositoryStorage.GetCommitsForBranch(repositoryName, branchName);
        }
    }
}