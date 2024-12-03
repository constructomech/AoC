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

    foreach (var line in input) {

        string pattern = @"mul\((\d+),(\d+)\)";
        MatchCollection matches = Regex.Matches(line, pattern);

        foreach (Match match in matches) {
            var a = int.Parse(match.Groups[1].Value);
            var b = int.Parse(match.Groups[2].Value);

            result += a * b;
        }
    }

    Console.WriteLine(result);
}
