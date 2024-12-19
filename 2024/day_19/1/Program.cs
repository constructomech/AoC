using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


bool FindDesign(string desiredPattern, Dictionary<char, List<string>> towelPatterns) {

    var visitied = new bool[desiredPattern.Length + 1];

    var q = new Queue<int>();
    q.Enqueue(0);

    while (q.Count > 0) {
        var desiredPatternIndex = q.Dequeue();
        
        if (desiredPatternIndex == desiredPattern.Length) {
            return true;
        }

        visitied[desiredPatternIndex] = true;

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
                    if (!q.Contains(nextIndex) && !visitied[nextIndex]) {
                        q.Enqueue(nextIndex);
                    }
                }
            }
        }
    }
    return false;
}

void Run(string[] input) {
    var result = 0L;

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
        if (FindDesign(desiredPattern, towelPatterns)) {
            result++;
        }
    }

    Console.WriteLine($"Result: {result}");
}
