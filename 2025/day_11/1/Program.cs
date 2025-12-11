using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input)
{
    var result = 0L;

    var devices = input.Select(line => line.Split(':')).ToDictionary(
        parts => parts[0],
        parts => parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList()
    );

    var cache = new Dictionary<(string from, string to), long>();

    long FindPath(string from, string to)
    {
        if (cache.TryGetValue((from, to), out long result)) return result;

        foreach (var next in devices[from])
        {
            if (next == to)
            {
                result += 1;
            }
            else
            {
                result += FindPath(next, to);
            }
        }

        cache.Add((from, to), result);
        return result;
    }

    result = FindPath("you", "out");
    Console.WriteLine($"Result: {result}");
}
