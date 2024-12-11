using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var stones = input[0].Split(' ').ToList();

    // Blink 25 times
    for (var i = 0; i < 25; i++) {

        for (var stoneIdx = 0; stoneIdx < stones.Count; stoneIdx++) {
            if (stones[stoneIdx] == "0") {
                stones[stoneIdx] = "1";

            } else if (stones[stoneIdx].Length % 2 == 0) {
                var lhs = stones[stoneIdx].Substring(0, stones[stoneIdx].Length / 2);
                var rhs = stones[stoneIdx].Substring(stones[stoneIdx].Length / 2);
                while (rhs[0] == '0') {
                    if (rhs == "0") break;
                    rhs = rhs.Substring(1);
                }
                stones[stoneIdx] = lhs;
                stoneIdx++;
                stones.Insert(stoneIdx, rhs);
                
            } else {
                var num = Convert.ToDecimal(stones[stoneIdx]);
                num *= 2024;
                stones[stoneIdx] = num.ToString();             
            }
        }

    }

    result = stones.Count;
    Console.WriteLine($"Result: {result}");
}
