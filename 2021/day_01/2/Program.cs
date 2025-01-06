using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var nums = input.Select(long.Parse).ToList();
    var previous = long.MaxValue;

    for (var i = 2; i < nums.Count; i++ ) {
        var current = nums[i] + nums[i - 1] + nums[i - 2];

        if (current > previous) {
            result++;
        }

        previous = current;
    }

    Console.WriteLine($"Result: {result}");
}
