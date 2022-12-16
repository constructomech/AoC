
using System.Drawing;
using System.Text.RegularExpressions;

long result = 0;
var input = File.ReadAllLines("input.txt");

var sensors = new Dictionary<Point, int>();
var beacons = new HashSet<Point>();

foreach (var line in input)
{
    var match = Regex.Match(line, @"Sensor at x=(?<x1>-?\d*\.?\d+), y=(?<y1>-?\d*\.?\d+): closest beacon is at x=(?<x2>-?\d*\.?\d+), y=(?<y2>-?\d*\.?\d+)");

    var sensorPos = new Point(Convert.ToInt32(match.Groups["x1"].Value), Convert.ToInt32(match.Groups["y1"].Value));
    var beaconPos = new Point(Convert.ToInt32(match.Groups["x2"].Value), Convert.ToInt32(match.Groups["y2"].Value));

    sensors.Add(sensorPos, Fun.ManhattanDistance(beaconPos, sensorPos));
    beacons.Add(beaconPos);
}

int maxSensorRange = sensors.Values.Max();

int minX = sensors.Keys.Concat(beacons).Min(p => p.X) - maxSensorRange;
int maxX = sensors.Keys.Concat(beacons).Max(p => p.X) + maxSensorRange;
int rangeX = maxX - minX + 1;

int y = 2000000;
for (int x = minX; x <= maxX; x++)
{
    if (beacons.Where(p => p.X == x && p.Y == y).Any() || sensors.Keys.Where(p => p.X == x && p.Y == y).Any())
    {
        // Position isn't valid for a potential beacon becuase it's already occupied.
        continue;
    }

    foreach ((Point pos, int range) in sensors)
    {
        if (Fun.ManhattanDistance(new Point(x, y), pos) <= range)
        {
            // Sensor would have seen this position.
            result++;
            break;
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static int ManhattanDistance(Point lhs, Point rhs) {
        return (Math.Abs(lhs.X - rhs.X) + Math.Abs(lhs.Y - rhs.Y));
    }
}