namespace MyVersionControlSystem.Core.Interfaces
{
    using MyVersionControlSystem.Core.Models;

    public interface IBranchService
    {
        Branch CreateBranch(string repositoryName, string branchName, string? sourceBranchName = null);  // Create from existing branch
        Branch? GetBranch(string repositoryName, string branchName);
        List<Branch> GetBranches(string repositoryName);
        void DeleteBranch(string repositoryName, string branchName);
        void MergeBranch(string repositoryName, string sourceBranchName, string targetBranchName);
    }
}