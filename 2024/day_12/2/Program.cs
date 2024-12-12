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

void AddToEdges(List<Edge> edges, Direction type, Vec2 pos) {
    foreach (var edge in edges) {
        if (edge.ContainsOrContinues(pos, type)) {
            edge.Add(pos);
            return;
        }
    }
    var newEdge = new Edge(pos, type);
    edges.Add(newEdge);
}

int CalcPerimeter(List<Vec2> region) {
    var rawEdges = new List<(Vec2 pos, Direction type)>();

    foreach (var pos in region) {
        foreach (var dir in new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right}) {

            var offset = OffsetFromDirection(dir);

            var adjacentPos = pos + offset;

            //Console.WriteLine($"Processing ({adjacentPos.X},{adjacentPos.Y})");

            if (!region.Contains(adjacentPos)) {
                rawEdges.Add((adjacentPos, dir));
            }
        }
    }

    var edges = new List<Edge>();
    var minX = rawEdges.MinBy(edge => edge.pos.X).pos.X;
    var maxX = rawEdges.MaxBy(edge => edge.pos.X).pos.X;
    var minY = rawEdges.MinBy(edge => edge.pos.Y).pos.Y;
    var maxY = rawEdges.MaxBy(edge => edge.pos.Y).pos.Y;
    
    // Traverse in reading order so we don't create the same edge twice.
    for (var y = minY; y <= maxY; y++) {
        for (var x = minX; x <= maxX; x++) {
            foreach (var rawEdge in rawEdges) {
                var pos = new Vec2(x, y);

                if (rawEdge.pos == pos) {
                    AddToEdges(edges, rawEdge.type, rawEdge.pos);
                }
            }
        }
    }

    return edges.Count;
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

Vec2 OffsetFromDirection(Direction direction) {
    return direction switch {
        Direction.Up => new(0, -1),
        Direction.Down => new(0, 1),
        Direction.Left => new(-1, 0),
        Direction.Right => new(1, 0),
        _ => throw new Exception("Invalid direction")
    };
}

public enum Direction { Up, Down, Left, Right }

public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);

    public static Vec2 Min(Vec2 a, Vec2 b) => (a.X < b.X || (a.X == b.X && a.Y < b.Y)) ? a : b;

    public static Vec2 Max(Vec2 a, Vec2 b) => (a.X > b.X || (a.X == b.X && a.Y > b.Y)) ? a : b;
}

public class Edge {

    public Edge(Vec2 p1, Direction type) {
        Type = type;
        Min = p1;
        Max = p1;
    }

    public bool HasLength { get { return Min != Max; } }

    public Vec2 Min { get; set; }

    public Vec2 Max { get; set; }

    public Direction Type { get; private set; }

    public bool ContainsOrContinues(Vec2 pos, Direction edgeType) {
        if (edgeType == Type) {
            switch(Type) {
                case Direction.Up:
                case Direction.Down:
                    return pos.Y == Min.Y && pos.X >= Min.X - 1 && pos.X <= Max.X + 1;

                case Direction.Left:
                case Direction.Right:
                    return pos.X == Min.X && pos.Y >= Min.Y - 1 && pos.Y <= Max.Y + 1; 

                default: throw new InvalidOperationException();
            }
        }
        return false;
    }

    public void Add(Vec2 pos) {
        if (pos.X == Min.X && (Type == Direction.Left || Type == Direction.Right)) {
            Min = (pos.Y < Min.Y) ? pos : Min;
            Max = (pos.Y > Max.Y) ? pos : Max;
        } else if (pos.Y == Min.Y && (Type == Direction.Up || Type == Direction.Down)) {
            Min = (pos.X < Min.X) ? pos : Min;
            Max = (pos.X > Max.X) ? pos : Max;
        }
    }
}
 