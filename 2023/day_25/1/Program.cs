using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


public record Connection(string From, string To);

public interface IGraph {
    IEnumerable<string> Nodes { get; }

    IEnumerable<Connection> Connections { get; }

    IEnumerable<string> LookupConnections(string node);
}

public class AlteredGraph : IGraph {
    public AlteredGraph(IGraph graph, IEnumerable<Connection> except) {
        _graph = graph;
        _except = except;
    }

    public IEnumerable<string> Nodes { 
        get { return _graph.Nodes; }
    }

    public IEnumerable<Connection> Connections {
        get { return _graph.Connections.Except(_except); }
    }

    public IEnumerable<string> LookupConnections(string node) {
        var result = _graph.LookupConnections(node);

        foreach (var item in result ) {

            if (!_except.Any(x => x.From == node && x.To == item) && 
                !_except.Any(x => x.From == item && x.To == node)) {

                yield return item;
            }
        }
    }

    private IGraph _graph;
    private IEnumerable<Connection> _except;
}

public class Graph : IGraph {

    public Graph() {
        _nodes = new HashSet<string>();
    }

    public IEnumerable<string> Nodes { 
        get { return _nodes; }
    }

    public IEnumerable<Connection> Connections {
        get { return _allConnections; }
    }

    public IEnumerable<string> LookupConnections(string node) {
        return _connections[node];
    }

    public void AddNode(string name) {
        _nodes.Add(name);
    }

    public void AddConnection(string lhs, string rhs) {
        _allConnections.Add(new Connection(lhs, rhs));

        if (!_connections.ContainsKey(lhs)) {
            _connections[lhs] = new List<string>();
        }
        _connections[lhs].Add(rhs);

        if (!_connections.ContainsKey(rhs)) {
            _connections[rhs] = new List<string>();
        }
        _connections[rhs].Add(lhs);
    }

    private HashSet<string> _nodes = new HashSet<string>();
    private HashSet<Connection> _allConnections = new HashSet<Connection>();
    private Dictionary<string, List<string>> _connections = new Dictionary<string, List<string>>();
}



static class Fun {

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
    {
        int i = 0;
        foreach(var item in items)
        {
            if(count == 1)
                yield return new T[] { item };
            else
            {
                foreach(var result in GetPermutations(items.Skip(i + 1), count - 1))
                    yield return new T[] { item }.Concat(result);
            }

            ++i;
        }
    }

    public static List<HashSet<string>> FindPartitions(IGraph graph) {
        var partitions = new List<HashSet<string>>();

        var remainingNodes = new HashSet<string>(graph.Nodes);
        while (remainingNodes.Count > 0) {        
            // Take the first node and create a new partition
            var partition = new HashSet<string>();
            var node = remainingNodes.First();
            partition.Add(node);
            remainingNodes.Remove(node);

            // Add all nodes that are connected to this node, recursively
            var nodesToProcess = new Stack<string>();
            nodesToProcess.Push(node);
            while (nodesToProcess.Count > 0) {
                var currentNode = nodesToProcess.Pop();

                foreach (var connectedNode in graph.LookupConnections(currentNode)) {
                    if (partition.Contains(connectedNode)) {
                        continue;
                    }

                    partition.Add(connectedNode);
                    remainingNodes.Remove(connectedNode);
                    nodesToProcess.Push(connectedNode);
                }
            }
            partitions.Add(partition);
        }

        return partitions;
    }

    public static (int nodes0, int nodes1) Find3Cuts(Graph graph) {

        var permutations = GetPermutations(graph.Connections, 3);
        foreach (var permutation in permutations) {
            var alteredGraph = new AlteredGraph(graph, permutation);

            var partitions = FindPartitions(alteredGraph);
            if (partitions.Count == 2) {

                Console.WriteLine($"Found partitions: {partitions[0].Count}, {partitions[1].Count}");
                Console.WriteLine($"Cuts: {permutation.First()}, {permutation.Skip(1).First()}, {permutation.Skip(2).First()}");
                return (partitions[0].Count, partitions[1].Count);
            }
        }
        throw new Exception("No solution found");
    }

    public static void Run(string[] data) {
        var result = 0;

        var graph = new Graph();

        foreach (var line in data) {
            var nodeNames = line.Split(':', ' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();

            foreach (var nodeName in nodeNames) {
                if (!graph.Nodes.Contains(nodeName)) {
                    graph.AddNode(nodeName);
                }
            }

            // Add forward and backward connections implied by this line
            foreach (var nodeName in nodeNames.Skip(1)) {
                graph.AddConnection(nodeNames[0], nodeName);
            }
        }

        var (lhs, rhs) = Find3Cuts(graph);

        result = lhs * rhs;

        Console.WriteLine($"Result: {result}");
    }
}