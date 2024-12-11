using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

long Blink(string input, int iter, Dictionary<(string input, int iter), long> cache) {
    long result = 0;

    long answer;
    if (cache.TryGetValue((input, iter), out answer)) {
        return answer;
    }

    if (iter == 75) {
        result = 1;
    } else {
        if (input == "0") {
            result += Blink("1", iter + 1, cache);

        } else if (input.Length % 2 == 0) {
            var lhs = input.Substring(0, input.Length / 2);
            var rhs = input.Substring(input.Length / 2);
            while (rhs[0] == '0') {
                if (rhs == "0") break;
                rhs = rhs.Substring(1);
            }

            result += Blink(lhs, iter + 1, cache);
            result += Blink(rhs, iter + 1, cache);
            
        } else {
            var num = Convert.ToDecimal(input);
            num *= 2024;
            result += Blink(num.ToString(), iter + 1, cache);
        }
    }

    cache.TryAdd((input, iter), result);

    return result;
}

void Run(string[] input) {
    var result = 0L;

    var stones = input[0].Split(' ').ToList();

    var cache = new Dictionary<(string input, int iter), long>(); // Input string & iter to final count

    for (var stoneIdx = 0; stoneIdx < stones.Count; stoneIdx++) {
        result += Blink(stones[stoneIdx], 0, cache);
    }

    Console.WriteLine($"Result: {result}");
}
