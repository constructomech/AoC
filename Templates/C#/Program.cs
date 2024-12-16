using System.Collections.Immutable;
using System.Diagnostics;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    Console.WriteLine($"Result: {result}");
}


// MATH helpers

static int gcf(int a, int b) {
    while (b != 0) {
        var temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static int lcm(int a, int b) => (a / gcf(a, b)) * b;

// For curve fitting, see the MathNet.Numerics package


// PERMUTATIONS

static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count) {
    var i = 0;
    foreach(var item in items) {
        if(count == 1) {
            yield return new T[] { item };
        }
        else {
            foreach(var result in GetPermutations(items.Skip(i + 1), count - 1)) {
                yield return new T[] { item }.Concat(result);
            }
        }
        i++;
    }
}


// DIRECTION helpers

Direction ParseDirection(char c) {
    return c switch {
        '^' => Direction.Up,
        'v' => Direction.Down,
        '<' => Direction.Left,
        '>' => Direction.Right,
        _ => throw new Exception("Invalid direction")
    };
}

Vec2 OffsetFromDirection(Direction direction) {
    return direction switch {
        Direction.Up => new(0, -1),
        Direction.Down => new(0, 1),
        Direction.Left => new(-1, 0),
        Direction.Right => new(1, 0),
        _ => throw new Exception("Invalid direction")
    };
}

enum Direction { Up, Down, Left, Right }


// COMPARERS 

// For a priority queue optimizing for highest cost
class InverseComparer : IComparer<int> {
    public int Compare(int lhs, int rhs) => rhs.CompareTo(lhs);
}

// For using List<string> as a key in a dictionary or set
class ListComparer<T> : IEqualityComparer<List<T>>
{
    public bool Equals(List<T>? x, List<T>? y) => x == null || y == null ? false : x.SequenceEqual(y);

    public int GetHashCode(List<T> obj) {
        var hashcode = 0;
        foreach (T t in obj) {
            var lineHash = t != null ? t.GetHashCode() : 0;
            hashcode ^= lineHash + BitConverter.ToInt32(_hashSalt) + (hashcode << 6) + (hashcode >> 2);
        }
        return hashcode;
    }

    private static readonly Byte[] _hashSalt = BitConverter.GetBytes(0x9e3779b9);
}


// VECTORS

public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}

public record Vec3 (int X, int Y, int Z) {
    public static Vec3 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator *(Vec3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
}

public class FixedBoard<T> {
    public FixedBoard(int width, int height) => _data = new T[width, height];

    public int Width { get => _data.GetLength(0); }
    public int Height { get => _data.GetLength(1); }

    public Vec2 Extents { get => new Vec2(_data.GetLength(0), _data.GetLength(1)); }
    public T this[int x, int y] { get => _data[x, y]; set => _data[x, y] = value; }
    public T this[Vec2 pos] { get => _data[pos.X, pos.Y]; set => _data[pos.X, pos.Y] = value; }

    public bool IsInBounds(int x, int y) => x >= 0 && y >= 0 && x < this.Width && y < this.Height;
    public bool IsInBounds(Vec2 pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < this.Width && pos.Y < this.Height;

    public void ForEachCell(Action<int, int, T> action) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                action(x, y, this._data[x, y]);
            }
        }
    }

    public void ForEachCell(Action<Vec2, T> action) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                action(new Vec2(x, y), this._data[x, y]);
            }
        }
    }

    public void Print(Func<T, char> resovleChar) => Print(resovleChar, null, default(T));
    public void Print(Func<T, char> resovleChar, List<Vec2>? overridePositions, T? overrideValue) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                char printVal = resovleChar(_data[x, y]);
                if (overridePositions != null && overridePositions.Contains(new Vec2(x, y))) {
                    if (overrideValue == null) throw new ArgumentNullException(nameof(overrideValue));
                    printVal = resovleChar(overrideValue);
                }
                Console.Write(printVal);
            }
            Console.WriteLine();
        }
    }

    private void PopulateBoard(string[] input, Func<char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                this._data[x, y] = transform(input[y][x]);
            }
        }
    }

    public static FixedBoard<char> FromString(string[] input) {
        var board = new FixedBoard<char>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, c => c);
        return board;
    }

    public static FixedBoard<T> FromString(string[] input, Func<char, T> transform) {
        var board = new FixedBoard<T>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, transform);
        return board;
    }
    
    private T[,] _data;
}

