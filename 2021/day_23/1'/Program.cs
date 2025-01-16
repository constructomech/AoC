﻿using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

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
    var bestKnownCosts = new Dictionary<ImmutableArray<(Vec2 pos, char type)>, int>();

    (var board, var amphipodsInitialState) = AmphipodBurrow.FromString(input);

    var q = new PriorityQueue<(ImmutableArray<(Vec2 pos, char type)> amphipods, int knownCost), int>();
    q.Enqueue((amphipodsInitialState, 0), board.EstimateOfCostToGoal(amphipodsInitialState));

    while (q.TryPeek(out var item, out var costEstimate)) {
        q.Dequeue();
        bestKnownCosts[item.amphipods] = item.knownCost;

        if (IsGoalState(item.amphipods, board)) {
            result = item.knownCost;
            break;
        }

//        Console.WriteLine($"Known cost: {item.knownCost}, Estimated total: {costEstimate}, Queued: {q.Count}");
//        board.Print(item.amphipods);

        var roomMoves = board.AvailableRoomMoves(item.amphipods);
        var hallwayMoves = board.AvailableHallwayMoves(item.amphipods);
        var moves = roomMoves.Concat(hallwayMoves);

        foreach (var move in moves) {
            var newAmphipods = item.amphipods.Select(p => p.pos == move.from ? (move.to, p.type) : p).ToImmutableArray();
            var knownCost = item.knownCost + move.cost;
            var estimatedRemainingCost = board.EstimateOfCostToGoal(newAmphipods);
            var totalCost = knownCost + estimatedRemainingCost;

            // Check if the cost < best total cost for this board state before queuing anything
            if (bestKnownCosts.TryGetValue(newAmphipods, out var bestKnownCost) && bestKnownCost <= knownCost) {
                continue;
            }

            // Console.WriteLine($"[Queuing] Known cost: {item.knownCost}, Estimated total: {costEstimate}, Queued: {q.Count}");
            // board.Print(newAmphipods);

            q.Enqueue((newAmphipods, knownCost), totalCost);
        }
    }

    Console.WriteLine($"Result: {result}");
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

    public int EstimateOfCostToGoal(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var cost = 0;
        foreach (var amphipod in amphipods) {
            var targetRoom = this.Rooms[amphipod.type - 'A'];

            var path0 = Enumerable.Range(amphipod.pos.Y, this.Hallway[0].Y - 1).Select(y => new Vec2(targetRoom[0].X, y));
            var path1 = Enumerable.Range(amphipod.pos.X, targetRoom[0].X - 1).Select(x => new Vec2(x, this.Hallway[0].Y));
            var path2 = Enumerable.Range(this.Hallway[0].Y, targetRoom[0].Y).Select(y => new Vec2(targetRoom[0].X, y));
            var path = path0.Concat(path1).Concat(path2);

            cost += CostPerMove(amphipod.type) * path.Count();
        }
        return cost;
    }

    public List<(Vec2 from, Vec2 to, int cost)> AvailableRoomMoves(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();

        var candidates = amphipods.Where(p => this.Hallway.Contains(p.pos));
        
        foreach (var candidate in candidates) {
            var targetRoom = this.Rooms[candidate.type - 'A'];

            var path0 = RangeBetween(candidate.pos.X, targetRoom[0].X).Select(x => new Vec2(x, this.Hallway[0].Y));
            var path1 = RangeBetween(this.Hallway[0].Y + 1, targetRoom.Max(p => p.Y)).Select(y => new Vec2(targetRoom[0].X, y));
            var path = path0.Concat(path1);
            path = path.Skip(1); // Skip the position of the Amphipod we're moving.

            var squatter = amphipods.Where(p => p.pos == path.Last());
            if (squatter.Any()) {
                if (squatter.First().type == candidate.type) {
                    path = path.Take(path.Count() - 1);
                }
            }

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
        }

        return moves;
    }

    public List<(Vec2 from, Vec2 to, int cost)> AvailableHallwayMoves(ImmutableArray<(Vec2 pos, char type)> amphipods) {
        var moves = new List<(Vec2 from, Vec2 to, int cost)>();
        var hallwayY = this.Hallway[0].Y;

        var candidates = amphipods.Where(p => this.Rooms.SelectMany(r => r).Contains(p.pos));
        candidates = AmphipodsOutOfPosition(amphipods);

        foreach (var candidate in candidates) {

            // If the amphipod isn't blocked in the room
            if (!amphipods.Any(p => p.pos == new Vec2(candidate.pos.X, candidate.pos.Y - 1) || p.pos == new Vec2(candidate.pos.X, hallwayY))) {

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
