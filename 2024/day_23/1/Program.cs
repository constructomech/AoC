using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

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

    var setsOfThree = new HashSet<HashSet<string>>();
    foreach (var permutation in GetPermutations(all, 3)) {
        var computers = permutation.ToList();
        var a = computers[0];
        var b = computers[1];
        var c = computers[2];
        
        if (connectedTo[a].Contains(b) && connectedTo[a].Contains(c) &&
            connectedTo[b].Contains(a) && connectedTo[b].Contains(c) && 
            connectedTo[c].Contains(a) && connectedTo[c].Contains(b)) {

            var set = new HashSet<string>() { computers[0], computers[1], computers[2] };
            setsOfThree.Add(set);
        }
    }

    result = setsOfThree.Count(s => s.Where(c => c.StartsWith('t')).Any());

    Console.WriteLine($"Result: {result}");
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
