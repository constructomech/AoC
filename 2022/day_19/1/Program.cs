using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

// List of blueprints which map <resource name> -> (costs indexed by resource)
var blueprints = new List<Dictionary<Resource, int[]>>();

var input = File.ReadAllLines("input.txt");
foreach (var line in input) 
{
    var blueprint = new Dictionary<Resource, int[]>();

    var parts = line.Split(':');
    parts = parts[1].Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    foreach (var phrase in parts)
    {
        var words = phrase.Split(' ');
        
        // Each ore robot costs 4 ore.
        // Each obsidian robot costs 4 ore and 16 clay.
        var resource = (Resource)Enum.Parse(typeof(Resource), words[1]);
        var recipeReqs = new int[4];

        var count1 = Convert.ToInt32(words[4]);
        var res1 = (int)Enum.Parse(typeof(Resource), words[5]);
        recipeReqs[res1] = count1;

        if (words.Length >= 8) {
            var count2 = Convert.ToInt32(words[7]);
            var res2 = (int)Enum.Parse(typeof(Resource), words[8]);
            recipeReqs[res2] = count2;
        }
        blueprint.Add(resource, recipeReqs);
    }
    blueprints.Add(blueprint);
}

int maxGeodes = 0;
int sumOfQuality = 0;
for (var i = 0; i < blueprints.Count; i++) 
{
    Console.WriteLine($"Solving blueprint {i}");
    var blueprint = blueprints[i];
    int crackedGeodes = Solve(blueprint);
    int qualityLevel = (i + 1) * crackedGeodes;

    sumOfQuality += qualityLevel;
}


watch.Stop();
Console.WriteLine($"Result: {sumOfQuality} Completed in {watch.ElapsedMilliseconds}ms");


//------------------------------------------------------------------------------------------
// Maximize opened Geodes in 24 minutes
//   ore collecting
//   clay collecting
//   obsidian collecting
//   geode cracking
// Resouces (ore, clay, obsidian, geodes, cracked_geodes)
//

int Solve(Dictionary<Resource, int[]> blueprint)
{
    maxGeodes = 0;

    InternalSolve(blueprint, 24, new Resources(1, 0, 0, 0, new int[4]));

    return maxGeodes;
}


void InternalSolve(Dictionary<Resource, int[]> blueprint, int timeRemaining, Resources @in) 
{
    if (timeRemaining <= 0) return;

    if (@in.resourceCounts[(int)Resource.geode] > maxGeodes)
    {
        maxGeodes = @in.resourceCounts[(int)Resource.geode];
        Console.WriteLine($"New max cracked geodes {maxGeodes}");
    }

    // Cut off branches with no chance of beating maxGeodes
    //   If I ever see a inventory that's a subset of any step less than or equal to my step, discard

    // Do production
    //
    var postProductionResourceCounts = new int[4];
    postProductionResourceCounts[(int)Resource.ore] = @in.resourceCounts[(int)Resource.ore] + @in.oreRobots;
    postProductionResourceCounts[(int)Resource.clay] = @in.resourceCounts[(int)Resource.clay] + @in.clayRobots;
    postProductionResourceCounts[(int)Resource.obsidian] = @in.resourceCounts[(int)Resource.obsidian] + @in.obsidianRobots;
    postProductionResourceCounts[(int)Resource.geode] = @in.resourceCounts[(int)Resource.geode] + @in.geodeRobots;

    // If resource needs are met, attempt a build of each type of robot recursively
    // 
    foreach (var (resource, recipe) in blueprint) 
    {
        var costs = blueprint[resource];

        bool valid = true;
        var liquidResourceCounts = (int[])@in.resourceCounts.Clone();
        for (int i = 0; i < 4; i++) 
        {
            liquidResourceCounts[i] -= recipe[i];
            if (liquidResourceCounts[i] < 0) {
                valid = false;
                break;
            }
        }

        if (valid) {
            int ord = 0, cld = 0, obd = 0, ged = 0;
            switch (resource) {
                case Resource.ore:
                    ord++;
                    break;
                case Resource.clay:
                    cld++;
                    break;
                case Resource.obsidian:
                    obd++;
                    break;
                case Resource.geode:
                    ged++;
                    break;
            }

            for (int i = 0; i < 4; i++) 
            {
                postProductionResourceCounts[i] -= recipe[i];
            }

            InternalSolve(blueprint, timeRemaining - 1, 
                new Resources(@in.oreRobots + ord, @in.clayRobots + cld, @in.obsidianRobots + obd, @in.geodeRobots + ged, postProductionResourceCounts));
        }
    }

    InternalSolve(blueprint, timeRemaining - 1, 
        new Resources(@in.oreRobots, @in.clayRobots, @in.obsidianRobots, @in.geodeRobots, postProductionResourceCounts));
}

record Resources(int oreRobots, int clayRobots, int obsidianRobots, int geodeRobots, int[] resourceCounts);

enum Resource 
{
    ore = 0,
    clay = 1,
    obsidian = 2,
    geode = 3,
}