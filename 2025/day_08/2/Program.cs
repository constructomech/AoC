using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var boxes = new List<Vec3>();

    foreach (var line in input)
    {
        boxes.Add(Vec3.FromString(line));
    }

    var allLengths = new List<(double len, int a, int b)>();
    for (var i = 0; i < boxes.Count; i++)
    {
        for (var j = i + 1; j < boxes.Count; j++)
        {
            if (i == j) throw new Exception();
            var dist = (boxes[i] - boxes[j]).Magnitude;
            allLengths.Add((dist, i, j));
        }
    }
    allLengths.Sort((a, b) => a.len.CompareTo(b.len));

    var pools = new List<HashSet<int>>();
    foreach ((var _, var a, var b) in allLengths)
    {
        var newPool = new HashSet<int> { a, b };
        pools.Add(newPool);

        var total = ConsolidatePools(pools);
        if (total == boxes.Count)
        {
            Console.WriteLine($"Last combination is {boxes[a]} and {boxes[b]}");
            result = boxes[a].X * boxes[b].X;
            break;
        }
    }

    Console.WriteLine($"Result: {result}");
}

int ConsolidatePools(List<HashSet<int>> pools)
{
    again:
    for (var i = 0; i < pools.Count; i++)
    {
        for (var j = 0; j < pools.Count; j++)
        {
            if (j != i)
            {
                if (pools[i].Overlaps(pools[j]))
                {
                    pools[i].UnionWith(pools[j]);
                    pools.RemoveAt(j);
                    goto again;
                }                    
            }
        }
    }

    var allNodes = new HashSet<int>();
    foreach (var pool in pools)
    {
        allNodes.UnionWith(pool);
    }
    return allNodes.Count;
}

public record Vec3 (long X, long Y, long Z) {
    public static Vec3 FromString(string s) {
        var parts = s.Split(',');
        return new(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
    }

    public double Magnitude { get => Math.Sqrt(X * X + Y * Y + Z * Z); }

    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator *(Vec3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
}
