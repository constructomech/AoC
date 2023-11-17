using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");


// Implement here


watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}