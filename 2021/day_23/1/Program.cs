using System.Collections.Immutable;
using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

bool IsGoalState(ImmutableArray<(Vec2 pos, char type)> amphipods, AmphipodBurrow board) {
    foreach (var amphipod in amphipods) {
        if (!board.Rooms[amphipod.type - 'A'].Contains(amphipod.pos)) {
            return false;
        }
    }
    return true;
}

void Run(string[] input) {
    var result = 0L;
    var bestKnownCosts = new Dictionary<ImmutableArray<(Vec2 pos, char type)>, int>(new ImmutableArrayEqualityComparer<(Vec2, char)>());

    (var board, var amphipodsInitialState) = AmphipodBurrow.FromString(input);

    var q = new PriorityQueue<ImmutableArray<(Vec2 pos, char type)>, int>();
    q.Enqueue(amphipodsInitialState, 0);

    while (q.TryDequeue(out var amphipods, out var cost)) {
        if (IsGoalState(amphipods, board)) {
            result = cost;
            break;
        }

        if (bestKnownCosts.TryGetValue(amphipods, out var knownCost) && knownCost <= cost) {
            continue;
        }
        bestKnownCosts[amphipods] = cost;

        var moves = board.GetAllMoves(amphipods);

        foreach (var move in moves) {
            var newAmphipods = amphipods.Select(a => a.pos == move.from ? (pos: move.to, type: a.type) : a).OrderBy(a => a.pos).ToImmutableArray();
            var newCost = cost + move.cost;

            q.Enqueue(newAmphipods, newCost);
        }
    }

    Console.WriteLine($"Result: {result}");
}

class ImmutableArrayEqualityComparer<T> : IEqualityComparer<ImmutableArray<T>> where T : IEquatable<T> {
    public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y) => x.SequenceEqual(y);
    public int GetHashCode(ImmutableArray<T> obj) => obj.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode()));
}

public record Vec2(int X, int Y) : IComparable<Vec2> {
    public int CompareTo(Vec2? other) => (other == null) ? 1 : (X, Y).CompareTo((other.X, other.Y));
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}

public class AmphipodBurrow : FixedBoard<char> {
    public List<List<Vec2>> Rooms { get; private set; }
    public List<Vec2> Hallway { get; private set; }
    private Dictionary<Vec2, List<(Vec2 to, List<Vec2> path)>> _precomputedMoves;
    private HashSet<Vec2> _allRooms;
    private List<int> _roomDeepestY;

    public AmphipodBurrow(int width, int height) : base(width, height) {
        Hallway = new List<Vec2>();
        Rooms = new List<List<Vec2>>();
        _precomputedMoves = new Dictionary<Vec2, List<(Vec2 to, List<Vec2> path)>>();
        _allRooms = new HashSet<Vec2>();
        _roomDeepestY = new List<int>();
    }

    public List<(Vec2 from, Vec2 to, int cost)> GetAllMoves(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();
        var occupied = amphipods.Select(a => a.pos).ToHashSet();
        var posToType = amphipods.ToDictionary(a => a.pos, a => a.type);

        foreach (var amphipod in amphipods) {
            if (IsInCorrectRoom(amphipod) && !IsBlockingOthers(amphipod, posToType)) {
                continue;
            }

            if (_precomputedMoves.TryGetValue(amphipod.pos, out var potentialMoves)) {
                foreach (var move in potentialMoves) {
                    if (occupied.Contains(move.to)) continue;

                    bool pathBlocked = move.path.Any(p => occupied.Contains(p));
                    if (pathBlocked) continue;

                    if (IsRoom(move.to)) {
                        int typeIndex = amphipod.type - 'A';
                        var targetRoom = Rooms[typeIndex];
                        if (!targetRoom.Contains(move.to)) continue;

                        if (RoomContainsIncorrectTypes(targetRoom, amphipod.type, posToType)) continue;

                        int deepestY = _roomDeepestY[typeIndex];
                        if (!IsDeepestAvailableSpot(occupied, move.to, deepestY)) continue;
                    }

                    int cost = move.path.Count * CostPerMove(amphipod.type);
                    moves.Add((amphipod.pos, move.to, cost));
                }
            }
        }

        return moves;
    }

    private bool IsRoom(Vec2 pos) => _allRooms.Contains(pos);

    private bool RoomContainsIncorrectTypes(List<Vec2> room, char expectedType, Dictionary<Vec2, char> posToType) {
        foreach (var pos in room) {
            if (posToType.TryGetValue(pos, out var type) && type != expectedType) {
                return true;
            }
        }
        return false;
    }

    private bool IsDeepestAvailableSpot(HashSet<Vec2> occupied, Vec2 pos, int deepestY) {
        if (pos.Y == deepestY) return true;
        for (int y = pos.Y + 1; y <= deepestY; y++) {
            if (!occupied.Contains(new Vec2(pos.X, y))) return false;
        }
        return true;
    }

    public void Print(ImmutableArray<(Vec2 pos, char type)> amphipods) => Print((pos, c) => amphipods.FirstOrDefault(a => a.pos == pos).type);

    public static int CostPerMove(char c) => c switch {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => 0
    };

