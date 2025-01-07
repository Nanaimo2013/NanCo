using System.Collections.Generic;

namespace NanCo.Models
{
    public class ProjectConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string MainFile { get; set; } = string.Empty;
        public List<string> Dependencies { get; set; } = new();
        public Dictionary<string, string> Assets { get; set; } = new();
    }
} 