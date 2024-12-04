using System.Diagnostics;

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
    int result = 0;

    int width = wordSearch[0].Length;
    int height = wordSearch.Length;

    for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {

            var currentPos = new Vec2(x, y);
            if (getAt(wordSearch, currentPos) == 'A') {

                var ul = getAt(wordSearch, currentPos + new Vec2(-1, -1));
                var ur = getAt(wordSearch, currentPos + new Vec2(1, -1));
                var ll = getAt(wordSearch, currentPos + new Vec2(-1, 1));
                var lr = getAt(wordSearch, currentPos + new Vec2(1, 1));

                if (((ul == 'M' && lr == 'S') || (ul == 'S' && lr == 'M')) &&
                    ((ur == 'M' && ll == 'S') || (ur == 'S' && ll == 'M'))) {
                        result++;
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
