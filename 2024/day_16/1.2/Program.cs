using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Security.AccessControl;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

int FindShortestCostPath(FixedBoard<char> board, Vec2 start, Vec2 target) {

    int minCost = int.MaxValue;
    var bestCosts = new Dictionary<(Vec2 pos, Direction dir), int>();
    var startPath = new PathNode(start, Direction.Right, null);

    var q = new Queue<(Vec2 pos, Direction dir, int cost, PathNode path)>();
    q.Enqueue((start, Direction.Right, 0, startPath));

    while (q.Count > 0) {
        (var pos, var dir, var cost, var path) = q.Dequeue();

        if (pos == target) {
            if (cost < minCost) {
                minCost = cost;
            }
            continue;
        }

        Console.WriteLine($"Cost: {cost}");
        board.Print(c => c, new List<Vec2>() { pos }, '@');

        int bestCost;
        if (bestCosts.TryGetValue((pos, dir), out bestCost)) {
            if (bestCost < cost) {
                continue;
            }
        }
        else {
            bestCosts.Add((pos, dir), cost);
        }

        // Go forward
        var nextPosInDir = pos + OffsetFromDirection(dir);
        if (board[nextPosInDir] == '.' && !path.GetReversePath().Contains((nextPosInDir, dir))) {
            var nextPath = new PathNode(nextPosInDir, dir, path);
            q.Enqueue((nextPosInDir, dir, cost + 1, nextPath));
        }

        // Only turn if we didn't just turn
        var lastTwo = path.GetReversePath().Take(2).ToList();
        if (lastTwo.Count < 2 || lastTwo[0].dir == lastTwo[1].dir) {

            // Turn right and left
            foreach (var turnDir in new Direction[] { TurnRight(dir), TurnLeft(dir) }) {
                // Only check turns if we could actually proceed in this direction.
                var nextPosInTurnDir = pos + OffsetFromDirection(turnDir);
                if (board[nextPosInTurnDir] == '.' && !path.GetReversePath().Contains((nextPosInTurnDir, turnDir))) {
                    var nextPath = new PathNode(pos, turnDir, path);
                    q.Enqueue((pos, turnDir, cost + 1000, path));
                }
            }
        }
    }

    return minCost;
}

// Add global best cost to abort recursion
(int cost, PathNode fullPath) FindShortestCostPathR(FixedBoard<char> board, PathNode curPath, Vec2 curPos, Direction curDir, int curCost, Vec2 target, Dictionary<(Vec2 curPos, Direction curDir), (int cost, PathNode path)> cache) {
    if (curPos == target) {
        return (curCost, curPath);
    }

    (int cost, PathNode fullPath) cachedResult;
    if (cache.TryGetValue((curPos, curDir), out cachedResult)) {
        return cachedResult;
    }
    
    Console.WriteLine($"Cost: {curCost}");
    var pathPositions = curPath.GetReversePath().Select(t => t.pos).ToList();
    board.Print(c => c, pathPositions, DirectionToChar(curDir));

    PathNode chosenPath = null;
    var minCost = int.MaxValue;
    var nextPosInDir = curPos + OffsetFromDirection(curDir);

    // Go forward (but never revisit our explored path)
    if (board[nextPosInDir] == '.' && !curPath.GetReversePath().Where(item => item.pos == nextPosInDir).Any()) {
        
        var path = new PathNode(nextPosInDir, curDir, curPath);
        var (cost, fullPath) = FindShortestCostPathR(board, path, nextPosInDir, curDir, curCost + 1, target, cache);
        if (cost < minCost) {
            chosenPath = fullPath;
            minCost = cost;
        }
    }

    // Only turn if we didn't just turn
    var lastTwo = curPath.GetReversePath().Take(2).ToList();
    if (lastTwo.Count < 2 || lastTwo[0].dir == lastTwo[1].dir) {

        // Turn right and left
        foreach (var turnDir in new Direction[] { TurnRight(curDir), TurnLeft(curDir) }) {
            // Only check turns if we could actually proceed in this direction.
            var nextPosInTurnDir = curPos + OffsetFromDirection(turnDir);
            if (board[nextPosInTurnDir] == '.' && !curPath.GetReversePath().Contains((nextPosInTurnDir, turnDir))) {

                var path = new PathNode(curPos, turnDir, curPath);

                var (cost, fullPath) = FindShortestCostPathR(board, curPath, curPos, turnDir, curCost + 1000, target, cache);
                if (cost < minCost) {
                    chosenPath = fullPath;
                    minCost = cost;
                }
            }
        }
    }

    cache.Add((curPos, curDir), (minCost, chosenPath));
    return (minCost, chosenPath);
}

void Run(string[] input) {
    var start = new Vec2(0, 0);
    var startDir = Direction.Right;
    var end = new Vec2(0 , 0);
    var board = FixedBoard<char>.FromString(input, (pos, c) => {
        if (c == 'S') {
            start = pos;
            return '.';
        } else if (c == 'E') {
            end = pos;
            return '.';
        }
        return c;
    });

    //var cost = FindShortestCostPath(board, start, end);

    var cache = new Dictionary<(Vec2 curPos, Direction curDir), (int cost, PathNode path)>();
    (var cost, var path) = FindShortestCostPathR(board, new PathNode(start, Direction.Right, null), start, Direction.Right, 0, end, cache);

    Console.WriteLine($"Result: {cost}");
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

char DirectionToChar(Direction direction) {
    return direction switch {
        Direction.Up => '^',
        Direction.Down => 'v',
        Direction.Left => '<',
        Direction.Right => '>',
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

Direction TurnRight(Direction dir) {
    return dir switch {
        Direction.Up => Direction.Right,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        Direction.Right => Direction.Down,
        _ => throw new Exception("Invalid direction")
    };
}

Direction TurnLeft(Direction dir) {
    return dir switch {
        Direction.Up => Direction.Left,
        Direction.Down => Direction.Right,
        Direction.Left => Direction.Down,
        Direction.Right => Direction.Up,
        _ => throw new Exception("Invalid direction")
    };
}

public enum Direction { Up, Down, Left, Right }

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

public class PathNode {
    public PathNode(Vec2 pos, Direction dir, PathNode? prior) {
        this._pos = pos;
        this._dir = dir;
        this._prior = prior;
    }

    public IEnumerable<(Vec2 pos, Direction dir)> GetReversePath() {
        var current = this;
        while (current != null) {
            yield return (current._pos, current._dir);
            current = current._prior;
        }
    }

    private Vec2 _pos;
    private Direction _dir;
    private PathNode? _prior;
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

    private void PopulateBoard(string[] input, Func<Vec2, char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                this._data[x, y] = transform(new Vec2(x, y), input[y][x]);
            }
        }
    }

    public static FixedBoard<char> FromString(string[] input) {
        var board = new FixedBoard<char>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, (_, c) => c);
        return board;
    }

    public static FixedBoard<T> FromString(string[] input, Func<Vec2, char, T> transform) {
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

                if (!gScore.ContainsKey(neighborIdx) || tentativeGScore < gScore[neighborIdx]) {
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

    private static List<int> ReconstructPath(Dictionary<int, int> parentMap, int current) {
        var path = new List<int> { current };
        
        while (parentMap.ContainsKey(current)) {
            current = parentMap[current];
            path.Add(current);
        }
        
        path.Reverse();
        return path;
    }
}