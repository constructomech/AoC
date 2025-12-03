using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    foreach (var line in input)
    {
        var firstIdx = 0;
        var secondIdx = line.Length - 1;

        for (int i = 1; i < line.Length - 1; i++)
        {
            if (line[i] > line[firstIdx])
            {
                firstIdx = i;
            }
        }

        for (int j = line.Length - 2; j > firstIdx; j--)
        {
            if (line[j] > line[secondIdx])
            {
                secondIdx = j;
            }
        }

        var joltage = Convert.ToInt64(line[firstIdx].ToString() + line[secondIdx].ToString());
        result += joltage;
    }

    Console.WriteLine($"Result: {result}");
}
