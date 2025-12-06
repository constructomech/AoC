using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var board = FixedBoard<char>.FromString(input);

    var operands = new List<long>();
    for (var x = board.Width - 1; x >= 0; x--)
    {        
        var numStr = "";
        var op = '\0';
        for (var y = 0; y < board.Height; y++)
        {
            var c = board[x, y];
            if (char.IsDigit(c))
            {
                numStr += c;
            }
            if (c == '*' || c == '+')
            {
                op = c;
            }
        }
        
        var num = Convert.ToInt64(numStr);
        operands.Add(num);

        if (op != '\0')
        {
            result += DoOp(op, operands);
            operands.Clear();
            x--;
        }
    }

    Console.WriteLine($"Result: {result}");
}

long DoOp(char op, List<long> operands)
{
    var result = operands[0];
    for (int i = 1; i < operands.Count; i++)
    {
        switch(op)
        {
            case '+':
                result += operands[i];
                break;
            case '*':
                result *= operands[i];
                break;
        }
    }
    return result;
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

    private void PopulateBoard(string[] input, Func<char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                this._data[x, y] = transform(input[y][x]);
            }
        }
    }

    public static FixedBoard<char> FromString(string[] input) {
        var board = new FixedBoard<char>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, c => c);
        return board;
    }

    public static FixedBoard<T> FromString(string[] input, Func<char, T> transform) {
        var board = new FixedBoard<T>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, transform);
        return board;
    }
    
    private T[,] _data;
}