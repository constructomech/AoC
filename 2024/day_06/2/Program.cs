using System.Collections.Immutable;
using System.Diagnostics;


var right = new Vec2(1, 0);
var left = new Vec2(-1, 0);
var up = new Vec2(0, -1);
var down = new Vec2(0, 1);

var CardinalAdjacent = ImmutableList.Create<Vec2>(new(1, 0), new(0, 1), new(-1, 0), new(0, -1));
var DiagonallyAdjacent = ImmutableList.Create<Vec2>(new(-1, -1), new(-1, 1), new(1, 1), new(1, -1));
var AllAdjacent = CardinalAdjacent.AddRange(DiagonallyAdjacent);


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

Vec2 TurnRight(Vec2 direction) {
    if (direction == right) {
        return down;
    } else if (direction == left) {
        return up;
    } else if (direction == up) {
        return right;
    } else if (direction == down) {
        return left;
    }
    throw new InvalidOperationException();
}

Cell GetAt(Cell[,] map, Vec2 pos) {
    if (pos.X >= 0 && pos.X < map.GetLength(0) && pos.Y >= 0 && pos.Y < map.GetLength(1)) {
        return map[pos.X, pos.Y];
    }
    return Cell.Open;
}

void PrintMap(Cell[,] map, HashSet<(Vec2 pos, Vec2 dir)> path) { 

    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            switch(map[x, y]) {
                case Cell.Obstruction:  Console.Write('#'); break;
                case Cell.Open:
                    var moves = path.Count(item => item.pos == new Vec2(x, y));
                    if (moves > 0) {
                        Console.Write('+');
                    } else {
                        Console.Write('.');
                    }
                    break;
                default: throw new InvalidOperationException();
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

(bool loop, HashSet<(Vec2 pos, Vec2 direction)>) Simulate(Cell[,] map, Vec2 currentPos, Vec2 currentDirection) {

    var path = new HashSet<(Vec2 pos, Vec2 direction)>();
    path.Add((currentPos, currentDirection));

    while (currentPos.X >= 0 && currentPos.X < map.GetLength(0) && currentPos.Y >= 0 && currentPos.Y < map.GetLength(1)) {

        var proposedNewPos = currentPos + currentDirection;
        var targetCell = GetAt(map, proposedNewPos);
        if (targetCell != Cell.Obstruction) {
            currentPos = proposedNewPos;
        }
        else {
            currentDirection = TurnRight(currentDirection);
        }
        if (path.Contains((currentPos, currentDirection))) {
            return (true, path);
        }
        path.Add((currentPos, currentDirection));
    }
    //PrintMap(map, path);
    return (false, path);
}

void Run(string[] input) {

    var result = 0;
    var map = new Cell[input[0].Length, input.Length];
    var startPos = new Vec2(0, 0);
    var startDirection = new Vec2(0, -1);

    for (int y = 0; y < input.Length; y++) {
        for (int x = 0; x < input[y].Length; x++) {
            switch (input[y][x]) {
                case '.': map[x, y] = Cell.Open; break;
                case '#': map[x, y] = Cell.Obstruction; break;
                case '^': 
                    map[x, y] = Cell.Open;
                    startPos = new Vec2(x, y);
                    break;
                default: throw new InvalidOperationException();
            }
        }
    }

    (_, var originalPath) = Simulate(map, startPos, startDirection);
    PrintMap(map, originalPath);

    var candidatePositions = new HashSet<Vec2>();
    foreach ((var pos, var direction) in originalPath) {
        var targetPos = pos + direction;

        if (targetPos.X >= 0 && targetPos.X < map.GetLength(0) && targetPos.Y >= 0 && targetPos.Y < map.GetLength(1)) {
            candidatePositions.Add(pos + direction);
        }
    }

    foreach (var candidatePosition in candidatePositions) {

        if (map[candidatePosition.X, candidatePosition.Y] == Cell.Open) {
            map[candidatePosition.X, candidatePosition.Y] = Cell.Obstruction;

            (var loop, _) = Simulate(map, startPos, startDirection);
            if (loop) {
                result++;
            }

            map[candidatePosition.X, candidatePosition.Y] = Cell.Open;
        }
    }

    Console.WriteLine($"Result: {result}");
}

enum Cell { Obstruction, Open };

public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}
