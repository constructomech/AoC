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

    var devices = new Dictionary<string, List<string>>();

    foreach (var line in input)
    {
        var parts = line.Split(':');
        var device = parts[0];
        var connections = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
        devices.Add(device, connections);
    }

    var cache = new Dictionary<(string from, string to), long>();

    result = FindPath(cache, devices, "you", "out");
    
    Console.WriteLine($"Result: {result}");
}

long FindPath(Dictionary<(string from, string to), long> cache, Dictionary<string, List<string>> devices, string from, string to)
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
            result += FindPath(cache, devices, next, to);
        }
    }

    cache.Add((from, to), result);
    return result;
}
