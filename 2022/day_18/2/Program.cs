using System.Collections.Immutable;
using System.Diagnostics;



Stopwatch watch = new Stopwatch();
watch.Start();

var min = new Location(int.MaxValue, int.MaxValue, int.MaxValue);
var max = new Location(int.MinValue, int.MinValue, int.MinValue);

var input = File.ReadAllLines("input.txt");
var map = new HashSet<Location>();
var floodFillLocations = new HashSet<Location>();

foreach (var line in input)
{
    var parts = line.Split(',');
    Location loc = new Location( 
        Convert.ToInt32(parts[0]),
        Convert.ToInt32(parts[1]),
        Convert.ToInt32(parts[2]));

    min = new Location(
        Math.Min(min.x, loc.x),
        Math.Min(min.y, loc.y),
        Math.Min(min.z, loc.z));

    max = new Location(
        Math.Max(max.x, loc.x),
        Math.Max(max.y, loc.y),
        Math.Max(max.z, loc.z));

    map.Add(loc);    
}

min = new Location(min.x - 1, min.y - 1, min.z - 1);
max = new Location(max.x + 1, max.y + 1, max.z + 1);

// Flood fill arond the map building floodFillLocations
//
var queue = new Queue<Location>();
var visited = new HashSet<Location>();
visited.Add(min);
queue.Enqueue(min);
while (queue.Count > 0) {
    var loc = queue.Dequeue();

    floodFillLocations.Add(loc);

    var xPos = new Location(loc.x + 1, loc.y, loc.z);
    var xNeg = new Location(loc.x - 1, loc.y, loc.z);
    var yPos = new Location(loc.x, loc.y + 1, loc.z);
    var yNeg = new Location(loc.x, loc.y - 1, loc.z);
    var zPos = new Location(loc.x, loc.y, loc.z + 1);
    var zNeg = new Location(loc.x, loc.y, loc.z - 1);

    foreach (var checkPos in new List<Location>() { xPos, xNeg, yPos, yNeg, zPos, zNeg })
    {
        if (!visited.Contains(checkPos) &&
            !map.Contains(checkPos) &&
            checkPos.x >= min.x && checkPos.x <= max.x && 
            checkPos.y >= min.y && checkPos.y <= max.y && 
            checkPos.z >= min.z && checkPos.z <= max.z)
        {
            visited.Add(checkPos);
            queue.Enqueue(checkPos);
        }
    }
}

// Test data should have 5 x 5 x 8 - 13 - 1 = 186 locations
//   Actual data should have 23 x 22 x 23 - 2817 - <interior space> locations
//   That would give us ~9000 locations
// Now count the faces that are adjacent to any floodFilllocation
//
int totalVisible = 0;
foreach (var loc in map)
{
    var xPos = new Location(loc.x + 1, loc.y, loc.z);
    var xNeg = new Location(loc.x - 1, loc.y, loc.z);
    var yPos = new Location(loc.x, loc.y + 1, loc.z);
    var yNeg = new Location(loc.x, loc.y - 1, loc.z);
    var zPos = new Location(loc.x, loc.y, loc.z + 1);
    var zNeg = new Location(loc.x, loc.y, loc.z - 1);

    foreach (var checkPos in new List<Location>() { xPos, xNeg, yPos, yNeg, zPos, zNeg })
    {
        if (floodFillLocations.Contains(checkPos))
        {
            totalVisible++;
        }
    }
}

watch.Stop();
Console.WriteLine($"{totalVisible}, Completed in {watch.ElapsedMilliseconds}ms");


record Location(int x, int y, int z);