using System.Collections.Immutable;
using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();

// You estimate it will take you one minute to open a single valve and one minute to follow any tunnel from one valve to another. 
//   What is the most pressure you could release?


// Parse
//
var input = File.ReadAllLines("input.txt");
var allValves = new Dictionary<int, Valve>();
int valveId = 0;
foreach (var line in input)
{
    // Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
    var words = line.Split(' ');

    var name = words[1];
    var rate = Convert.ToInt32(words[4].Split('=')[1].Replace(";", ""));

    var destinations = new List<string>();
    for (int i = 9; i < words.Length; i++)
    {
        destinations.Add(words[i].Split(',')[0]);
    }
    allValves.Add(valveId, new Valve(valveId, name, rate, destinations, new Dictionary<int, int>()));
    valveId++;
}
var valvesByName = allValves.Values.ToDictionary(v => v.name, v => v);
var significantValves = allValves.Values.Where(v => v.flowRate > 0).ToArray();


// Simplify the map
//
foreach (var valve in significantValves.Append(valvesByName["AA"])) 
{
    foreach (var other in significantValves) 
    {
        if (valve == other) continue;

        valve.distances[other.id] = CalculateDistance(valve, other);
    }
}


// Solve
//
int maxPressure = 0;
var start = valvesByName["AA"];
foreach (var significantValve in significantValves) 
{
    var dist = CalculateDistance(start, significantValve);

    Traverse(dist, ImmutableHashSet<int>.Empty.Add(significantValve.id), significantValve, significantValve.flowRate * (30 - dist));
}

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Traverse(int currentDistance, ImmutableHashSet<int> visited, Valve currentValve, int totalPressure)
{
    if (currentDistance > 30) return;
    if (totalPressure > maxPressure)
    {
        maxPressure = totalPressure;
        Console.WriteLine($"New max pressure: {maxPressure}");
    }

    foreach (var (id, distance) in currentValve.distances)
    {
        if (visited.Contains(id)) continue;
        
        var nextValve = allValves[id];
        var newFlowrate = nextValve.flowRate * (30 - currentDistance - distance);

        Traverse(currentDistance + distance, visited.Add(id), nextValve, totalPressure + newFlowrate);
    }
}

int CalculateDistance(Valve start, Valve end) 
{
    // Distance is always +1 for opening the valve

    var queue = new Queue<(Valve valve, int depth)>();  // Valve id, depth
    var visited = new HashSet<Valve>();
    queue.Enqueue((start, 0));
    while (queue.Count > 0) 
    {
        var (current, depth) = queue.Dequeue();
        if (current == end) 
        {
            return depth + 1;
}

        visited.Add(current);

        foreach (var adjacent in current.targets) 
        {
            var adjacentVavle = valvesByName[adjacent];
            if (!visited.Contains(adjacentVavle))
            {
                queue.Enqueue((adjacentVavle, depth + 1));
            }
        }
    }
    throw new InvalidOperationException();
}

record Valve(int id, string name, int flowRate, List<string> targets, Dictionary<int, int> distances);
