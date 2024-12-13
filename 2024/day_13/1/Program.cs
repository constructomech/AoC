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

        var minCost = int.MaxValue;

        for (var buttonAPresses = 0; buttonAPresses < 100; buttonAPresses++) {

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

        if (minCost != int.MaxValue) {
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
