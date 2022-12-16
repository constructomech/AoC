
using System.Drawing;
using System.Text.RegularExpressions;

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

for (int x = 0; x <= 4000000; x++)
{
    for (int y = 0; y <= 4000000; y++)
    {
        Point test = new Point(x, y);

        if (beacons.Contains(test) || sensors.ContainsKey(test))
        {
            // Position isn't valid for a potential beacon becuase it's already occupied.
            continue;
        }

        bool seen = false;
        foreach ((Point pos, int range) in sensors)
        {
            if (Fun.ManhattanDistance(test, pos) <= range)
            {
                // Sensor would have seen this position.
                seen = true;
                break;
            }
        }
        if (!seen) {
            Console.WriteLine("No sensor saw ({0}, {1}) with freq: {2}", test.X, test.Y, test.X * 4000000 + test.Y);
        }
    }
}


static class Fun
{
    public static int ManhattanDistance(Point lhs, Point rhs)
    {
        return (Math.Abs(lhs.X - rhs.X) + Math.Abs(lhs.Y - rhs.Y));
    }
}