    public static (AmphipodBurrow board, ImmutableArray<(Vec2 pos, char type)> amphipods) FromString(string[] input) {
        var board = new AmphipodBurrow(input[0].Length, input.Length);
        var amphipods = new List<(Vec2 pos, char type)>();

        board.PopulateBoard(input, (pos, c) => {
            if (c >= 'A' && c <= 'D') {
                amphipods.Add((pos, c));
                return '.';
            }
            return c;
        });

        board.Hallway = Enumerable.Range(1, board.Width - 2).Select(x => new Vec2(x, 1)).ToList();

        var roomLocations = new List<Vec2>();
        for (int x = 1; x < board.Width - 1; x++) {
            for (int y = 2; y < board.Height - 1; y++) {
                if (board[x, y] == '.') roomLocations.Add(new Vec2(x, y));
            }
        }

        board._allRooms = new HashSet<Vec2>(roomLocations);
        board.Rooms = roomLocations.GroupBy(p => p.X)
            .OrderBy(g => g.Key)
            .Select(g => g.OrderBy(p => p.Y).ToList()).ToList();
        board._roomDeepestY = board.Rooms.Select(room => room.Max(p => p.Y)).ToList();

        foreach (var hallwayPos in board.Hallway) {
            if (board._allRooms.Any(p => p.X == hallwayPos.X)) continue;

            foreach (var roomPos in roomLocations) {
                var pathToRoom = board.GetPath(hallwayPos, roomPos);
                AddMove(board._precomputedMoves, hallwayPos, roomPos, pathToRoom);

                var pathToHallway = board.GetPath(roomPos, hallwayPos);
                AddMove(board._precomputedMoves, roomPos, hallwayPos, pathToHallway);
            }
        }

        return (board, amphipods.OrderBy(a => a.pos).ToImmutableArray());
    }

    private static void AddMove(Dictionary<Vec2, List<(Vec2 to, List<Vec2> path)>> moves, Vec2 from, Vec2 to, List<Vec2> path) {
        if (!moves.TryGetValue(from, out var existing)) {
            existing = new List<(Vec2 to, List<Vec2> path)>();
            moves[from] = existing;
        }
        existing.Add((to, path));
    }

    private List<Vec2> GetPath(Vec2 from, Vec2 to) {
        var path = new List<Vec2>();
        bool moveHorizontallyFirst = from.Y == Hallway[0].Y;

        if (moveHorizontallyFirst) {
            MoveHorizontally(from, new Vec2(to.X, from.Y), path);
            MoveVertically(new Vec2(to.X, from.Y), to, path);
        } else {
            MoveVertically(from, new Vec2(from.X, Hallway[0].Y), path);
            MoveHorizontally(new Vec2(from.X, Hallway[0].Y), to, path);
        }

        return path;
    }

    private static void MoveHorizontally(Vec2 from, Vec2 to, List<Vec2> path) {
        int step = Math.Sign(to.X - from.X);
        for (int x = from.X + step; x != to.X + step; x += step) {
            path.Add(new Vec2(x, from.Y));
        }
    }

    private static void MoveVertically(Vec2 from, Vec2 to, List<Vec2> path) {
        int step = Math.Sign(to.Y - from.Y);
        for (int y = from.Y + step; y != to.Y + step; y += step) {
            path.Add(new Vec2(from.X, y));
        }
    }

    private bool IsInCorrectRoom((Vec2 pos, char type) amphipod) {
        var targetRoom = Rooms[amphipod.type - 'A'];
        return targetRoom.Contains(amphipod.pos);
    }

    private bool IsBlockingOthers((Vec2 pos, char type) amphipod, Dictionary<Vec2, char> posToType) {
        var targetRoom = Rooms[amphipod.type - 'A'];
        if (!targetRoom.Contains(amphipod.pos) || amphipod.pos.Y == _roomDeepestY[amphipod.type - 'A']) return false;

        for (int y = amphipod.pos.Y + 1; y <= _roomDeepestY[amphipod.type - 'A']; y++) {
            var posBelow = new Vec2(amphipod.pos.X, y);
            if (posToType.TryGetValue(posBelow, out var type) && type != amphipod.type) {
                return true;
            }
        }
        return false;
    }
}

public class FixedBoard<T> {
    protected T[,] _data;
    public int Width => _data.GetLength(0);
    public int Height => _data.GetLength(1);
    public Vec2 Extents => new Vec2(Width, Height);

    public FixedBoard(int width, int height) => _data = new T[width, height];

    public T this[int x, int y] { get => _data[x, y]; set => _data[x, y] = value; }
    public T this[Vec2 pos] { get => _data[pos.X, pos.Y]; set => _data[pos.X, pos.Y] = value; }

    public bool IsInBounds(Vec2 pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < Width && pos.Y < Height;

    public void Print(Func<Vec2, T, char> resolveChar) {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                Console.Write(resolveChar(new Vec2(x, y), _data[x, y]));
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    protected void PopulateBoard(string[] input, Func<Vec2, char, T> transform) {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                char c = input[y].Length > x ? input[y][x] : ' ';
                _data[x, y] = transform(new Vec2(x, y), c);
            }
        }
    }
}