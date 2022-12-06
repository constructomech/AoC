
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

var visitQueue = new Queue<Path>();
visitQueue.Enqueue(startPath);

while (visitQueue.Count > 0) {
    var path = visitQueue.Dequeue();

    var current = nodes[path.name];

    foreach (var nextCave in current.connections) {
        
        if (!(isSmallCave(nextCave.name) && isAlreadyInPath(path, nextCave.name))) {
            Path newPath = new Path() { name = nextCave.name, parent = path };

            if (newPath.name == "end") {
                paths.Add(newPath);
            }
            else {
                visitQueue.Enqueue(newPath);
            }
        }
    }
}

Console.WriteLine("Paths: {0}", paths.Count);


bool isSmallCave(string cave) {
    foreach (char c in cave) {
        if (c < 'a' || c > 'z') return false;
    }
    return true;
}

bool isAlreadyInPath(Path path, string name) {
    Path current = path;
    while (current != null) {
        if (current.name == name) {
            return true;
        }
        current = current.parent;
    }
    return false;
}

Node getOrCreate(string name) {
    Node node;
    if (!nodes.TryGetValue(name, out node)) {
        node = new Node() { name = name };
        nodes.Add(name, node);
    }
    return node;
}

class Node {
    public string name;

    public List<Node> connections = new List<Node>();
}

class Path {

    public string name;

    public Path parent = null;
}