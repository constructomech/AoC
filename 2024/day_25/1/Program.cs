using System.Collections.Immutable;
using System.Diagnostics;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var boards = new List<LockKey>();

    var readIndex = 0;
    while (readIndex < input.Length) {
        (var board, readIndex) = LockKey.Parse(input, readIndex);
        boards.Add(board);
    }

    foreach (var key in boards.Where(b => b.Type == LockKeyType.Key)) {

        var keyHeightMap = key.HeightMap;

        foreach (var lok in boards.Where(b => b.Type == LockKeyType.Lock)) {

            var lockHeightMap = lok.HeightMap;

            var fit = true;
            for (var i = 0; i < keyHeightMap.Length; i++) {
                if (keyHeightMap[i] + lockHeightMap[i] > 5) {
                    fit = false;
                }
            }
            if (fit) {
                result++;
            }
        }
    }

    Console.WriteLine($"Result: {result}");
}


// MATH helpers

static int gcf(int a, int b) {
    while (b != 0) {
        var temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static int lcm(int a, int b) => (a / gcf(a, b)) * b;

// For curve fitting, see the MathNet.Numerics package


// PERMUTATIONS

static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count) {
    var i = 0;
    foreach(var item in items) {
        if(count == 1) {
            yield return new T[] { item };
        }
        else {
            foreach(var result in GetPermutations(items.Skip(i + 1), count - 1)) {
                yield return new T[] { item }.Concat(result);
            }
        }
        i++;
    }
}


// DIRECTION helpers

Direction ParseDirection(char c) {
    return c switch {
        '^' => Direction.Up,
        'v' => Direction.Down,
        '<' => Direction.Left,
        '>' => Direction.Right,
        _ => throw new Exception("Invalid direction")
    };
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

enum Direction { Up, Down, Left, Right }


// COMPARERS 

// For a priority queue optimizing for highest cost
class InverseComparer : IComparer<int> {
    public int Compare(int lhs, int rhs) => rhs.CompareTo(lhs);
}

// For using List<string> as a key in a dictionary or set
class ListComparer<T> : IEqualityComparer<List<T>>
{
    public bool Equals(List<T>? x, List<T>? y) => x == null || y == null ? false : x.SequenceEqual(y);

    public int GetHashCode(List<T> obj) {
        var hashcode = 0;
        foreach (T t in obj) {
            var lineHash = t != null ? t.GetHashCode() : 0;
            hashcode ^= lineHash + BitConverter.ToInt32(_hashSalt) + (hashcode << 6) + (hashcode >> 2);
        }
        return hashcode;
    }

    private static readonly Byte[] _hashSalt = BitConverter.GetBytes(0x9e3779b9);
}


// VECTORS

public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}

public record Vec3 (int X, int Y, int Z) {
    public static Vec3 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator *(Vec3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
}

public enum LockKeyType { Lock, Key }

public class LockKey : FixedBoard<char> {
    public LockKey(int width, int height)
        : base(width, height) {}

    public LockKeyType Type { get => this._data[0, 0] == '#' ? LockKeyType.Lock : LockKeyType.Key; }

    public int[] HeightMap {
        get {
            var result = new int[this.Width];
            for (var x = 0; x < this.Width; x++) {
                result[x] = CountCol(x, '#') - 1;
            }
            return result;
        }
    }

    private int CountCol(int row, char c) {
        var sum = 0;
        for (int y = 0; y < this.Height; y++) {
            if (this._data[row, y] == c) sum++;
        }
        return sum;
    }

    public static (LockKey board, int index) Parse(string[] input, int startIndex) {
        var width = 0;
        var height = 0;

        for (var i = startIndex; i < input.Length && input[i] != ""; i++) {
            width = input[i].Length;
            height++;
        }

        var board = new LockKey(width, height);

        for (var y = 0; y < board.Height; y++) {
            for (var x = 0; x < board.Width; x++) {
                board[x, y] = input[y + startIndex][x];
            }
        }

        return (board, startIndex + height + 1);
    }
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
    protected T[,] _data;
}

