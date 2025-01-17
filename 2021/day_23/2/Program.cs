using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

// Observation: This board state is a fail since C can't be moved out of the way to allow D to move into place. If we can detect this we can cut off a lot of search space.
//
// #############
// #C....A.C..A#
// ###.#B#.#.###
//   #.#B#D#D#  
//   #########  

bool IsGoalState(ImmutableArray<(Vec2 pos, char type)> amphipods, AmphipodBurrow board) {

    // If all amphipods are in the correct room
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

    while (q.TryPeek(out var amphipods, out var cost)) {
        q.Dequeue();
        bestKnownCosts[amphipods] = cost;

        if (IsGoalState(amphipods, board)) {
            result = cost;
            break;
        }

        // Console.WriteLine($"Known cost: {cost}, Queued: {q.Count}");
        // board.Print(amphipods);

        var roomMoves = board.HallwayToRoomMoves(amphipods);
        var hallwayMoves = board.RoomToHallwayMoves(amphipods);
        var moves = roomMoves.Concat(hallwayMoves);

        foreach (var move in moves) {
            var newAmphipods = amphipods.Select(p => p.pos == move.from ? (move.to, p.type) : p).ToImmutableArray();
            var newCost = cost + move.cost;

            // Check if the cost < best total cost for this board state before queuing anything
            if (bestKnownCosts.TryGetValue(newAmphipods, out var bestKnownCost) && bestKnownCost <= newCost) {
                continue;
            }

            // Console.WriteLine($"[Queuing] cost: {cost}, Queued: {q.Count}");
            // board.Print(newAmphipods);

            q.Enqueue(newAmphipods, newCost);
        }
    }

    Console.WriteLine($"Result: {result}");
}

class ImmutableArrayEqualityComparer<T> : IEqualityComparer<ImmutableArray<T>> where T : IEquatable<T> {
    public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y) => x.SequenceEqual(y);
    public int GetHashCode(ImmutableArray<T> obj) => obj.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode()));
}

public record Vec2 (int X, int Y) {
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}


public class AmphipodBurrow : FixedBoard<char> {

    public AmphipodBurrow(int width, int height) : base(width, height) {
        _hallway = new List<Vec2>();
        _rooms = new List<List<Vec2>>();
    }

    public List<List<Vec2>> Rooms { get => _rooms; }

    public List<Vec2> Hallway { get => _hallway; }

    public List<(Vec2 from, Vec2 to, int cost)> HallwayToRoomMoves(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();

        var candidates = amphipods.Where(p => this.Hallway.Contains(p.pos));
        
        foreach (var candidate in candidates) {
            var targetRoom = this.Rooms[candidate.type - 'A'];

            var path0 = RangeBetween(candidate.pos.X, targetRoom[0].X).Select(x => new Vec2(x, this.Hallway[0].Y));
            var path1 = RangeBetween(this.Hallway[0].Y + 1, targetRoom.Max(p => p.Y)).Select(y => new Vec2(targetRoom[0].X, y));
            var path = path0.Concat(path1);
            path = path.Skip(1); // Skip the position of the Amphipod we're moving.

            // Verifty that room is not already occupied by a wrong type amphipod and shorten the path to the first available room position
            foreach (var roomPos in targetRoom.OrderBy(p => p.Y)) {
                var squatter = amphipods.Where(p => p.pos == roomPos);
                if (squatter.Any()) {
                    if (squatter.First().type != candidate.type) {
                        goto NextCandidate;
                    }
                    path = path.Take(path.Count() - 1);
                }
                else {
                    break;
                }
            }

            // Check for obstructions in the hallway
            bool pathIsClear = true;
            foreach (var pos in path) {
                if (amphipods.Any(p => p.pos == pos)) {
                    pathIsClear = false;
                    break;
                }
            }

            if (pathIsClear) {
                var cost = CostPerMove(candidate.type) * path.Count();
                moves.Add((candidate.pos, path.Last(), cost));
            }

            NextCandidate:;
        }

        return moves;
    }

