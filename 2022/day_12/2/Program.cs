using System.IO;
using System.Collections.Generic;

// Your current position (S) and the location that should get the best signal (E). 
// Your current position (S) has elevation a, and the location that should get the best signal (E) has elevation z.

List<string> lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
        }
    }
}

(int x, int y) end = (0, 0);

var heightMap = new char[lines.First().Length, lines.Count];
for (int y = 0; y < heightMap.GetLength(1); y++) {
    for (int x = 0; x < heightMap.GetLength(0); x++) {

        char height = lines[y][x];

        if (height == 'S') {
            height = 'a';
        } else if (height == 'E') {
            end = (x, y);
            height = 'z';
        }

        heightMap[x, y] = height;
    }
}

Console.WriteLine("Running on {0} cell height map, {1}x{2}", heightMap.Length, heightMap.GetLength(0), heightMap.GetLength(1));

int minMoves = int.MaxValue;
var bestMoves = new int[heightMap.GetLength(0), heightMap.GetLength(1)];
for (int y = 0; y < heightMap.GetLength(1); y++) {
    for (int x = 0; x < heightMap.GetLength(0); x++) {
        bestMoves[x, y] = int.MaxValue;
    }
}

var stack = new Stack<((int x, int y), int steps)>();
stack.Push((end, 0));
while (stack.Count > 0) {

    ((int x, int y), int steps) = stack.Pop();
    var currentHeight = heightMap[x, y];

    if (steps < minMoves && bestMoves[x, y] > steps) {
        bestMoves[x, y] = steps;

        if (currentHeight == 'a') {
            if (steps < minMoves) {
                minMoves = steps;
            }
            Console.WriteLine("{0} moves to ({1}, {2})", steps, x, y);
        }

        // Queue Left
        if (x > 0 && currentHeight - heightMap[x - 1, y] <= 1) {
            stack.Push(((x - 1, y), steps + 1));
        }

        // Queue Right
        if (x < heightMap.GetLength(0) - 1 && currentHeight - heightMap[x + 1, y] <= 1) {
            stack.Push(((x + 1, y), steps + 1));
        }

        // Queue Up
        if (y > 0 && currentHeight - heightMap[x, y - 1] <= 1) {
            stack.Push(((x, y - 1), steps + 1));
        }

        // Queue Down
        if (y < heightMap.GetLength(1) - 1 && currentHeight - heightMap[x, y + 1] <= 1) {
            stack.Push(((x, y + 1), steps + 1));
        }
    }
}

Console.WriteLine("Min Moves: {0}", minMoves);
