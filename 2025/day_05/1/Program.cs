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
    var tests = new List<long>();

    while (true)
    {
        if (input[i] == "") break;

        var strs = input[i].Split('-');
        ranges.Add((Convert.ToInt64(strs[0]), Convert.ToInt64(strs[1])));
        i++;
    }

    i++;

    while (i < input.Length)
    {
        tests.Add(Convert.ToInt64(input[i]));
        i++;
    }

    foreach (var test in tests)
    {
        bool spoiled = true;
        foreach (var range in ranges)
        {
            if (test >= range.from && test <= range.to)
            {
                spoiled = false;
                break;
            }
        }
        if (!spoiled)
        {
            result++;
        }
    }

    Console.WriteLine($"Result: {result}");
}
