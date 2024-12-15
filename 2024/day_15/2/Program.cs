using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


List<(Vec2 from, Vec2 to)> CollectMoves(FixedBoard<char> board, Vec2 pos, Vec2 dir) {

    var moves = new List<(Vec2 from, Vec2 to)>();

    var q = new Queue<(Vec2 from, Vec2 to)>();
    q.Enqueue((pos, pos + dir));

    while (q.Count > 0) {
        var item = q.Dequeue();
        moves.Add(item);

        var movesToCheck = new List<(Vec2 from, Vec2 to)>();
        switch (board[item.to]) {
            case '[': 
                movesToCheck.Add((item.to, item.to + dir));
                if (dir.Y != 0) movesToCheck.Add((item.to + new Vec2(1, 0), item.to + new Vec2(1, 0) + dir));
                break;
            case ']':
                movesToCheck.Add((item.to, item.to + dir));
                if (dir.Y != 0) movesToCheck.Add((item.to + new Vec2(-1, 0), item.to + new Vec2(-1, 0) + dir));
                break;
            case '#':
                // There's a barrier, call the whole thing off!
                moves.Clear();
                goto Done;
            case '.':
                // Do nothing
                break;
        }

        foreach ((Vec2 from, Vec2 to) move in movesToCheck) {
            if (!q.Contains(move)) {
                q.Enqueue(move);
            }
        }
    }

    Done:
    return moves;
}

Vec2? Move(FixedBoard<char> board, Vec2 pos, Vec2 dir) {
    var targetPos = pos + dir;
    if (board[targetPos] == '.') {
        return targetPos;        
    }

    if (board[targetPos] == '#') {
        return null;
    }

    if (board[targetPos] == '[' || board[targetPos] == ']') { 
        var moves = CollectMoves(board, pos, dir);
        if (moves.Count > 0) {
            for (var i = moves.Count - 1; i >= 0; i--) {
                (var from, var to) = moves[i];
                board.Swap(from, to);                
            }
            return targetPos;
        }
        return null;
    }
    throw new InvalidOperationException();
}

void Run(string[] input) {
    Vec2 start = new Vec2(0, 0);

    var alteredInput = new string[input.Length];
    for (var y = 0; y < input.Length; y++) {
        var newLine = "";
        for (var x = 0; x < input[y].Length; x++) {
            switch (input[y][x]) {
                case '#': newLine = newLine + "##"; break;
                case 'O': newLine = newLine + "[]"; break;
                case '.': newLine = newLine + ".."; break;
                case '@': newLine = newLine + "@."; break;
            }
        }
        alteredInput[y] = newLine;
    }

    var board = FixedBoard<char>.FromString(alteredInput, (pos, c) => {
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
        if (c == '[') {
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
