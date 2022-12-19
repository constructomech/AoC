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

// Flood fill a shell around the map (all locations 1 away from map including 
//   diagonals) building floodFillLocations
//
var blockAtRandom = map.First();
var start = new Location(min.x - 1, blockAtRandom.y, blockAtRandom.z);
for (int outermostX = min.x -1; outermostX < max.x; outermostX++)
{
    if (map.Contains(new Location(outermostX, min.y, min.z)))
    {
        start = new Location(outermostX - 1, min.y, min.z);
        break;
    }   
}


var queue = new Queue<Location>();
var visited = new HashSet<Location>();
queue.Enqueue(start);
while (queue.Count > 0) {
    var loc = queue.Dequeue();

    visited.Add(loc);

    bool hasAdjacentMapBlock = false;
    var xPos = new Location(loc.x + 1, loc.y, loc.z);
    var xNeg = new Location(loc.x - 1, loc.y, loc.z);
    var yPos = new Location(loc.x, loc.y + 1, loc.z);
    var yNeg = new Location(loc.x, loc.y - 1, loc.z);
    var zPos = new Location(loc.x, loc.y, loc.z + 1);
    var zNeg = new Location(loc.x, loc.y, loc.z - 1);
    foreach (var adjacentPos in new List<Location>() { xPos, xNeg, yPos, yNeg, zPos, zNeg })
    {
        if (map.Contains(adjacentPos))
        {
            hasAdjacentMapBlock = true;
            break;
        }       
    }

    if (hasAdjacentMapBlock)
    {
        floodFillLocations.Add(loc);

        // Enqueue checks for map blocks direction adjacent
        // Plus the 12 diagonally adjacent
        var xPosyPos = new Location(loc.x + 1, loc.y + 1, loc.z);
        var xPosyNeg = new Location(loc.x + 1, loc.y - 1, loc.z);
        var xNegyPos = new Location(loc.x - 1, loc.y + 1, loc.z);
        var xNegyNeg = new Location(loc.x - 1, loc.y - 1, loc.z);
        var xPoszPos = new Location(loc.x + 1, loc.y, loc.z + 1);
        var xPoszNeg = new Location(loc.x + 1, loc.y, loc.z - 1);
        var xNegzPos = new Location(loc.x - 1, loc.y, loc.z + 1);
        var xNegzNeg = new Location(loc.x - 1, loc.y, loc.z - 1);
        var yPoszPos = new Location(loc.x, loc.y + 1, loc.z + 1);
        var yPoszNeg = new Location(loc.x, loc.y + 1, loc.z - 1);
        var yNegzPos = new Location(loc.x, loc.y - 1, loc.z + 1);
        var yNegzNeg = new Location(loc.x, loc.y - 1, loc.z - 1);

        foreach (var checkPos in new List<Location>() { 
            xPos, xNeg, yPos, yNeg, zPos, zNeg })
            // xPosyPos, xPosyNeg, xNegyPos, xNegyNeg,
            // xPoszPos, xPoszNeg, xNegzPos, xNegzNeg,
            // yPoszPos, yPoszNeg, yNegzPos, yNegzNeg })
        {
            if (!map.Contains(checkPos) && !visited.Contains(checkPos))
            {
                queue.Enqueue(checkPos);
            }
        }
    }
}

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