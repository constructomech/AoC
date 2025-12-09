using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;
    var points = new List<Vec2>();
    foreach (var line in input)
    {
        points.Add(Vec2.FromString(line));
    }

    var largestArea = 0L;
    for (var i = 0; i < points.Count; i++)
    {
        for (var j = i + 1; j < points.Count; j++)
        {
            var pi = points[i];
            var pj = points[j];
            var area = Math.Abs((long)pi.X - pj.X + 1) * Math.Abs((long)pi.Y - pj.Y + 1);
            if (area > largestArea)
            {
                largestArea = area;
                Console.WriteLine($"Largest area found: {area}, from {pi} to {pj}");
            }
        }
    }

    result = largestArea;
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
