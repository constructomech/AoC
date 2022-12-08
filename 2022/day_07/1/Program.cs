using System.IO;
using System.Collections.Generic;

Node root = new Node { IsDirectory = true };
Node current = root;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line!.StartsWith('$')) {
            var parts = line.Split(' ');
            switch (parts[1]) {
                case "cd":
                    var path = parts[2];
                    if (path == "/") {
                        current = root;
                    }
                    else if (path == "..") {
                        current = current.Parent!;
                    }
                    else {
                        current = current!.Children.Find(n => n.Name == path)!;
                    }
                break;
                case "ls":
                    // Do nothing, subsequent parsing will enumerate files and directories
                break;                
            }
        }
        else {
            var parts = line.Split(' ');
            if (parts[0] == "dir") {
                var name = parts[1];
                if (current.Children.Find(n => n.Name == name) == null) {
                    current.Children.Add(new Node { Name = name, IsDirectory = true, Parent = current });
                }
            } else {
                var size = Convert.ToInt64(parts[0]);
                var name = parts[1];
                if (current.Children.Find(n => n.Name == name) == null) {
                    current.Children.Add(new Node { Name = name, IsDirectory = false, Size = size, Parent = current });
                }
            }
            // Directory list
        }
    }
}

Fun.SetDirSize(root);

// Find all directories with at most 100,000 byles
long totalSize = 0;

var stack = new Stack<Node>();
stack.Push(root);
while (stack.Count > 0) {
    var node = stack.Pop();

    foreach (var child in node.Children) {
        stack.Push(child);
    }

    if (node.IsDirectory) {
        if (node.Size <= 100000) {
            totalSize += node.Size;
        }
    }
}

Console.WriteLine("Result: {0}", totalSize);


public class Node {

    public string Name = "";

    public bool IsDirectory { get; set; }

    public long Size { get; set; }

    public Node? Parent { get; set; }

    public List<Node> Children = new List<Node>();
}

static class Fun {
    public static long SetDirSize(Node node) {
        if (node.IsDirectory) {
            long totalSize = 0;
            foreach (Node child in node.Children) {
                totalSize += SetDirSize(child);
            }
            node.Size = totalSize;
            return totalSize;
        }
        else {  // Is File
            return node.Size;
        }
    }
}