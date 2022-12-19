using System.Collections.Immutable;
using System.Diagnostics;



Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");
var map = new Dictionary<Location, int?>();

foreach (var line in input)
{
    var parts = line.Split(',');
    Location loc = new Location( 
        Convert.ToInt32(parts[0]),
        Convert.ToInt32(parts[1]),
        Convert.ToInt32(parts[2]));

    map.Add(loc, null);    
}

int totalVisible = 0;
foreach (var (loc, unused) in map)
{
    var xPos = new Location(loc.x + 1, loc.y, loc.z);
    var xNeg = new Location(loc.x - 1, loc.y, loc.z);
    var yPos = new Location(loc.x, loc.y + 1, loc.z);
    var yNeg = new Location(loc.x, loc.y - 1, loc.z);
    var zPos = new Location(loc.x, loc.y, loc.z + 1);
    var zNeg = new Location(loc.x, loc.y, loc.z - 1);

    foreach (var checkPos in new List<Location>() { xPos, xNeg, yPos, yNeg, zPos, zNeg })    
    {
        int count = 0;
        if (!map.ContainsKey(checkPos)) 
        {
            count++;
        }

        map[loc] = count;
        totalVisible += count;
    }
}


// Implement here


watch.Stop();
Console.WriteLine($"{totalVisible}, Completed in {watch.ElapsedMilliseconds}ms");


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}

record Location(int x, int y, int z);