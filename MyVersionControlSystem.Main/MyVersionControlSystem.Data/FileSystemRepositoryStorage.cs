namespace MyVersionControlSystem.Data
{
    using MyVersionControlSystem.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    public class FileSystemRepositoryStorage : IRepositoryStorage
    {
        private readonly string _basePath;

        public FileSystemRepositoryStorage(string basePath)
        {
            _basePath = basePath; // e.g., "C:\\MyVersionControl"
            Directory.CreateDirectory(_basePath);
        }

        private string GetRepositoryDirectory(string repositoryName)
        {
            return Path.Combine(_basePath, repositoryName);
        }

        private string GetBranchDirectory(string repositoryName, string branchName)
        {
            return Path.Combine(GetRepositoryDirectory(repositoryName), "branches", branchName);
        }

        private string GetCommitsDirectory(string repositoryName, string branchName)
        {
            return Path.Combine(GetBranchDirectory(repositoryName, branchName), "commits");
        }

        private string GetCommitFilePath(string repositoryName, string branchName, string commitHash)
        {
            return Path.Combine(GetCommitsDirectory(repositoryName, branchName), $"{commitHash}.json");
        }

        // Repository Methods
        public void SaveRepository(Repository repository)
        {
            string repoDirectory = GetRepositoryDirectory(repository.Name);
            Directory.CreateDirectory(repoDirectory);

            //Save Repository Metadata to repository.json file
            string repoMetadataPath = Path.Combine(repoDirectory, "repository.json");
            string jsonString = JsonSerializer.Serialize(repository);
            File.WriteAllText(repoMetadataPath, jsonString);

            // Create default branch (e.g., "main")
            SaveBranch(repository.Name, new Branch(repository.DefaultBranchName, repository.Name));
        }

        public Repository? GetRepository(string name)
        {
            string repoDirectory = GetRepositoryDirectory(name);
            if (!Directory.Exists(repoDirectory))
            {
                return null;
            }

            string repoMetadataPath = Path.Combine(repoDirectory, "repository.json");
            if (!File.Exists(repoMetadataPath))
            {
                return null;
            }

            string jsonString = File.ReadAllText(repoMetadataPath);
            return JsonSerializer.Deserialize<Repository>(jsonString);
        }

        public bool RepositoryExists(string name)
        {
            string repoDirectory = GetRepositoryDirectory(name);
            return Directory.Exists(repoDirectory);
        }

        public List<Repository> GetAllRepositories()
        {
            List<Repository> repositories = new List<Repository>();
            foreach (var directory in Directory.GetDirectories(_basePath))
            {
                string repositoryName = Path.GetFileName(directory);
                var repository = GetRepository(repositoryName);
                if (repository != null)
                {
                    repositories.Add(repository);
                }
            }
            return repositories;
        }

        // Commit Methods
        public void SaveCommit(string repositoryName, string branchName, Commit commit)
        {
            string commitsDirectory = GetCommitsDirectory(repositoryName, branchName);
            Directory.CreateDirectory(commitsDirectory);

            string commitFilePath = GetCommitFilePath(repositoryName, branchName, commit.Hash);
            string jsonString = JsonSerializer.Serialize(commit);
            File.WriteAllText(commitFilePath, jsonString);
        }

        public Commit? GetCommit(string repositoryName, string commitHash)
        {
            foreach (var branch in GetBranches(repositoryName))
            {
                string commitFilePath = GetCommitFilePath(repositoryName, branch.Name, commitHash);
                if (File.Exists(commitFilePath))
                {
                    string jsonString = File.ReadAllText(commitFilePath);
                    return JsonSerializer.Deserialize<Commit>(jsonString);
                }
            }

            return null;
        }

        public List<Commit> GetCommitsForBranch(string repositoryName, string branchName)
        {
            string commitsDirectory = GetCommitsDirectory(repositoryName, branchName);
            List<Commit> commits = new List<Commit>();

            if (Directory.Exists(commitsDirectory))
            {
                foreach (var file in Directory.GetFiles(commitsDirectory, "*.json"))
                {
                    string jsonString = File.ReadAllText(file);
                    Commit? commit = JsonSerializer.Deserialize<Commit>(jsonString);
                    if (commit != null)
                    {
                        commits.Add(commit);
                    }
                }
            }

            return commits;
        }

        // Branch Methods
        public void SaveBranch(string repositoryName, Branch branch)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branch.Name);
            Directory.CreateDirectory(branchDirectory);

            //Save Branch Metadata to branch.json file
            string branchMetadataPath = Path.Combine(branchDirectory, "branch.json");
            string jsonString = JsonSerializer.Serialize(branch);
            File.WriteAllText(branchMetadataPath, jsonString);
        }

        public Branch? GetBranch(string repositoryName, string branchName)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branchName);
            if (!Directory.Exists(branchDirectory))
            {
                return null;
            }

            string branchMetadataPath = Path.Combine(branchDirectory, "branch.json");
            if (!File.Exists(branchMetadataPath))
            {
                return null;
            }

            string jsonString = File.ReadAllText(branchMetadataPath);
            return JsonSerializer.Deserialize<Branch>(jsonString);
        }

        public List<Branch> GetBranches(string repositoryName)
        {
            string repositoryDirectory = GetRepositoryDirectory(repositoryName);
            string branchesDirectory = Path.Combine(repositoryDirectory, "branches");
            List<Branch> branches = new List<Branch>();

            if (Directory.Exists(branchesDirectory))
            {
                foreach (var directory in Directory.GetDirectories(branchesDirectory))
                {
                    string branchName = Path.GetFileName(directory);
                    var branch = GetBranch(repositoryName, branchName);
                    if (branch != null)
                    {
                        branches.Add(branch);
                    }
                }
            }

            return branches;
        }

        public void DeleteBranch(string repositoryName, string branchName)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branchName);
            if (Directory.Exists(branchDirectory))
            {
                Directory.Delete(branchDirectory, true); //recursive delete
            }
        }

        // File Methods
        public string GetFileContent(string repositoryName, string branchName, string filePath)
        {
            string fileDirectory = GetBranchDirectory(repositoryName, branchName);
            string fullFilePath = Path.Combine(fileDirectory, filePath);

            if (File.Exists(fullFilePath))
            {
                return File.ReadAllText(fullFilePath);
            }

            return string.Empty;
        }

        public void SaveFileContent(string repositoryName, string branchName, string filePath, string content)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branchName);
            string fullFilePath = Path.Combine(branchDirectory, filePath);

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath)!);

            File.WriteAllText(fullFilePath, content);
        }

        public void DeleteFileContent(string repositoryName, string branchName, string filePath)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branchName);
            string fullFilePath = Path.Combine(branchDirectory, filePath);

            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
        }

        public bool FileExists(string repositoryName, string branchName, string filePath)
        {
            string branchDirectory = GetBranchDirectory(repositoryName, branchName);
            string fullFilePath = Path.Combine(branchDirectory, filePath);

            return File.Exists(fullFilePath);
        }
    }
}