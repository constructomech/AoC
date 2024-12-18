using System.Collections.Immutable;
using System.Diagnostics;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


int FindMinPath(FixedBoard<char> board) {
    var start = new Vec2(0, 0);
    var end = new Vec2(board.Width - 1, board.Height - 1);

    var visited = new bool[board.Width, board.Height];
    var q = new Queue<(Vec2 pos, int steps)>();
    q.Enqueue((start, 0));

    while (q.Count > 0) {
        (var pos, var steps) = q.Dequeue();

        if (pos == end) {
            return steps;
        }

        foreach (var dir in CardinalAdjacent) {
            var target = pos + dir;
            if (board.IsInBounds(target) && board[target] != '#' && visited[target.X, target.Y] != true) {

                q.Enqueue((target, steps + 1));
                visited[target.X, target.Y] = true;        
            }
        }
    }
    return int.MaxValue;
}

void Run(string[] input) {
    var points = input.Select(s => { var p = s.Split(','); return new Vec2(int.Parse(p[0]), int.Parse(p[1])); }).ToList();

    var board = new FixedBoard<char>(71, 71);

    for (var i = 0; i < points.Count; i++) {
        board[points[i]] = '#';

        var result = FindMinPath(board);
        if (result == int.MaxValue) {
            Console.WriteLine($"Result: ({points[i].X}, {points[i].Y})");
            break;
        }
    }    
}


public record Vec2 (int X, int Y) {
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
}


public class FixedBoard<T> {
    public FixedBoard(int width, int height) => _data = new T[width, height];

    public int Width { get => _data.GetLength(0); }
    public int Height { get => _data.GetLength(1); }

    public T this[Vec2 pos] { get => _data[pos.X, pos.Y]; set => _data[pos.X, pos.Y] = value; }

    public bool IsInBounds(Vec2 pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < this.Width && pos.Y < this.Height;

    private T[,] _data;
}
