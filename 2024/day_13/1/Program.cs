using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

long SolvePart1((long ax, long ay, long bx, long by, long px, long py) m) {
    var minCost = long.MaxValue;
    // I should be able to consrain the max a button presses, but I'm not having luck
    //   with this optimization.
    long maxAPresses = Math.Max(m.px / m.ax, m.py / m.ay) + 1;
    for (var buttonAPresses = 0L; buttonAPresses < 100; buttonAPresses++) {

        var numeratorX = m.px - buttonAPresses * m.bx;
        var numeratorY = m.py - buttonAPresses * m.by;

        if (numeratorX % m.ax != 0 || numeratorY % m.ay != 0) {
            continue;
        }
        var buttonBPressesToAlignX = numeratorX / m.ax;
        var buttonBPressesToAlignY = numeratorY / m.ay;

        if (buttonBPressesToAlignX == buttonBPressesToAlignY) {
            Console.WriteLine($"\tFound: {buttonBPressesToAlignX} {buttonAPresses}");

            var cost = 3 * buttonBPressesToAlignX + buttonAPresses;
            if (cost < minCost) {
                minCost = cost;
                break;
            }
        }
    }

    return minCost;
}

(long x, long y) FindUberBlockSize((long ax, long ay, long bx, long by, long px, long py) m) {
    // var lcmX = lcm(m.ax, m.bx);
    // var lcmY = lcm(m.ay, m.by);
    return (100000000000L, 100000000000L);
}

void Run(string input) {

    string pattern = @"Button A: X\+(\d+), Y\+(\d+)\s+Button B: X\+(\d+), Y\+(\d+)\s+Prize: X=(\d+), Y=(\d+)";
    var matches = Regex.Matches(input, pattern);
    
    var machines = new List<(int ax, int ay, int bx, int by, int px, int py)>();

    foreach (Match match in matches) {
        int ax = int.Parse(match.Groups[1].Value);
        int ay = int.Parse(match.Groups[2].Value);
        int bx = int.Parse(match.Groups[3].Value);
        int by = int.Parse(match.Groups[4].Value);
        int px = int.Parse(match.Groups[5].Value);
        int py = int.Parse(match.Groups[6].Value);
        
        machines.Add((ax, ay, bx, by, px, py));
    }

    var result = 0L;
    int machineNum = 0;

    foreach (var m in machines) {

        var uberBlockSize = FindUberBlockSize(m);
        var maxUberBlocks = Math.Min(m.px / uberBlockSize.x, m.py / uberBlockSize.y);
        var uberAPresses = uberBlockSize.x / m.ax * maxUberBlocks; // will be the same as uberBlockSize.y / m.ay
        var uberBPresses = uberBlockSize.x / m.bx * maxUberBlocks; // will be the same as uberBlockSize.y / m.by
        var uberBlockCost = Math.Min(uberAPresses * 3, uberBPresses);
        
        // var uberCost = Math.Min()

        var px = m.px - (maxUberBlocks * uberBlockSize.x);
        var py = m.py - (maxUberBlocks * uberBlockSize.y);

        var minCost = SolvePart1((m.ax, m.ay, m.bx, m.by, px, py));

        if (minCost != long.MaxValue) {
            Console.WriteLine($"Machine {machineNum}: uber: {uberBlockCost} remain {minCost}");
            result += (minCost + uberBlockCost);
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
