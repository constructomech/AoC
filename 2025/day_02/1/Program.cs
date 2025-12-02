
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var rangeStrings = input[0].Split(',');
    
    foreach (var rangeString in rangeStrings) {
        var parts = rangeString.Split('-');

        var start = long.Parse(parts[0]);
        var end = long.Parse(parts[1]);

        for (var num = start; num <= end; num++) {
            var numStr = num.ToString();

            if (HasRepeatingPattern(numStr)) {
                result += num;
            }
        }
    }

    Console.WriteLine($"Result: {result}");
}

bool HasRepeatingPattern(string numStr)
{
    if (numStr.Length % 2 != 0) {
        return false;
    }

    var firstHalf = numStr.Substring(0, numStr.Length / 2);
    var secondHalf = numStr.Substring(firstHalf.Length);

    return (firstHalf == secondHalf);
}
