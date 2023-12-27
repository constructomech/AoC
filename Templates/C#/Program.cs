using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);

void Run(string[] input) {

    // Implement
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
