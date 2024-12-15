using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string input) {

    string pattern = @"Button A: X\+(\d+), Y\+(\d+)\s+Button B: X\+(\d+), Y\+(\d+)\s+Prize: X=(\d+), Y=(\d+)";
    var matches = Regex.Matches(input, pattern);
    
    var machines = new List<(int ax, int ay, int bx, int by, long px, long py)>();

    foreach (Match match in matches) {
        int ax = int.Parse(match.Groups[1].Value);
        int ay = int.Parse(match.Groups[2].Value);
        int bx = int.Parse(match.Groups[3].Value);
        int by = int.Parse(match.Groups[4].Value);
        long px = int.Parse(match.Groups[5].Value) + 10000000000000L;
        long py = int.Parse(match.Groups[6].Value) + 10000000000000L;
        
        machines.Add((ax, ay, bx, by, px, py));
    }

    var result = 0L;

    foreach (var m in machines) {

        // Algegra happens: bPressGuess = (m.py * m.ax - m.px * m.ay) / (m.by * m.ax - m.bx * m.ay)
        var bPressGuess = (m.py * m.ax - m.px * m.ay) / (m.by * m.ax - m.bx * m.ay);

        // Due to integer math, try a slopy range
        var minCost = long.MaxValue;
        for (var bPresses = bPressGuess - 200; bPresses <= bPressGuess + 200; bPresses++) {

            var aPressNumerator = m.px - bPresses * m.bx;
            if (aPressNumerator % m.ax == 0) {

                var aPresses = (m.px - bPresses * m.bx) / m.ax;

                // Valid solution
                if ((m.px == bPresses * m.bx + aPresses * m.ax) && (m.py == bPresses * m.by + aPresses * m.ay)) {
                    var cost = 3 * aPresses + bPresses;
                    if (cost < minCost) {
                        minCost = cost;
                    }
                }
            }
        }
        if (minCost != long.MaxValue) {
            result += minCost;
        }
    }

    Console.WriteLine($"Total cost: {result}");
}
