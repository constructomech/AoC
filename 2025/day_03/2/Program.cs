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
        var digits = new char[12];
        Array.Fill(digits, '0');
        var inputIdx = 0;

        for (var outputIdx = 0; outputIdx < 12; outputIdx++)
        {
            for (int i = inputIdx; i <= line.Length - 12 + outputIdx; i++)
            {
                if (line[i] > digits[outputIdx])
                {
                    digits[outputIdx] = line[i];
                    inputIdx = i + 1;
                }
            }
        }

        var digitsStr = new string(digits);
        var joltage = Convert.ToInt64(digitsStr);

        result += joltage;
    }

    Console.WriteLine($"Result: {result}");
}
