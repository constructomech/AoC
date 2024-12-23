using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

bool AllConnected(Dictionary<string, List<string>> connectedTo, IEnumerable<string> set) {
    foreach (var c0 in set) {
        foreach (var c1 in set) {
            if (c0 != c1) {
                if (!connectedTo[c0].Contains(c1)) {
                    return false;
                }
            }
        }
    }
    return true;
}

void Run(string[] input) {
    var connected = input.Select(s => s.Split('-').ToList()).ToList();
    var all = new HashSet<string>();

    var connectedTo = new Dictionary<string, List<string>>();
    foreach (var con in connected) {
        all.Add(con[0]);
        all.Add(con[1]);
        connectedTo.TryAdd(con[0], new List<string>());
        connectedTo[con[0]].Add(con[1]);
        connectedTo.TryAdd(con[1], new List<string>());
        connectedTo[con[1]].Add(con[0]);
    }

    var maxConnections = connectedTo.Max(kvp => kvp.Value.Count);

    var sets = new List<SortedSet<string>>();
    foreach (var c in all) {
        sets.Add(new() { c });
    }

    for (var i = 0; i < sets.Count; i++) {

        foreach (var c in all) {
            // Add this computer to the set if all computers in the set are connected to each other.
            if (AllConnected(connectedTo, sets[i].Concat([c]))) {
                sets[i].Add(c);
            }
        }
    }

    var top = sets.OrderBy(s => -s.Count).First();

    Console.WriteLine($"{string.Join(",", top)}");
}
