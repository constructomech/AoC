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


char? getAt(string[] wordSearch, Vec2 pos) {
    if (pos.X >= 0 && pos.X < wordSearch[0].Length && pos.Y >= 0 && pos.Y < wordSearch.Length) {
        return wordSearch[pos.Y][pos.X];
    }
    return null;
}

void Run(string[] wordSearch) {

    const string targetWord = "XMAS";
    int result = 0;

    int width = wordSearch[0].Length;
    int height = wordSearch.Length;

    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
                
            foreach (var dir in AllAdjacent) {
                var currentPos = new Vec2(x, y);

                for (int i = 0; i < targetWord.Length; i++) {
                    if (targetWord[i] != getAt(wordSearch, currentPos)) {
                        break;
                    }
                    currentPos += dir;
                    if (i == targetWord.Length - 1) {
                        result++;
                    }
                }
            }
        }
    }

    Console.WriteLine($"Found {result} instances");
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
