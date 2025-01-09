using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var board = AmphipodBurrow.FromString(input);

    board.Print();

    Console.WriteLine($"Result: {result}");
}


public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}

public class AmphipodBurrow : FixedBoard<char> {

    public AmphipodBurrow(int width, int height) : base(width, height) {
        _hallway = new List<Vec2>();
        _rooms = new List<List<Vec2>>();
        _amphipods = new List<(Vec2 pos, char type)>();
    }

    public List<List<Vec2>> Rooms { get => _rooms; }

    public List<Vec2> Hallway { get => _hallway; }

    public List<(Vec2, int)> AvailableRoomMoves(Vec2 pos) {
        var moves = new List<(Vec2, int)>();

        return moves;
    }

    public List<(Vec2 from, Vec2 to, int cost)> AvailableHallwayMoves() {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();
        var hallwayY = this.Hallway[0].Y;

        foreach (var pos in this.Rooms.SelectMany(r => r)) {
            if (this[pos] != '.') {

                // If the amphipod isn't blocked in the room
                if (!this._amphipods.Any(p => p.pos == new Vec2(pos.X, pos.Y - 1) || p.pos == new Vec2(pos.X, hallwayY))) {

                    Func<List<(Vec2, Vec2, int)>, int, Vec2, int, bool> conditionalAdd = (moves, offset, pos, hallwayY) => {
                        // If this is not a position directly above a room
                        if (this[pos.X + offset, hallwayY + 1] != '.') {

                            // If this position is blocked by another amphipod, bail
                            if (this._amphipods.Any(p => p.pos == new Vec2(pos.X + offset, hallwayY))) return false;

                            moves.Add((pos, new Vec2(pos.X + offset, hallwayY), AmphipodBurrow.ManhattanDistance(pos, new Vec2(pos.X + offset, hallwayY))));
                        }
                        return true;

                    };

                    // Moves to the left
                    var offset = -1;
                    while (this[pos.X + offset, hallwayY] == '.' && conditionalAdd(moves, offset, pos, hallwayY)) offset--;
                    
                    // Moves to the right
                    offset = 1;
                    while (this[pos.X + offset, hallwayY] == '.' && conditionalAdd(moves, offset, pos, hallwayY)) offset++;
                }
            }
        }

        return moves;
    }

    public void Print() => Print(c => c);

    public static AmphipodBurrow FromString(string[] input) {

        var board = new AmphipodBurrow(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, c => {
            if (c >= 'A' && c <= 'D') {
                board._amphipods.Add((new Vec2(board._amphipods.Count, 0), c));
                return '.';
            }
            return c;
        });

        for (var x = 1; x < board.Width - 1; x++) {
            board._hallway.Add(new Vec2(x, 1));
        }

        for (var x = 1; x < board.Width - 1; x++) {
            for (var y = 2; y < board.Height - 1; y++) {
                if (board[x, y] == '.') {
                    var room = new List<Vec2>();
                    var offset = 0;
                    while (board[x - offset, y] == '.') {
                        room.Add(new Vec2(x - offset, y));
                        offset++;
                    }
                    board._rooms.Add(room);
                }
            }
        }

        return board;
    }

    private static int ManhattanDistance(Vec2 a, Vec2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private List<List<Vec2>> _rooms;
    private List<Vec2> _hallway;

    private List<(Vec2 pos, char type)> _amphipods;
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

    protected void PopulateBoard(string[] input, Func<char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                var item = input[y].Length > x ? input[y][x] : ' ';
                this._data[x, y] = transform(item);
            }
        }
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