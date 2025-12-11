using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input)
{
    decimal result = 0;

    var devices = new Dictionary<string, List<string>>();

    foreach (var line in input)
    {
        var parts = line.Split(':');
        var device = parts[0];
        var connections = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToList();
        devices.Add(device, connections);
    }

    var cache = new Dictionary<(string from, string to), Histogram>();

    var histogram = FindPath(cache, devices, "svr", "out");
    histogram.Print();
    result = histogram.GetCountWithTags( new() { "dac", "fft" });
    Console.WriteLine($"Result: {result}");
}

Histogram FindPath(Dictionary<(string from, string to), Histogram> cache, Dictionary<string, List<string>> devices, string from, string to)
{
    if (cache.TryGetValue((from, to), out Histogram result)) return result.Clone();
    result = new Histogram();

    foreach (var next in devices[from])
    {
        if (next == to)
        {
            result += 1;
        }
        else
        {
            var newPaths = FindPath(cache, devices, next, to);
            if (next == "dac" || next == "fft")
            {
                Console.WriteLine($"Encountered {next} after {from} node");
                newPaths.AddTag(next);
            }
            result += newPaths;
        }
    }

    cache.Add((from, to), result.Clone());
    return result;
}

class Histogram
{
    public class HistogramEntry
    {
        public List<string> Tags { get; set; }

        public decimal Count { get; set; }
    }

    public Histogram()
    {
        this.Entires = new List<HistogramEntry>();
    }

    public void Print()
    {
        foreach (var entry in this.Entires)
        {
            Console.WriteLine($"{string.Join(',', entry.Tags)}: {entry.Count}");
        }
    }

    public decimal GetCountWithTags(List<string> tags)
    {
        tags.Sort();

        var entry = this.Entires.Where(e => e.Tags.SequenceEqual(tags));

        return entry.Any() ? entry.First().Count : 0;
    }

    public void AddTag(string tag)
    {
        foreach (var entry in this.Entires)
        {
            if (!entry.Tags.Contains(tag))
            {
                entry.Tags.Add(tag);
            }

            // Sort them so we can use sequence equal
            entry.Tags.Sort();
        }
    }

    public void operator += (int c)
    {
        if (this.Entires.Any(e => e.Tags.Count == 0))
        {
            var entry = this.Entires.First(e => e.Tags.Count == 0);
            entry.Count += 1;
        }
        else
        {
            var entry = new HistogramEntry() { Count = 1, Tags = new List<string>() };
            this.Entires.Add(entry);
        }
    }

    public void operator += (Histogram other)
    {
        foreach (var otherEntry in other.Entires)
        {
            var matchingEntries = this.Entires.Where(e => e.Tags.SequenceEqual(otherEntry.Tags));
            if (matchingEntries.Any())
            {
                matchingEntries.First().Count += otherEntry.Count;
            }
            else
            {
                this.Entires.Add(new HistogramEntry() { Count = otherEntry.Count, Tags = otherEntry.Tags });
            }
        }
    }

    public List<HistogramEntry> Entires { get; private set; }

    public Histogram Clone()
    {
        var clone = new Histogram();
        foreach (var entry in this.Entires)
        {
            clone.Entires.Add(new HistogramEntry() 
            { 
                Count = entry.Count, 
                Tags = new List<string>(entry.Tags) 
            });
        }
        return clone;
    }
}
