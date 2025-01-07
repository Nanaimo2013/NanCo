using System;
using System.IO;
using System.Collections.Generic;
using NanCo.Games;

namespace NanCo.IDE
{
    public class ProjectView
    {
        private TreeNode? projectRoot;
        private int selectedIndex;
        private List<string> flattenedTree;
        private readonly string projectPath;

        public ProjectView(string path)
        {
            projectPath = path;
            flattenedTree = new List<string>();
            LoadProjectStructure();
        }

        private void LoadProjectStructure()
        {
            var dirInfo = new DirectoryInfo(projectPath);
            projectRoot = new TreeNode(dirInfo.Name, projectPath, true);
            LoadDirectory(projectRoot, dirInfo);
        }

        private void LoadDirectory(TreeNode node, DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                node.Children.Add(new TreeNode(file.Name, file.FullName, false));
            }

            foreach (var subDir in dir.GetDirectories())
            {
                var childNode = new TreeNode(subDir.Name, subDir.FullName, true);
                node.Children.Add(childNode);
                LoadDirectory(childNode, subDir);
            }
        }

        private void BuildFlatTree(TreeNode? node, int depth)
        {
            if (node == null) return;
            
            flattenedTree.Add($"{new string(' ', depth * 2)}{(node.IsDirectory ? "📁" : "📄")} {node.Name}");
            
            if (node.IsDirectory)
            {
                foreach (var child in node.Children)
                {
                    BuildFlatTree(child, depth + 1);
                }
            }
        }

        public void DisplayProjectTree()
        {
            Console.Clear();
            Console.WriteLine("Project Explorer");
            Console.WriteLine("═".PadRight(30, '═'));
            
            flattenedTree.Clear();
            BuildFlatTree(projectRoot, 0);
            
            for (int i = 0; i < flattenedTree.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }
                Console.WriteLine(flattenedTree[i]);
                Console.ResetColor();
            }
        }

        public void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.N when key.Modifiers == ConsoleModifiers.Control:
                    CreateNewFile();
                    break;
                    
                case ConsoleKey.G when key.Modifiers == ConsoleModifiers.Control:
                    CreateGameFile();
                    break;

                case ConsoleKey.UpArrow:
                    if (selectedIndex > 0) selectedIndex--;
                    break;

                case ConsoleKey.DownArrow:
                    if (selectedIndex < flattenedTree.Count - 1) selectedIndex++;
                    break;

                case ConsoleKey.Enter:
                    OpenSelectedItem();
                    break;
            }
            DisplayProjectTree();
        }

        private void OpenSelectedItem()
        {
            var node = GetNodeAtIndex(projectRoot, selectedIndex);
            if (node != null && !node.IsDirectory)
            {
                var ide = new NanIDE(node.Path);
                ide.Start();
            }
        }

        private TreeNode? GetNodeAtIndex(TreeNode? root, int index)
        {
            if (root == null) return null;
            int currentIndex = 0;
            return TraverseTree(root, ref currentIndex, index);
        }

        private TreeNode? TraverseTree(TreeNode node, ref int currentIndex, int targetIndex)
        {
            if (currentIndex == targetIndex) return node;
            currentIndex++;

            if (node.IsDirectory)
            {
                foreach (var child in node.Children)
                {
                    var result = TraverseTree(child, ref currentIndex, targetIndex);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void CreateGameFile()
        {
            Console.WriteLine("\nSelect Game Template:");
            Console.WriteLine("1. Basic Game");
            Console.WriteLine("2. Arcade Game");
            Console.Write("\nSelect template (1-2): ");
            
            var choice = Console.ReadKey(true);
            Console.WriteLine("\nEnter game name: ");
            string gameName = Console.ReadLine() ?? "NewGame";
            
            string template = choice.KeyChar switch
            {
                '1' => NanSyntax.CodeSnippets["game"],
                '2' => NanSyntax.CodeSnippets["arcade"],
                _ => NanSyntax.CodeSnippets["game"]
            };
            
            string filePath = Path.Combine(projectPath, $"{gameName}.ns");
            File.WriteAllText(filePath, template);
            LoadProjectStructure();
        }

        private void CreateNewFile()
        {
            Console.Clear();
            Console.WriteLine("Create New File:");
            Console.WriteLine("1. Empty File");
            Console.WriteLine("2. Game File");
            Console.Write("\nSelect type (1-2): ");
            
            var key = Console.ReadKey(true);
            Console.WriteLine("\nEnter file name: ");
            string fileName = Console.ReadLine() ?? "NewFile";
            
            if (key.KeyChar == '1')
            {
                string filePath = Path.Combine(projectPath, $"{fileName}.ns");
                File.WriteAllText(filePath, "");
                LoadProjectStructure();
            }
            else if (key.KeyChar == '2')
            {
                CreateGameFile();
            }
        }
    }
} 