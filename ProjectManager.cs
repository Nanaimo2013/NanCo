using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NanCo
{
    public class ProjectManager
    {
        public static void CreateProject(string name)
        {
            // Use the FileSystemManager's project directory
            string projectDir = Path.Combine(FileSystemManager.ProjectsDir, name);
            string srcDir = Path.Combine(projectDir, "src");
            string assetsDir = Path.Combine(projectDir, "assets");
            
            try
            {
                // Create directory structure
                Directory.CreateDirectory(projectDir);
                Directory.CreateDirectory(srcDir);
                Directory.CreateDirectory(assetsDir);
                Directory.CreateDirectory(Path.Combine(assetsDir, "images"));
                Directory.CreateDirectory(Path.Combine(assetsDir, "sounds"));

                // Create project files
                CreateProjectFiles(name, projectDir, srcDir);
                
                // Create run.bat
                CreateRunBat(name, projectDir);

                Console.WriteLine($"Created new project: {name}");
                Console.WriteLine($"Location: {projectDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating project: {ex.Message}");
            }
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
    }
} 