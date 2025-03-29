namespace MyVersionControlSystem.Core.Models
{
    public class Repository
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string DefaultBranchName { get; set; } = "main"; // Default branch name

        public Repository(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}