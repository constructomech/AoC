
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var ranges = input[0].Split(',')
        .Select(rangeString => rangeString.Split('-'))
        .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])));
    
    foreach (var range in ranges) {

        for (var num = range.Start; num <= range.End; num++) {
            var numStr = num.ToString();

            if (HasRepeatingPattern(numStr)) {
                // Console.WriteLine($"Found repeating pattern in {num}");
                result += num;
            }
        }
    }

    Console.WriteLine($"Result: {result}");
}

bool HasRepeatingPattern(string numStr)
{
    for (var parts = 2; parts < numStr.Length + 1; parts++)
    {
        // If the string divides cleanly into this number of parts
        if (numStr.Length % parts == 0)
        {
            var partLen = numStr.Length / parts;
            var targetPattern = numStr.Substring(0, partLen);

            bool validPattern = true;
            for (int i = 1; i < parts; i++)
            {
                var partPattern = numStr.Substring(i * partLen, partLen);
                if (partPattern != targetPattern)
                {
                    validPattern = false;
                    break;
                }
            }
            if (validPattern)
            {
                return true;
            }
        }
    }

    if (numStr.Length % 2 != 0) {
        return false;
    }

    var firstHalf = numStr.Substring(0, numStr.Length / 2);
    var secondHalf = numStr.Substring(firstHalf.Length);

    return (firstHalf == secondHalf);
}
