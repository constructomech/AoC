using System.Collections.Immutable;
using System.Diagnostics;


var right = new Vec2(1, 0);
var left = new Vec2(-1, 0);
var up = new Vec2(0, -1);
var down = new Vec2(0, 1);

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

char GetAt(char[,] map, Vec2 pos) {
    if (pos.X >= 0 && pos.X < map.GetLength(0) && pos.Y >= 0 && pos.Y < map.GetLength(1)) {
        return map[pos.X, pos.Y];
    }
    return '.';
}

void SetVisitedAt(char[,] map, Vec2 pos) {
    if (pos.X >= 0 && pos.X < map.GetLength(0) && pos.Y >= 0 && pos.Y < map.GetLength(1)) {
        map[pos.X, pos.Y] = 'X';
    }
}

void PrintMap(char[,] map, Vec2 currentPos, Vec2 currentDirection) {
    for (int y = 0; y < input.Length; y++) {
        for (int x = 0; x < input[y].Length; x++) {
            if (currentPos.X == x && currentPos.Y == y) {
                Console.Write('^');
            } else {
                Console.Write(map[x,y]);
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

void Simulate(char[,] map, Vec2 currentPos, Vec2 currentDirection) {

    while (currentPos.X >= 0 && currentPos.X < map.GetLength(0) && currentPos.Y >= 0 && currentPos.Y < map.GetLength(1)) {

        // PrintMap(map, currentPos, currentDirection);

        var proposedNewPos = currentPos + currentDirection;
        var targetChar = GetAt(map, proposedNewPos);
        if (targetChar == '.' || targetChar == 'X') {
            SetVisitedAt(map, proposedNewPos);
            currentPos = proposedNewPos;
        }
        else {
            currentDirection = TurnRight(currentDirection);
        }
    }
}

void Run(string[] input) {

    var result = 0;
    var map = new char[input[0].Length, input.Length];
    var currentPosition = new Vec2(0, 0);
    var currentDirection = new Vec2(0, -1);

    for (int y = 0; y < input.Length; y++) {
        for (int x = 0; x < input[y].Length; x++) {
            map[x, y] = input[y][x];

            if (map[x, y] == '^') {
                map[x, y] = 'X';
                currentPosition = new Vec2(x, y);
            }
        }
    }

    Simulate(map, currentPosition, currentDirection);

    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            if (map[x, y] == 'X') {
                result++;
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
