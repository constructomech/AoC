using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string input) {
    var result = 0L;

    //var boardSize = new Vec2(11, 7); 
    var boardSize = new Vec2(101, 103); 

    var robots = new List<(Vec2 p, Vec2 v)>();

    string pattern = @"p=(?<pX>-?\d+),(?<pY>-?\d+)\s+v=(?<vX>-?\d+),(?<vY>-?\d+)";
    var  regex = new Regex(pattern);

    foreach (Match match in regex.Matches(input))
    {
        var pX = int.Parse(match.Groups["pX"].Value);
        var pY = int.Parse(match.Groups["pY"].Value);
        var vX = int.Parse(match.Groups["vX"].Value);
        var vY = int.Parse(match.Groups["vY"].Value);

        Console.WriteLine($"p=({pX},{pY}) v=({vX},{vY})");
        robots.Add((new Vec2(pX, pY), new Vec2(vX, vY)));

    }
 
    // Move the robots for 100 ticks
    for (var second = 0; second < 100; second++) {
        for (var i = 0; i < robots.Count; i++) {
            var robot = robots[i];
            robot.p = Vec2.AddWithWrap(robot.p, robot.v, boardSize);
            robots[i] = robot;
        }
    }

    Vec2 midline = new Vec2((boardSize.X - 1) / 2, (boardSize.Y - 1) / 2);

    int ul = 0, ur = 0, ll = 0, lr = 0;

    // Count the quadrants
    foreach ((var pos, _) in robots) {
        if (pos.X < midline.X && pos.Y < midline.Y) {
            ul++;
        } else if (pos.X > midline.X && pos.Y < midline.Y) {
            ur++;
        } else if (pos.X < midline.X && pos.Y > midline.Y) {
            ll++;
        } else if (pos.X > midline.X && pos.Y > midline.Y) {
            lr++;
        }
    }

    var board = new FixedBoard<char>(boardSize.X, boardSize.Y);
    board.Print(c => c == '\0' ? '.' : c, robots.Select(robot => robot.p).ToList(), '#');
    
    result = ul * ur * ll * lr;

    Console.WriteLine($"Result: {result}");
}


public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 AddWithWrap(Vec2 a, Vec2 b, Vec2 extents) {
        var x = (a.X + b.X) % extents.X;
        var y = (a.Y + b.Y) % extents.Y;

        while (x < 0) x += extents.X;
        while (y < 0) y += extents.Y;

        return new Vec2(x, y);
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}

public class FixedBoard<T> {
    public FixedBoard(int width, int height) => _data = new T[width, height];

    public int Width { get => _data.GetLength(0); }
    public int Height { get => _data.GetLength(1); }

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
        var board = new FixedBoard<char>(input.Length, input.Length > 0 ? input[0].Length : 0);
        board.PopulateBoard(input, c => c);
        return board;
    }

    public static FixedBoard<T> FromString(string[] input, Func<char, T> transform) {
        var board = new FixedBoard<T>(input.Length, input.Length > 0 ? input[0].Length : 0);
        board.PopulateBoard(input, transform);
        return board;
    }
    
    private T[,] _data;
}
