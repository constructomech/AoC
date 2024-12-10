using System.Collections.Immutable;
using System.Diagnostics;

var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


int CountSummits(int[,] map, Vec2 pos) {
    var cache = new Dictionary<Vec2, int>(); // Position to set of unique summits

    var result = CountSummitsRecurse(map, pos, cache);
    return result;
}

int CountSummitsRecurse(int[,] map, Vec2 pos, Dictionary<Vec2, int> cache) {
    int result;
    if (!cache.TryGetValue(pos, out result)) {
        result = 0;

        if (map[pos.X, pos.Y] == 9) {
            result++;
        }
        else {
            foreach (var dir in CardinalAdjacent) {
                var targetPos = pos + dir;
                if (targetPos.X >= 0 && targetPos.X < map.GetLength(0) && targetPos.Y >= 0 && targetPos.Y < map.GetLength(1) &&
                    map[targetPos.X, targetPos.Y] == map[pos.X, pos.Y] + 1) {

                    result += CountSummitsRecurse(map, targetPos, cache);
                }
            }
        }
        cache.Add(pos, result);
    }
    return result;
}

void Run(string[] input) {
    long result = 0;

    var width = input[0].Length;
    var height = input.Length;
    var map = new int[width, height];

    for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
            map[x, y] = input[y][x] - '0';
        }
    }

    for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
            if (map[x, y] == 0) {
                result += CountSummits(map, new Vec2(x, y));
            }
        }
    }

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
