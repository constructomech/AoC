using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


long FindDesign(string desiredPattern, Dictionary<char, List<string>> towelPatterns) {

    var visited = new long[desiredPattern.Length]; // # of ways to get to this index
    var q = new SortedSet<int>();
    q.Add(0);

    while (q.Count > 0) {
        var desiredPatternIndex = q.First();
        q.Remove(desiredPatternIndex);
        
        if (desiredPatternIndex == desiredPattern.Length) {
            break;
        }

        List<string>? availablePatterns = null;
        if (towelPatterns.TryGetValue(desiredPattern[desiredPatternIndex], out availablePatterns)) {

            foreach (var towelPattern in availablePatterns) {

                var equal = true;
                for (int compareIndex = 0; compareIndex < towelPattern.Length; compareIndex++) {
                    var j = desiredPatternIndex + compareIndex;
                    if (j >= desiredPattern.Length || towelPattern[compareIndex] != desiredPattern[j]) {
                        equal = false;
                        break;
                    }
                }

                if (equal) {
                    var nextIndex = desiredPatternIndex + towelPattern.Length;

                    var visitedCount = desiredPatternIndex == 0 ? 1 : visited[desiredPatternIndex - 1];
                    visited[nextIndex - 1] += visitedCount;

                    q.Add(nextIndex);
                }
            }
        }
    }
    return visited[desiredPattern.Length - 1];
}

void Run(string[] input) {
    long result = 0;

    var towelPatterns = new Dictionary<char, List<string>>();
    foreach (var pattern in input[0].Split(", ")) {
        towelPatterns.TryAdd(pattern[0], new List<string>());
        towelPatterns[pattern[0]].Add(pattern);
    }

    var desiredPatterns = new List<string>();
    for (int i = 2; i < input.Length; i++) {
        desiredPatterns.Add(input[i]);
    }

    foreach (var desiredPattern in desiredPatterns) {
        result += FindDesign(desiredPattern, towelPatterns);
    }

    Console.WriteLine($"Result: {result}");
}
