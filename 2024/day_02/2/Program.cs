using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

bool CheckSafety(List<int> list, int? ignoreIndex) {
    bool? ascending = null;

    int? priorValue = null;

    for (int i = 0; i < list.Count; i++) {
        if (ignoreIndex.HasValue && i == ignoreIndex) {
            continue;
        }

        int currentValue = list[i];

        if (priorValue.HasValue) {

            bool localAscending = currentValue > priorValue.Value;
            int diff = Math.Abs(currentValue - priorValue.Value);

            if (ascending == null) {
                ascending = localAscending;
            }

            if (localAscending != ascending || diff < 1 || diff > 3) {
                return false;
            }
        }
        
        priorValue = currentValue;
    }
    return true;
}

void Run(string[] input) {

    int result = 0;
    var lists = input.Select(x => x.Split(' ').Select(int.Parse).ToList()).ToList();

    foreach (var list in lists) {

        int problemCount = 0;
        bool? ascending = null;

        bool safe = CheckSafety(list, null);
        if (!safe) {
            for (int i = 0; i < list.Count; i++) {
                if (CheckSafety(list, i)) {
                    Console.WriteLine("Ignored index " + i + " (" + list[i] + ")");
                    safe = true;
                    break;
                }
            }
        }

        foreach (var num in list) {
            Console.Write(num + " ");
        }
        Console.WriteLine(safe);

        if (safe) {
            result++;
        }
    }
    Console.WriteLine(result);
}
