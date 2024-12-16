using System.Diagnostics;


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

    var pq = new PriorityQueue<PathNode, int>();
    pq.Enqueue(startPath, 0);

    int cost;
    PathNode path;
    while (pq.TryPeek(out path, out cost)) {        
        pq.Dequeue();
        bestCosts[(path.Pos, path.Dir)] = cost;

        if (path.Pos == target) {
            return cost;
        }

        // Console.WriteLine($"Cost: {cost}");
        // board.Print(c => c, new List<Vec2>() { path.Pos }, '@');

        // Go forward
        var nextPosInDir = path.Pos + OffsetFromDirection(path.Dir);
        if (board[nextPosInDir] == '.' && !path.Contains(nextPosInDir)) {

            int bestCost;
            if (!bestCosts.TryGetValue((nextPosInDir, path.Dir), out bestCost)) bestCost = int.MaxValue;

            var moveCost = cost + 1;
            if (moveCost < bestCost) {
                var nextPath = new PathNode(nextPosInDir, path.Dir, path);
                pq.Enqueue(nextPath, moveCost);
            }
        }

        // Turn right and left
        foreach (var turnDir in new Direction[] { TurnRight(path.Dir), TurnLeft(path.Dir) }) {

            // Only check turns if we could actually proceed in this direction.
            var nextPosInTurnDir = path.Pos + OffsetFromDirection(turnDir);

            if (board[nextPosInTurnDir] == '.' && !path.Contains(nextPosInTurnDir, turnDir)) {

                int bestCost;
                if (!bestCosts.TryGetValue((path.Pos, turnDir), out bestCost)) bestCost = int.MaxValue;

                var moveCost = cost + 1000;
                if (moveCost < bestCost) {
                    var nextPath = new PathNode(path.Pos, turnDir, path);
                    pq.Enqueue(nextPath, moveCost);
                }
            }
        }
    }

    return minCost;
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

    var cost = FindShortestCostPath(board, start, end);

    Console.WriteLine($"Result: {cost}");
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

    public Vec2 Pos { get => this._pos; }
    public Direction Dir { get => this._dir; }

    public bool Contains(Vec2 pos) {
        var current = this;
        while (current != null) {
            if (current._pos == pos) return true;
            current = current._prior;
        }
        return false;
    }

    public bool Contains(Vec2 pos, Direction dir) {
        var current = this;
        while (current != null) {
            if (current._pos == pos && current._dir == dir) return true;
            current = current._prior;
        }
        return false;
    }

    private Vec2 _pos;
    private Direction _dir;
    private PathNode? _prior;
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