using System.Collections.Immutable;
using System.Diagnostics;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


List<Vec2> FindMinPath(FixedBoard<char> board, Vec2 start, Vec2 end) {
    var visited = new Dictionary<Vec2, Vec2>();
    var q = new Queue<(Vec2 pos, int steps)>();
    q.Enqueue((start, 0));

    while (q.Count > 0) {
        (var pos, var steps) = q.Dequeue();

        foreach (var dir in CardinalAdjacent) {
            var target = pos + dir;

            if (board.IsInBounds(target) && (board[target] != '#') && !visited.ContainsKey(target)) {

                q.Enqueue((target, steps + 1));
                visited[target] = pos;
            }
        }
    }

    var result = new List<Vec2>();
    var current = end;
    do {
        result.Insert(0, current);
        current = visited[current];

    } while (current != start);

    return result;
}

int ManhattanDist(Vec2 a, Vec2 b) {
    return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
}

Dictionary<int, int> CountCheatPlacements(FixedBoard<char> board, Vec2 start, Vec2 end, int minWin) {
    var result = new Dictionary<int, int>();

    var baselinePath = FindMinPath(board, start, end);
    baselinePath.Insert(0, start);

    for (int trailingIdx = 0; trailingIdx < baselinePath.Count; trailingIdx++) {
        for (int leadingIdx = trailingIdx + 1; leadingIdx < baselinePath.Count; leadingIdx++) {
            var trailingPos = baselinePath[trailingIdx];
            var leadingPos = baselinePath[leadingIdx];

            var bridgeDist = ManhattanDist(trailingPos, leadingPos);
            if (bridgeDist <= 20) {

                var pathDist = leadingIdx - trailingIdx;

                var winBy = pathDist - bridgeDist;
                if (winBy > minWin) {
                    result.TryAdd(winBy, 0);
                    result[winBy]++;
                }
            }
        }
    }

    return result;
}

void Run(string[] input) {
    var  start = new Vec2(0, 0);
    var  end = new Vec2(0, 0);

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

    var sum = 0;
    var result = CountCheatPlacements(board, start, end, 99);
    foreach (var item in result.ToImmutableSortedDictionary()) {
        Console.WriteLine($"There are {item.Value} cheats that save {item.Key} picoseconds.");
        sum += item.Value;
    }

    Console.WriteLine($"Result: {sum}");
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
