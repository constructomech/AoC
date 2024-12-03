using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {

    int result = 0;

    var all = input.Aggregate((a, b) => a + b);

    var parts = all.Split("do()");

    foreach (var doLine in parts) {
        var doParts = doLine.Split("don't()");
        var check = doParts[0];

        string pattern = @"mul\((\d+),(\d+)\)";
        MatchCollection matches = Regex.Matches(check, pattern);

        foreach (Match match in matches) {
            var a = int.Parse(match.Groups[1].Value);
            var b = int.Parse(match.Groups[2].Value);
            result += a * b;
        }
    }

    Console.WriteLine(result);
}
