using System.Collections.Immutable;
using System.Diagnostics;


var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

Vec2? FindNextUnvisitied(bool[,] visited) {
    for (var x = 0; x < visited.GetLength(0); x++) {
        for (var y = 0; y < visited.GetLength(1); y++) {
            if (!visited[x, y]) {
                return new Vec2(x, y);
            }
        }
    }
    return null;
}

List<Vec2> FloodFind(Vec2 startPos, string[] map, bool[,] visited) {
    var result = new List<Vec2>();
    var q = new Queue<Vec2>();
    visited[startPos.X, startPos.Y] = true;

    q.Enqueue(startPos);
    while (q.Count > 0) {
        var pos = q.Dequeue();
        result.Add(pos);

        foreach (var dir in CardinalAdjacent) {
            var adjacentPos = pos + dir;
            if (adjacentPos.X >= 0 && adjacentPos.X < visited.GetLength(0) && 
                adjacentPos.Y >= 0 && adjacentPos.Y < visited.GetLength(1) &&
                !visited[adjacentPos.X, adjacentPos.Y]) {

                if (map[pos.Y][pos.X] == map[adjacentPos.Y][adjacentPos.X]) {
                    visited[adjacentPos.X, adjacentPos.Y] = true;
                    q.Enqueue(adjacentPos);
                }
            }
        }
    }
    Console.WriteLine($"Area {map[startPos.Y][startPos.X]} = {result.Count}");
    return result;
}

int CalcPerimeter(List<Vec2> region) {
    var result = 0;
    var outerPositions = new HashSet<Vec2>();

    foreach (var pos in region) {
        foreach (var dir in CardinalAdjacent) {

            var adjacentPos = pos + dir;
            if (!region.Contains(adjacentPos)) {
                result++;
            }
        }
    }

    return result;
}


void Run(string[] input) {

    var width = input[0].Length;
    var height = input.Length;

    var visisted = new bool[width,height];
    var regions = new List<List<Vec2>>();
    long result = 0;

    var pos = FindNextUnvisitied(visisted);
    while (pos != null) {

        var newRegion = FloodFind(pos, input, visisted);
        regions.Add(newRegion);

        pos = FindNextUnvisitied(visisted);
    }

    int num = 0;
    foreach (var region in regions) {
        var area = region.Count;
        var perimeter = CalcPerimeter(region);

        result += area * perimeter;

        Console.WriteLine($"Area of {num}: {area} x {perimeter} = {area * perimeter}");
        num++;
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
