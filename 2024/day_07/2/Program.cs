using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


bool CouldBeValid(long key, List<int> values) {

    var queue = new Queue<(long result, int valueIdx)>();
    queue.Enqueue((values[0], 1));

    while (queue.Count > 0) {
        (var total, var valueIdx) = queue.Dequeue();

        if (valueIdx == values.Count) {
            if (total == key) {
                return true;
            }
            continue;
        }

        var addResult = total + values[valueIdx];
        queue.Enqueue((addResult, valueIdx + 1));

        var multResult = total * values[valueIdx];
        queue.Enqueue((multResult, valueIdx + 1));

        var concatResult = long.Parse(total.ToString() + values[valueIdx].ToString());
        queue.Enqueue((concatResult, valueIdx + 1));
    }

    return false;
}

void Run(string[] input) {

    var result = 0L;

    foreach (var line in input) {
        var parts = line.Split(": ");
        var key = Convert.ToInt64(parts[0]);
        var values = new List<int>();        
        foreach (var item in parts[1].Split(' ')) {
            var value = Convert.ToInt32(item);
            values.Add(value);
        }

        if (CouldBeValid(key, values)) {
            result += key;
        }
    }
    
    Console.WriteLine($"Result: {result}");
}
