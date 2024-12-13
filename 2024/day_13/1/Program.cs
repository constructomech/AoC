using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);

void Run(string input) {

    string pattern = @"Button A: X\+(\d+), Y\+(\d+)\s+Button B: X\+(\d+), Y\+(\d+)\s+Prize: X=(\d+), Y=(\d+)";
    var matches = Regex.Matches(input, pattern);
    
    var machines = new List<(int ax, int ay, int bx, int by, int px, int py)>();

    foreach (Match match in matches) {
        int ax = int.Parse(match.Groups[1].Value);
        int ay = int.Parse(match.Groups[2].Value);
        int bx = int.Parse(match.Groups[3].Value);
        int by = int.Parse(match.Groups[4].Value);
        int px = int.Parse(match.Groups[5].Value);
        int py = int.Parse(match.Groups[6].Value);
        
        machines.Add((ax, ay, bx, by, px, py));
    }

    var result = 0L;

    foreach (var m in machines) {

        // Decide if we're minizing a or b presses.
        var aArea = m.px / m.ax * m.py / m.ay;
        var bArea = m.px / m.bx * m.py / m.by * 3; // We get 3x the value from B presses

        var AisExpensive = true;
        var cheapX = m.bx;
        var cheapY = m.by;
        var expensiveX = m.ax;
        var expensiveY = m.ay;

        if (aArea > bArea) {
            AisExpensive = false;
            cheapX = m.ax;
            cheapY = m.ay;
            expensiveX = m.bx;
            expensiveY = m.by;
        }

        var epResult = 0;
        var cpResult = 0;

        var winnable = false;
        var increment = gcf(expensiveX, expensiveY);
        for (var cheapPresses = 0; cheapPresses < 100; cheapPresses += increment) {            
            // Calculate the number of expensive presses for this cheap presss value.

            var expensivePressesToAlignX = (m.px - cheapPresses * cheapX) / expensiveX;
            var expensivePressesToAlignY = (m.py - cheapPresses * cheapY) / expensiveY;

            if (expensivePressesToAlignX == expensivePressesToAlignY) {
                epResult = expensivePressesToAlignX;
                cpResult = cheapPresses;
                //Console.WriteLine($"Found one: {expensivePressesToAlignX} {cheapPresses}");
                winnable = true;
                break;
            }
        }

        if (winnable) {
            var cost = 0;
            if (AisExpensive) {
                cost = 3 * epResult + cpResult;
                Console.WriteLine($"A presses: {epResult}, B presses: {cpResult}, Cost {cost}");

            } else {
                cost = 3 * cpResult + epResult;
                Console.WriteLine($"A presses: {cpResult}, B presses: {epResult}, Cost {cost}");
            }
            result += cost;
        }
    }
    Console.WriteLine($"Total cost: {result}");

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
