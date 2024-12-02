using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {

    int result = 0;
    var lists = input.Select(x => x.Split(' ').Select(int.Parse).ToList()).ToList();

    foreach (var list in lists) {

        bool safe = true;
        bool? ascending = null;

        for (int i = 1; i < list.Count; i++) {
            bool localAscending = list[i] > list[i - 1];
            int diff = Math.Abs(list[i] - list[i - 1]);

            if (ascending == null) {
                ascending = localAscending;
            }

            if (localAscending != ascending || diff < 1 || diff > 3) {
                safe = false;
                break;
            }
        }

        if (safe) {
            result++;
        }
    }
    Console.WriteLine(result);
}
