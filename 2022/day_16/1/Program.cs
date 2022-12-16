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




/*
Node? bestSolution = null;
// int bestTotalPressure = 0; //1500; // Prepop from earlier run  (test data)
int bestTotalPressure = 1741; // Prepop from earlier run (part 1 data 1684, using less to see some output)

var queue = new PriorityQueue<Node>();

queue.Enqueue(new Node("AA", false, 30, 0, 0, int.MaxValue, null));
while (queue.Count > 0)
{
    var current = queue.Dequeue();

    if (current.timeReamining == 0)
    {
        if (current.totalPressure > bestTotalPressure)
        {
            Console.WriteLine();
            Console.WriteLine("Found best solution: {0} total pressure.  {1} items in queue.", current.totalPressure, queue.Count);

            bestSolution = current;
            bestTotalPressure = current.totalPressure;
        }
        continue;
    }

    // If three are unopened valves with > 0 flow rate, continue moving rooms
    var lostTotalPressure = 0;
    var potentialRemainingTotalPressure = 0;
    var remainingValves = new HashSet<string>();
    foreach ((var valve, int flowRate) in flowRates)
    {
        if (flowRate > 0)
        {
            remainingValves.Add(valve);
        }
    }
    foreach (var openValve in current.OpenValves) 
    {
        remainingValves.Remove(openValve);
        potentialRemainingTotalPressure += current.timeReamining * flowRates[openValve];  // Add open valves potential remaining
    }
    foreach (var remainingValve in remainingValves) 
    {
        lostTotalPressure += (30 - current.timeReamining) * flowRates[remainingValve];
    }

    // To calculate the potential remaining total pressure, simulate opening remaining valves largest first, every 2 moves.
    int remainingMinutes = current.timeReamining;
    foreach ((string  valve, int flowRate) in flowRates.OrderByDescending(kvp => kvp.Value))
    {
        if (remainingValves.Contains(valve)) 
        {
            potentialRemainingTotalPressure += remainingMinutes * flowRate;
            remainingMinutes -= 2;
            if (remainingMinutes <= 0) {
                break;
            }
        }
    }

    if (current.totalPressure + potentialRemainingTotalPressure < bestTotalPressure)
    {
        continue;
    }

    // If we haven't opened this valve yet, try that
    var currentRoomFlowRate = flowRates[current.room];
    if (currentRoomFlowRate > 0 && !current.valveOpened && !current.OpenValves.Contains(current.room))
    {
        queue.Enqueue(new Node(current.room, true, current.timeReamining - 1, current.flowRate + currentRoomFlowRate, current.totalPressure + current.flowRate, lostTotalPressure, current));
    }

    if (remainingValves.Count > 0) {
        // Move to all possible rooms
        foreach (var room in doors[current.room])
        {
            queue.Enqueue(new Node(room, false, current.timeReamining - 1, current.flowRate, current.totalPressure + current.flowRate, lostTotalPressure, current));
        }
    }

    // Or finally, do nothing.
    queue.Enqueue(new Node(current.room, false, current.timeReamining - 1, current.flowRate, current.totalPressure + current.flowRate, lostTotalPressure, current));
}

bestSolution?.PrintSolution(flowRates);

watch.Stop();

Console.WriteLine("Result: {0} in {1}ms", bestTotalPressure, watch.ElapsedMilliseconds);


class Node : IComparable<Node>
{
    public Node(string room, bool valveOpened, int timeReamining, int flowRate, int totalPressure, int lostTotalPressure, Node? parent)
    {
        this.room = room;
        this.valveOpened = valveOpened;
        this.timeReamining = timeReamining;
        this.flowRate = flowRate;
        this.totalPressure = totalPressure;
        this.lostTotalPressure = lostTotalPressure;
        this.Parent = parent;
    }

    public int CompareTo(Node? other)
    {
        int h = this.timeReamining * this.lostTotalPressure / ((this.flowRate == 0)?(1):this.flowRate);
        int hOther = other.timeReamining * other.lostTotalPressure / ((other.flowRate == 0)?(1):other.flowRate);
        
        return h.CompareTo(hOther);


        // var result = 0;

        // Lower is better
        // result = this.timeReamining.CompareTo(other?.timeReamining);

        // Lower is better
        // if (result == 0) {
        //     result = this.lostTotalPressure.CompareTo(other?.lostTotalPressure);
        // }
        
        // Higher is better
        // if (result == 0) {
        //     result = -this.flowRate.CompareTo(other?.flowRate);
        // }

        // if (result == 0) {
        //     // Higher is better
        //     result = -this.totalPressure.CompareTo(other?.totalPressure);
        // }

        // return result;
    }

    public List<string> OpenValves
    {
        get
        {
            var result = new List<string>();
            Node? current = this;
            while (current != null)
            {
                if (current != this && current.valveOpened)
                {
                    result.Add(current.room);
                }
                current = current.Parent;
            }

            return result;
        }
    }

    public void PrintSolution(Dictionary<string, int> flowRates)
    {
        var moves = new List<Node>();
        var current = this;
        while (current != null)
        {
            moves.Insert(0, current);
            current = current.Parent;
        }

        foreach (var move in moves)
        {
            if (move.Parent != null) 
            {
                Console.WriteLine("== Minute {0}==", 30 - move.timeReamining);

                var unopenedValves = new HashSet<string>(flowRates.Keys);

                var valvesOpenList = move.OpenValves;
                if (valvesOpenList.Count == 0) 
                {
                    Console.WriteLine("No valves are open");
                }
                else 
                {
                    string valvesOpen = "";
                    int totalFlowRate = 0;
                    foreach (var openValve in valvesOpenList)
                    {
                        unopenedValves.Remove(openValve);
                        valvesOpen += openValve + " ";
                        totalFlowRate += flowRates[openValve];
                    }

                    Console.WriteLine("{0}valves open, releasing {1} pressure.", valvesOpen, totalFlowRate);
                }

                if (move.valveOpened)
                {
                    Console.WriteLine("You open valve {0}", move.room);
                }
                else
                {
                    Console.WriteLine("You move to room {0}", move.room);
                }

                var potentialPressure = 0;
                foreach (var unopened in unopenedValves) 
                {
                    potentialPressure += flowRates[unopened] * move.timeReamining;
                }

                Console.WriteLine("                                                 [total pressure: {0}, lost total pressure: {1}, potentialPreasure: {2}]", move.totalPressure, move.lostTotalPressure, potentialPressure );
            }
        }
    }

    public string room = "";
    public bool valveOpened = false;
    public int timeReamining = 0;
    public int flowRate = 0;
    public int totalPressure = 0;
    public int lostTotalPressure = 0;
    public Node? Parent = null;
}
*/