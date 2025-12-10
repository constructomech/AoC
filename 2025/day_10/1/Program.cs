using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var machines = new List<(List<bool> lightTarget, List<List<int>> buttons, List<int> joltages)>();
    foreach (var line in input)
    {
        var parts = line.Split(' ');
        var lightTarget = parts[0].Substring(1, parts[0].Length - 2).Select(c => c == '#').ToList();

        var buttons = new List<List<int>>();
        foreach (var part in parts[1..(parts.Length - 1)])
        {
            var button = part[1..(part.Length - 1)].Split(',').Select(i => Convert.ToInt32(i)).ToList();
            buttons.Add(button);
        }

        var lastPart = parts[parts.Length - 1];
        var joltages = lastPart[1..(lastPart.Length - 1)].Split(',').Select(c => Convert.ToInt32(c)).ToList();

        machines.Add((lightTarget, buttons, joltages));
    }

    foreach (var machine in machines)
    {
        var minPresses = FindMinPressesToTarget(machine);
        result += minPresses;
    }

    Console.WriteLine($"Result: {result}");
}

int FindMinPressesToTarget((List<bool> lightTarget, List<List<int>> buttons, List<int> joltages) machine)
{
    for (var presses = 1; true; presses++)
    {
        if (TryNPresses(machine, presses))
        {
            return presses;
        }
    }
}

bool TryNPresses((List<bool> lightTarget, List<List<int>> buttons, List<int> joltages) machine, int presses)
{
    foreach (var buttonsToPress in GetPermutations(machine.buttons, presses))
    {
        var lights = new List<bool>(Enumerable.Repeat(false, machine.lightTarget.Count));
        foreach (var button in buttonsToPress)
        {
            foreach (var lightToggle in button)
            {
                lights[lightToggle] = !lights[lightToggle];
            }
        }

        if (lights.SequenceEqual(machine.lightTarget))
        {
            return true;
        }
    }
    return false;
}

static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count) {
    var i = 0;
    foreach(var item in items) {
        if(count == 1) {
            yield return new T[] { item };
        }
        else {
            foreach(var result in GetPermutations(items.Skip(i + 1), count - 1)) {
                yield return new T[] { item }.Concat(result);
            }
        }
        i++;
    }
}
