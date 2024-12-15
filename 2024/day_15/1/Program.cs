using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


Vec2? Move(FixedBoard<char> board, Vec2 pos, Vec2 dir) {
    var targetPos = pos + dir;
    if (board[targetPos] == '.') {
        return targetPos;        
    }

    if (board[targetPos] == '#') {
        return null;
    }

    if (board[targetPos] == 'O') { 
        var testPos = targetPos + dir;

        // Find an opening
        var opening = false;
        while (board.IsInBounds(testPos) && board[testPos] != '#') {
            if (board[testPos] == '.') {
                opening = true;
                break;
            }
            testPos += dir;
        }

        if (!opening) {
            return null;
        }

        // Move all the things
        var backPos = testPos;
        do {
            var swapPos = backPos - dir;
            board.Swap(backPos, swapPos);
            backPos = swapPos;
        } while (backPos != pos);
        
        return targetPos;
    }
    throw new InvalidOperationException();
}

void Run(string[] input) {
    Vec2 start = new Vec2(0, 0);

    var board = FixedBoard<char>.FromString(input, (pos, c) => {
        if (c == '@') {
            start = pos; 
            return '.';
        }
        return c;
    });

    var moves = File.ReadAllText("moves.txt");
    Vec2 pos = start;
    foreach (var move in moves) {
        Vec2 dir = new Vec2(0, 0);
        switch (move) {
            case '^': dir = new Vec2(0, -1); break;
            case 'v': dir = new Vec2(0, 1); break;
            case '<': dir = new Vec2(-1, 0); break;
            case '>': dir = new Vec2(1, 0); break;
        }
        var newPos = Move(board, pos, dir);
        if (newPos != null) {
            pos = newPos;
        }

        // Console.WriteLine($"Moved {move}");
        // board.Print(c => c, new List<Vec2>() { pos }, '@');
        // Console.WriteLine();
    }

    board.Print(c => c, new List<Vec2>() { pos }, '@');

    var result = 0;
    board.ForEachCell((x, y, c) => {
        if (c == 'O') {
            result += 100 * y + x;
        }
    });

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

public class FixedBoard<T> {
    public FixedBoard(int width, int height) => _data = new T[width, height];

    public int Width { get => _data.GetLength(0); }
    public int Height { get => _data.GetLength(1); }

    public Vec2 Extents { get => new Vec2(_data.GetLength(0), _data.GetLength(1)); }
    public T this[int x, int y] { get => _data[x, y]; set => _data[x, y] = value; }
    public T this[Vec2 pos] { get => _data[pos.X, pos.Y]; set => _data[pos.X, pos.Y] = value; }

    public bool IsInBounds(int x, int y) => x >= 0 && y >= 0 && x < this.Width && y < this.Height;
    public bool IsInBounds(Vec2 pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < this.Width && pos.Y < this.Height;

    public void Swap(Vec2 a, Vec2 b) {
        T tmp = _data[a.X, a.Y];
        _data[a.X, a.Y] = _data[b.X, b.Y];
        _data[b.X, b.Y] = tmp;
    }

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
