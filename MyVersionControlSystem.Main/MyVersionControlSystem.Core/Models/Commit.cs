namespace MyVersionControlSystem.Core.Models
{
    using System;
    using System.Collections.Generic;

    public class Commit
    {
        public string Hash { get; set; }  // SHA-256 hash of the content & metadata
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Author { get; set; } // User who made the commit
        public List<FileChange> FileChanges { get; set; } = new List<FileChange>();
        public string? PreviousCommitHash { get; set; } // Hash of the previous commit on the branch

        public Commit(string message, string author)
        {
            Message = message;
            Timestamp = DateTime.UtcNow;
            Author = author;
        }
    }
}