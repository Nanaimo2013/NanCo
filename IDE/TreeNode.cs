using System.Collections.Generic;

namespace NanCo.IDE
{
    public class TreeNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public List<TreeNode> Children { get; set; }

        public TreeNode(string name, string path, bool isDirectory)
        {
            Name = name;
            Path = path;
            IsDirectory = isDirectory;
            Children = new List<TreeNode>();
        }
    }
} 