namespace MyVersionControlSystem.Core.Models
{
    public class Branch
    {
        public string Name { get; set; }
        public string RepositoryName { get; set; }
        public string? HeadCommitHash { get; set; } // The hash of the latest commit on this branch

        public Branch(string name, string repositoryName)
        {
            Name = name;
            RepositoryName = repositoryName;
        }
    }
}