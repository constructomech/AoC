using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var i = 0;
    var ranges = new List<(long from, long to)>();

    while (true)
    {
        if (input[i] == "") break;

        var strs = input[i].Split('-');
        ranges.Add((Convert.ToInt64(strs[0]), Convert.ToInt64(strs[1])));
        i++;
    }
    ranges.Sort((a, b) => a.from.CompareTo(b.from));

    var outputRanges = new List<(long from, long to)>();
    var current = ranges[0];

    for (int j = 1; j < ranges.Count; j++)
    {
        if (ranges[j].from <= current.to)
        {
            current = (current.from, Math.Max(current.to, ranges[j].to));
        }
        else
        {
            outputRanges.Add(current);
            current = ranges[j];
        }
    }
    outputRanges.Add(current);

    foreach (var range in outputRanges)
    {
        result += (range.to - range.from + 1);
    }

    Console.WriteLine($"Result: {result}");
}
