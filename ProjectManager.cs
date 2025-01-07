using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NanCo.Models;
using NanCo.FileSystem;

namespace NanCo
{
    public class ProjectManager
    {
        public static void CreateProject(string name)
        {
            Console.Write("Project Type (C# or NS): ");
            string projectType = Console.ReadLine()?.ToUpper() ?? "NS";
            
            Console.Write("Version (default 1.0.0): ");
            string version = Console.ReadLine() ?? "1.0.0";
            
            Console.Write("Creator: ");
            string creator = Console.ReadLine() ?? "Unknown";

            var projectConfig = new ProjectConfig
            {
                Name = name,
                Type = projectType,
                Version = version,
                Creator = creator,
                MainFile = projectType == "CS" ? $"src/{name}.cs" : "src/main.ns",
                Dependencies = new List<string>(),
                Assets = new Dictionary<string, string>
                {
                    ["images"] = "assets/images",
                    ["sounds"] = "assets/sounds",
                    ["scripts"] = "src/scripts"
                }
            };

            // Create project structure
            CreateProjectStructure(name, projectConfig);
        }

        private static void CreateProjectFiles(string name, string projectDir, string srcDir)
        {
            // Create project file
            string projectFile = Path.Combine(projectDir, $"{name}.nsp");
            File.WriteAllText(projectFile, CreateProjectTemplate(name));

            // Create main source file
            string mainFile = Path.Combine(srcDir, "main.ns");
            File.WriteAllText(mainFile, CreateMainTemplate());

            // Create gitignore
            string gitignore = Path.Combine(projectDir, ".gitignore");
            File.WriteAllText(gitignore, CreateGitignoreTemplate());
        }

        private static void CreateRunBat(string name, string projectDir)
        {
            string runBat = Path.Combine(projectDir, "run.bat");
            string content = $@"@echo off
echo Running {name}...
cd ""%~dp0""
nanc run src/main.ns
pause";
            File.WriteAllText(runBat, content);
        }

        private static string CreateProjectTemplate(string name)
        {
            return $@"{{
    ""name"": ""{name}"",
    ""version"": ""1.0.0"",
    ""type"": ""application"",
    ""main"": ""src/main.ns"",
    ""author"": """",
    ""description"": """",
    ""assets"": {{
        ""images"": ""assets/images"",
        ""sounds"": ""assets/sounds""
    }}
}}";
        }

        private static string CreateMainTemplate()
        {
            return @"// Main entry point for your NanCo application

func main() {
    terminal.clear()
    print ""Hello from NanCo!""
    
    // Example of using terminal commands
    terminal.color(""green"")
    print ""Welcome to your new project!""
    
    // Example of using audio
    audio.beep(500)
    
    // Wait for user input
    var name = input.readLine(""What's your name? "")
    print ""Nice to meet you, "" + name + ""!""
}";
        }

        private static string CreateGitignoreTemplate()
        {
            return @"# NanCo specific
*.nsc
build/

# IDE files
.vscode/
.idea/

# OS specific
.DS_Store
Thumbs.db";
        }

        private static void CreateProjectStructure(string name, ProjectConfig config)
        {
            string projectDir = Path.Combine(FileSystemManager.ProjectsDir, name);
            string srcDir = Path.Combine(projectDir, "src");
            string assetsDir = Path.Combine(projectDir, "assets");

            Directory.CreateDirectory(projectDir);
            Directory.CreateDirectory(srcDir);
            Directory.CreateDirectory(assetsDir);
            Directory.CreateDirectory(Path.Combine(assetsDir, "images"));
            Directory.CreateDirectory(Path.Combine(assetsDir, "sounds"));

            CreateProjectFiles(name, projectDir, srcDir);
            CreateRunBat(name, projectDir);
        }
    }
} 