public class Graph<TData> {
    public int VertexCount { get => _verticies.Count; }

    public int AddVertex(TData data) {
        this._verticies.Add((data, new List<(int vertexIndex, int weight)>()));
        return this._verticies.Count - 1;
    }

    public void AddEdge(int lhsVertexIndex, int rhsVertexIndex, int weight) {
        this._verticies[lhsVertexIndex].edges.Add((rhsVertexIndex, weight));
    }

    public (TData data, List<(int vertexIndex, int weight)> edges) GetVertex(int index) {
        return _verticies[index];
    }

    IEnumerable<(TData data, List<(int vertexIndex, int weight)> edges)> SelectVerticies(Func<TData, List<(int vertexIndex, int weight)>, bool> predicate) {
        foreach (var vertex in _verticies) {
            if (predicate(vertex.data, vertex.edges)) {
                yield return vertex;
            }
        }
    }

    private List<(TData data, List<(int vertexIndex, int weight)> edges)> _verticies = 
        new List<(TData data, List<(int vertexIndex, int weight)> edges)>();
}

public class DijkstraAlgorithm<TData> {
    public static int[] FindMinWeightFromSource(Graph<TData> graph, int source) {
        var vertices = graph.VertexCount;
        var distances = new int[vertices];
        var shortestPathTreeSet = new bool[vertices];

        for (int i = 0; i < vertices; i++) {
            distances[i] = int.MaxValue;
            shortestPathTreeSet[i] = false;
        }

        distances[source] = 0;

        for (int count = 0; count < vertices - 1; count++) {

            int u = MinimumDistance(distances, shortestPathTreeSet);
            shortestPathTreeSet[u] = true;

            foreach ((int v, int weight) in graph.GetVertex(u).edges) {

                if (!shortestPathTreeSet[v] && distances[u] != int.MaxValue && distances[u] + weight < distances[v]) {
                    distances[v] = distances[u] + weight;
                }
            }
        }

        return distances;
    }

    private static int MinimumDistance(int[] distances, bool[] shortestPathTreeSet) {
        int min = int.MaxValue, minIndex = -1;

        for (int v = 0; v < distances.Length; v++) {

            if (!shortestPathTreeSet[v] && distances[v] <= min) {
                min = distances[v];
                minIndex = v;
            }
        }

        return minIndex;
    }
}

public static class AStar<TData> {

    // Find and return the shortest path from start to goal.
    public static List<int>? Search(Graph<TData> graph, int startIdx, int goalIdx, Func<int, int, int> heuristic) {

        var openList = new List<int> { startIdx };
        var closedList = new HashSet<int>();

        // Dictionaries to hold g(n), h(n), and parent pointers
        var gScore = new Dictionary<int, double> { [startIdx] = 0 };
        var hScore = new Dictionary<int, double> { [startIdx] = heuristic(startIdx, goalIdx) };
        var parentMap = new Dictionary<int, int>();

        while (openList.Count > 0) {

            // Find node in open list with the lowest F score
            var currentIdx = openList.OrderBy(id => gScore[id] + hScore[id]).First();

            if (currentIdx == goalIdx) {
                return ReconstructPath(parentMap, currentIdx);
            }

            openList.Remove(currentIdx);
            closedList.Add(currentIdx);

            foreach ((var neighborIdx, var weightToNeighbor) in graph.GetVertex(currentIdx).edges) {

                var neighbor = graph.GetVertex(neighborIdx);
                if (closedList.Contains(neighborIdx)) continue;

                // Tentative gScore (current gScore + distance to neighbor)
                double tentativeGScore = gScore[currentIdx] + weightToNeighbor;

                if (!gScore.ContainsKey(neighborIdx) || tentativeGScore < gScore[neighborIdx])
                {
                    // Update gScore and hScore
                    gScore[neighborIdx] = tentativeGScore;
                    hScore[neighborIdx] = heuristic(neighborIdx, goalIdx);

                    // Set the current node as the parent of the neighbor
                    parentMap[neighborIdx] = currentIdx;

                    if (!openList.Contains(neighborIdx))
                    {
                        openList.Add(neighborIdx);
                    }
                }
            }
        }

        return null; // No path found
    }

    private static List<int> ReconstructPath(Dictionary<int, int> parentMap, int current)
    {
        var path = new List<int> { current };
        
        while (parentMap.ContainsKey(current))
        {
            current = parentMap[current];
            path.Add(current);
        }
        
        path.Reverse();
        return path;
    }
}