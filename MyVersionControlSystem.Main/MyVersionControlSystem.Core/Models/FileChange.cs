namespace MyVersionControlSystem.Core.Models
{
    public class FileChange
    {
        public string FilePath { get; set; }
        public string ChangeType { get; set; } // "Added", "Modified", "Deleted"
        public string? Content { get; set; } // New content of the file (if applicable)
    }
}