using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

IEnumerable<long> ConstrainedBPressValues((int ax, int ay, int bx, int by, long px, long py) m, long maxPresses) {
    // Precompute constraints
    var gcdX = gcf(m.bx, m.ax);
    var modX = m.px % m.ax;
    if (modX % gcdX != 0) yield break;

    var gcdY = gcf(m.by, m.ay);
    var modY = m.py % m.ay;
    if (modY % gcdY != 0) yield break;

    // Iterate over possible values of cheapPresses
    for (long cheapPresses = 1; cheapPresses < maxPresses; cheapPresses++)
    {
        // Calculate the numerators
        long numeratorX = m.px - cheapPresses * m.bx;
        long numeratorY = m.py - cheapPresses * m.by;

        // Check if the current cheapPresses satisfies the modular conditions
        if (numeratorX % m.ax == 0 && numeratorY % m.ay == 0)
        {
            yield return cheapPresses;
        }
    }
}

void Run(string input) {

    string pattern = @"Button A: X\+(\d+), Y\+(\d+)\s+Button B: X\+(\d+), Y\+(\d+)\s+Prize: X=(\d+), Y=(\d+)";
    var matches = Regex.Matches(input, pattern);
    
    var machines = new List<(int ax, int ay, int bx, int by, long px, long py)>();

    foreach (Match match in matches) {
        int ax = int.Parse(match.Groups[1].Value);
        int ay = int.Parse(match.Groups[2].Value);
        int bx = int.Parse(match.Groups[3].Value);
        int by = int.Parse(match.Groups[4].Value);
        long px = int.Parse(match.Groups[5].Value);
        long py = int.Parse(match.Groups[6].Value);
        
        machines.Add((ax, ay, bx, by, px, py));
    }

    var result = 0L;
    int machineNum = 0;

    foreach (var m in machines) {

        var minCost = long.MaxValue;

        foreach (var cheapPresses in ConstrainedBPressValues(m, 100)) {

            var numeratorX = m.px - cheapPresses * m.bx;
            var numeratorY = m.py - cheapPresses * m.by;

            if (numeratorX % m.ax != 0 || numeratorY % m.ay != 0) {
                continue;
            }
            var expensivePressesToAlignX = numeratorX / m.ax;
            var expensivePressesToAlignY = numeratorY / m.ay;

            if (expensivePressesToAlignX == expensivePressesToAlignY) {
                Console.WriteLine($"\tFound: {expensivePressesToAlignX} {cheapPresses}");

                var cost = 3 * expensivePressesToAlignX + cheapPresses;
                if (cost < minCost) {
                    minCost = cost;
                    break;
                }
            }
        }

        if (minCost != long.MaxValue) {
            Console.WriteLine($"Machine {machineNum}: cost {minCost}");
            result += minCost;
        }
        machineNum++;
    }
    Console.WriteLine($"Total cost: {result}");
}


// MATH helpers

static int gcf(int a, int b) {
    while (b != 0) {
        var temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static int lcm(int a, int b) => (a / gcf(a, b)) * b;
