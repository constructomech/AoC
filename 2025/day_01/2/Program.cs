using System.Collections.Immutable;
using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;
    var dial = 50L;

    for (int i = 0; i < input.Length; i++)
    {
        var inc = (input[i][0] == 'R') ? 1 : -1;
        int steps = int.Parse(input[i].Substring(1));

        while (steps > 0) { 
            dial += inc;
            if (dial == 100) { dial = 0; }
            else if (dial == -1) { dial = 99; }
            if (dial == 0) result++;
            steps--;
        }
    }

    Console.WriteLine($"Result: {result}");
}
