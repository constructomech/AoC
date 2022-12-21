using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();


var input = File.ReadAllLines("input.txt");
var blueprints = Parse(input).ToList();

//var cache = new Dictionary<State, int>();

int sumOfQuality = 0;
for (var i = 0; i < blueprints.Count; i++) 
{
    Console.WriteLine($"Solving blueprint {i}");

    var blueprint = blueprints[i];
    int geodes = MaxGeodes(blueprint, 24);
    int qualityLevel = (i + 1) * geodes;

    sumOfQuality += qualityLevel;
}

watch.Stop();
Console.WriteLine($"Result: {sumOfQuality} Completed in {watch.ElapsedMilliseconds}ms");


int MaxGeodes(Blueprint blueprint, int maxTime) 
{
    int maxGeodes = 0;
    var stack = new Stack<State>();
    stack.Push(new State(maxTime, Material.Zero, new Material(1, 0, 0, 0)));

    while (stack.Count > 0)
    {
        var current = stack.Pop();

        if (current.remainingTime == 0) {

            if (current.inventory.geode > maxGeodes)
            {
                maxGeodes = current.inventory.geode;
                Console.WriteLine($"  Found new maximum: {maxGeodes}");
            }
        }
        else
        {
            foreach (var nextState in NextStates(maxTime, blueprint, current))
            {
                stack.Push(nextState);
            }
        }
    }

    return maxGeodes;
}

IEnumerable<State> NextStates(int maxTime, Blueprint blueprint, State current)
{
    // Return the states in the order of least important to most important
    //   since we're adding to a stack.
    int turnsToProduce = int.MaxValue;
    Material? newInventory = null;
    int timeRemaining = 0;
    bool noRobots = true;

    // Consider building an ore robot
    if (current.production.ore < blueprint.maxOreNeed)
    {
        (turnsToProduce, newInventory) = TurnsToProduce(blueprint.ore, current);
        timeRemaining = current.remainingTime - turnsToProduce;
        if (timeRemaining > 0) 
        {
            noRobots = false;
            yield return new State(timeRemaining, newInventory!, current.production + blueprint.ore.produces);
        }
    }

    // Consider building a clay robot
    if (current.production.clay < blueprint.maxClayNeed)
    {
        (turnsToProduce, newInventory) = TurnsToProduce(blueprint.clay, current);
        timeRemaining = current.remainingTime - turnsToProduce;
        if (timeRemaining > 0) 
        {
            noRobots = false;
            yield return new State(timeRemaining, newInventory!, current.production + blueprint.clay.produces);
        }
    }

    // Consider building an obsidian robot
    if (current.production.obsidian < blueprint.maxObsidianNeed)
    {
        (turnsToProduce, newInventory) = TurnsToProduce(blueprint.obsidian, current);
        timeRemaining = current.remainingTime - turnsToProduce;
        if (timeRemaining > 0)
        {
            noRobots = false;
            yield return new State(timeRemaining, newInventory!, current.production + blueprint.obsidian.produces);
        }
    }

    // Consider building a geode robot
    (turnsToProduce, newInventory) = TurnsToProduce(blueprint.geode, current);
    timeRemaining = current.remainingTime - turnsToProduce;
    if (timeRemaining > 0) 
    {
        noRobots = false;
        yield return new State(timeRemaining, newInventory!, current.production + blueprint.geode.produces);
    }

    if (noRobots)
    {
        yield return new State(0, current.inventory + current.production * current.remainingTime, current.production);
    }
}

(int turns, Material? inventory) TurnsToProduce(Robot robot, State state) 
{
    if (robot.cost.ore > 0 && state.production.ore == 0) return (int.MaxValue, null);
    if (robot.cost.clay > 0 && state.production.clay == 0) return (int.MaxValue, null);
    if (robot.cost.obsidian > 0 && state.production.obsidian == 0) return (int.MaxValue, null);

    int turns = 0;
    var inventory = state.inventory - robot.cost;
    while (true) 
    {
        if (inventory >= Material.Zero)
        {
            return (turns, inventory);
        }

        inventory += state.production;
        turns++;
    }
}

