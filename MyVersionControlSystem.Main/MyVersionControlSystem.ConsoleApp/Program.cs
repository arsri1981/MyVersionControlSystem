using Microsoft.Extensions.DependencyInjection;
using MyVersionControlSystem.Core.Interfaces;
using MyVersionControlSystem.Services;
using MyVersionControlSystem.Data;
using System;

namespace MyVersionControlSystem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRepositoryStorage>(new FileSystemRepositoryStorage("C:\\MyVersionControl"))
                .AddSingleton<IRepositoryService, RepositoryService>()
                .AddSingleton<ICommitService, CommitService>()
                .AddSingleton<IBranchService, BranchService>()
                .AddSingleton<IFileService, FileService>()
                .BuildServiceProvider();

            var repositoryService = serviceProvider.GetService<IRepositoryService>()!;
            var commitService = serviceProvider.GetService<ICommitService>()!;
            var branchService = serviceProvider.GetService<IBranchService>()!;
            var fileService = serviceProvider.GetService<IFileService>()!;

            // Example usage
            Console.WriteLine("Welcome to MyVersionControl!");

            // Create a repository
            Console.Write("Enter repository name: ");
            string repoName = Console.ReadLine()!;
            string repoPath = "C:\\MyVersionControl\\" + repoName; // Adjust as needed

            if (!repositoryService.RepositoryExists(repoName))
            {
                var repo = repositoryService.CreateRepository(repoName, repoPath);
                Console.WriteLine($"Repository '{repo.Name}' created at '{repo.Path}'.");
            }
            else
            {
                Console.WriteLine($"Repository '{repoName}' already exists.");
            }

            // Create a branch
            Console.Write("Enter branch name: ");
            string branchName = Console.ReadLine()!;

            try
            {
                branchService.CreateBranch(repoName, branchName);
                Console.WriteLine($"Branch '{branchName}' created in repository '{repoName}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating branch: {ex.Message}");
            }

            // Add a file to the repository
            Console.Write("Enter file path: ");
            string filePath = Console.ReadLine()!;
            Console.Write("Enter file content: ");
            string fileContent = Console.ReadLine()!;

            try
            {
                fileService.AddFile(repoName, branchName, filePath, fileContent);
                Console.WriteLine($"File '{filePath}' added to repository '{repoName}' on branch '{branchName}'.");

                // Create a commit
                Console.Write("Enter commit message: ");
                string commitMessage = Console.ReadLine()!;

                //Create fileChanges to include in commit
                List<Core.Models.FileChange> fileChanges = new List<Core.Models.FileChange>() {
                    new Core.Models.FileChange()
                        {
                            ChangeType = "Added",
                            FilePath = filePath,
                            Content = fileContent
                        }
                };

                var commit = commitService.CreateCommit(repoName, branchName, commitMessage, fileChanges);
                Console.WriteLine($"Commit '{commit.Hash}' created on branch '{branchName}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding file: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}