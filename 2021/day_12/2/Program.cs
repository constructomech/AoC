
var nodes = new Dictionary<string, Node>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {
            string[] parts = line.Split('-');
            Node node1 = getOrCreate(parts[0]);
            Node node2 = getOrCreate(parts[1]);

            node1.connections.Add(node2);
            node2.connections.Add(node1);
        }
    }
}

List<Path> paths = new List<Path>();

Path startPath = new Path() { name = "start" };

var visitQueue = new Stack<Path>();
visitQueue.Push(startPath);

while (visitQueue.Count > 0) {
    var path = visitQueue.Pop();

    var current = nodes[path.name];

    foreach (var nextCave in current.connections) {
        
        if (isAllowedInPath(path, nextCave.name)) {
            Path newPath = new Path() { name = nextCave.name, parent = path };

            if (newPath.name == "end") {
                paths.Add(newPath);
            }
            else {
                visitQueue.Push(newPath);
            }
        }
    }
}

paths.Reverse();

writePaths();

Console.WriteLine("Paths: {0}", paths.Count);


bool isSmallCave(string cave) {
    foreach (char c in cave) {
        if (c < 'a' || c > 'z') return false;
    }
    return true;
}

bool isAllowedInPath(Path path, string name) {
    if (name == "start") {
        return false;
    }
    
    if (name == "end" || !isSmallCave(name)) {
        return true;
    }

    var existingSmallCaves = new HashSet<string>();
    Path current = path;
    bool alreadyHasDuplicate = false;
    bool alreadyHasThisName = false;
    while (current != null) {
        if (current.name == name) {
            alreadyHasThisName = true;
        }
        if (isSmallCave(current.name)) {
            if (existingSmallCaves.Contains(current.name)) {
                alreadyHasDuplicate = true;

                if (current.name == name) {
                    return false;
                }
            }
            existingSmallCaves.Add(current.name);
        }
        current = current.parent;
    }

    if (alreadyHasDuplicate && alreadyHasThisName) {
        return false;
    }

    return true;
}

Node getOrCreate(string name) {
    Node node;
    if (!nodes.TryGetValue(name, out node)) {
        node = new Node() { name = name };
        nodes.Add(name, node);
    }
    return node;
}

void writePaths() {
    foreach (Path path in paths) {
        var pathStack = new Stack<string>();
        var current = path;
        while (current != null) {
            pathStack.Push(current.name);
            current = current.parent;
        }

        foreach (string item in pathStack) {
            if (item == path.name) {
                Console.WriteLine("{0}", item);
            } else {
                Console.Write("{0},", item);
            }
        }
    }
}

class Node {
    public string name;

    public List<Node> connections = new List<Node>();
}

class Path {

    public string name;

    public Path parent = null;
}