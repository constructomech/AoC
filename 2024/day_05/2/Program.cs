using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


bool MeetsRule(List<int> list, (int x, int y) rule) {

    int firstIdx = list.FindIndex(item => item == rule.x);
    int secondIdx = list.FindIndex(item => item == rule.y);

    if (firstIdx != -1 && secondIdx != -1) {
        return firstIdx < secondIdx;    
    }

    return true;
}

void Swap(List<int> list, (int x, int y) rule) {
    int firstIdx = list.FindIndex(item => item == rule.x);
    int secondIdx = list.FindIndex(item => item == rule.y);

    var tmp = list[firstIdx];
    list[firstIdx] = list[secondIdx];
    list[secondIdx] = tmp;
}

void Run(string[] input) {
    long result = 0;

    var rules = new List<(int, int)>();
    var sequences = new List<List<int>>();

    bool processingRules = true;

    for (int i = 0; i < input.Length; i++) {
        
        if (input[i] == "") {
            processingRules = false;
            continue;
        }

        if (processingRules) {
            var parts = input[i].Split("|");
            var lhs = Convert.ToInt32(parts[0]);
            var rhs = Convert.ToInt32(parts[1]);
            rules.Add((lhs, rhs));
        }
        else {
            var parts = input[i].Split(",").Select(int.Parse).ToList();
            sequences.Add(parts);
        }
    }

    foreach (var sequence in sequences) {

        bool valid = true;

        Again:

        foreach (var rule in rules) {
            if (!MeetsRule(sequence, rule)) {
                Swap(sequence, rule);
                valid = false;
                goto Again;
            }
        }

        if (!valid) {
            result += sequence[sequence.Count / 2];
        }
    }

    Console.WriteLine($"Result is {result}");
}