IEnumerable<Blueprint> Parse(string[] input) 
{
    foreach (var line in input) 
    {
        var robots = new List<Robot>();

        var parts = line.Split(':');
        parts = parts[1].Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var phrase in parts)
        {
            var words = phrase.Split(' ');
            
            // Each ore robot costs 4 ore.
            // Each obsidian robot costs 4 ore and 16 clay.
            var produces = Material.FromString(words[1], 1);

            var cost = Material.FromString(words[5], Convert.ToInt32(words[4]));

            if (words.Length >= 8) {
                cost += Material.FromString(words[8], Convert.ToInt32(words[7]));
            }

            robots.Add(new Robot(cost, produces));
        }

        yield return Blueprint.FromRobots(robots);
    }
}

record Material(int ore, int clay, int obsidian, int geode) {
    public static Material Zero = new Material(0, 0, 0, 0);

    public static Material FromString(string name, int quantity)
    {
        switch (name)
        {
            case "ore":       return new Material(quantity, 0, 0, 0);
            case "clay":      return new Material(0, quantity, 0, 0);
            case "obsidian":  return new Material(0, 0, quantity, 0);
            case "geode":     return new Material(0, 0, 0, quantity);
        }
        throw new InvalidProgramException();
    }

    public static Material operator +(Material a, Material b) {
        return new Material(
            a.ore + b.ore,
            a.clay + b.clay,
            a.obsidian + b.obsidian,
            a.geode + b.geode
        );
    }

    public static Material operator *(Material a, int turns) {
        return new Material(
            a.ore * turns,
            a.clay * turns,
            a.obsidian * turns,
            a.geode * turns
        );
    }

    public static Material operator -(Material a, Material b) {
        return new Material(
            a.ore - b.ore,
            a.clay - b.clay,
            a.obsidian - b.obsidian,
            a.geode - b.geode
        );
    }

    public static bool operator <=(Material a, Material b) {
        return
            a.ore <= b.ore &&
            a.clay <= b.clay &&
            a.obsidian <= b.obsidian &&
            a.geode <= b.geode;
    }

    public static bool operator >=(Material a, Material b) {
        return
            a.ore >= b.ore &&
            a.clay >= b.clay &&
            a.obsidian >= b.obsidian &&
            a.geode >= b.geode;
    }
}

record Robot(Material cost, Material produces);

record State(int remainingTime, Material inventory, Material production);

record Blueprint(Robot ore, Robot clay, Robot obsidian, Robot geode, int maxOreNeed, int maxClayNeed, int maxObsidianNeed)
{
    public static Blueprint FromRobots(IEnumerable<Robot> robots)
    {
        Robot? ore = null;
        Robot? clay = null;
        Robot? obsidian = null;
        Robot? geode = null;

        foreach(var robot in robots)
        {
            if (robot.produces.ore > 0) 
            {
                ore = robot;
            } else if (robot.produces.clay > 0) 
            {
                clay = robot;
            } else if (robot.produces.obsidian > 0) 
            {
                obsidian = robot;
            } else
            {
                geode = robot;
            }
        }

        int maxOreNeed = Math.Max(Math.Max(ore.cost.ore, clay.cost.ore), Math.Max(obsidian.cost.ore, geode.cost.ore));
        int maxClayNeed = Math.Max(Math.Max(ore.cost.clay, clay.cost.clay), Math.Max(obsidian.cost.clay, geode.cost.clay));
        int maxObsidianNeed = Math.Max(Math.Max(ore.cost.obsidian, clay.cost.obsidian), Math.Max(obsidian.cost.obsidian, geode.cost.obsidian));

        return new Blueprint(ore!, clay!, obsidian!, geode!, maxOreNeed, maxClayNeed, maxObsidianNeed);
    }
}