    public List<(Vec2 from, Vec2 to, int cost)> RoomToHallwayMoves(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();
        var hallwayY = this.Hallway[0].Y;

        // Candidates are amphipods that are in the lowest Y position in a room that this not theirs
        foreach (var room in this.Rooms) {
            var topAmphipodPos = room.OrderBy(p => p.Y).Where(p => amphipods.Any(a => a.pos == p));
            if (topAmphipodPos.Any()) {

                var candidate = amphipods.First(a => a.pos == topAmphipodPos.First());

                Func<List<(Vec2, Vec2, int)>, int, Vec2, int, bool> conditionalAdd = (moves, offset, pos, hallwayY) => {
                    // If this is not a position directly above a room
                    if (this[pos.X + offset, hallwayY + 1] != '.') {

                        // If this position is blocked by another amphipod, bail
                        if (amphipods.Any(p => p.pos == new Vec2(pos.X + offset, hallwayY))) return false;

                        var cost = AmphipodBurrow.ManhattanDistance(pos, new Vec2(pos.X + offset, hallwayY)) * CostPerMove(candidate.type);

                        moves.Add((pos, new Vec2(pos.X + offset, hallwayY), cost));
                    }
                    return true;

                };

                // Moves to the left
                var offset = -1;
                while (this[candidate.pos.X + offset, hallwayY] == '.' && conditionalAdd(moves, offset, candidate.pos, hallwayY)) offset--;
                
                // Moves to the right
                offset = 1;
                while (this[candidate.pos.X + offset, hallwayY] == '.' && conditionalAdd(moves, offset, candidate.pos, hallwayY)) offset++;
            }
        }
        return moves;
    }

    public void Print(ImmutableArray<(Vec2 pos, char type)> amphipods) => Print((pos, c) => {
        if (amphipods.Any(p => p.pos == pos)) {
            return amphipods.First(p => p.pos == pos).type;
        }
        return c;
    });

    public static int CostPerMove(char c) =>
        c switch {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            'D' => 1000,
            _ => 0
        };

    public static (AmphipodBurrow board, ImmutableArray<(Vec2 pos, char type)> amphipods) FromString(string[] input) {

        var board = new AmphipodBurrow(input.Length > 0 ? input[0].Length : 0, input.Length);
        var amphipods = new List<(Vec2 pos, char type)>();

        board.PopulateBoard(input, (pos, c) => {
            if (c >= 'A' && c <= 'D') {
                amphipods.Add((pos, c));
                return '.';
            }
            return c;
        });

        for (var x = 1; x < board.Width - 1; x++) {
            board._hallway.Add(new Vec2(x, 1));
        }

        var roomLocations = new List<Vec2>();
        for (var x = 1; x < board.Width - 1; x++) {
            for (var y = 2; y < board.Height - 1; y++) {
                if (board[x, y] == '.') {
                    roomLocations.Add(new Vec2(x, y));
                }
            }
        }
        board._rooms = roomLocations.GroupBy(p => p.X).Select(g => g.OrderBy(p => p.Y).ToList()).ToList();

        return (board, amphipods.ToImmutableArray());
    }

    private static IEnumerable<int> RangeBetween(int start, int end) {
        int step = start <= end ? 1 : -1;
        for (int i = start; i != end + step; i += step) {
            yield return i;
        }
    }

    private static int ManhattanDistance(Vec2 a, Vec2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private IEnumerable<(Vec2 pos, char type)> AmphipodsOutOfPosition(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        foreach (var amphipod in amphipods) {
            if (!this.Rooms[amphipod.type - 'A'].Contains(amphipod.pos)) {
                yield return amphipod;
            }
            else {
                var targetRoom = this.Rooms[amphipod.type - 'A'];

                // If this amphipod is in the bottom row OR if it's on the top row and the rooms is full of appropiate amphipods
                bool inBottomRow = targetRoom.Max(p => p.Y) == amphipod.pos.Y;
                bool allInRoomAreCorrect = targetRoom.All(p => amphipods.Any(a => a.pos == p && a.type == amphipod.type));
                if (!inBottomRow && !allInRoomAreCorrect) {
                    yield return amphipod;
                }
            }
        }
    }

    private List<List<Vec2>> _rooms;
    private List<Vec2> _hallway;
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

    public void Print(Func<Vec2, T, char> resovleChar) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                char printVal = resovleChar(new Vec2(x, y), _data[x, y]);
                Console.Write(printVal);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    protected void PopulateBoard(string[] input, Func<Vec2, char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                var item = input[y].Length > x ? input[y][x] : ' ';
                this._data[x, y] = transform(new Vec2(x, y), item);
            }
        }
    }

    private T[,] _data;
}
