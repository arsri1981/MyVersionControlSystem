namespace MyVersionControlSystem.Core.Interfaces
{
    using MyVersionControlSystem.Core.Models;

    public interface ICommitService
    {
        Commit CreateCommit(string repositoryName, string branchName, string message, List<FileChange> fileChanges);
        Commit? GetCommit(string repositoryName, string commitHash);
        List<Commit> GetCommitsForBranch(string repositoryName, string branchName);
    }
}