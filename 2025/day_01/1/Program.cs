using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;
    var dial = 50L;

    for (int i = 0; i < input.Length; i++)
    {
        Direction dir = (input[i][0] == 'R') ? Direction.Right : Direction.Left;
        int steps = int.Parse(input[i].Substring(1));

        if (dir == Direction.Right)
        {
            dial += steps;
        }
        else
        {
            dial -= steps;
        }

        if (dial == 0)
        {
            result++;
        }
        while (dial < 0) { dial += 100; }
        while (dial >= 100 ) { dial -= 100; }
    }

    Console.WriteLine($"Result: {result}");
}

enum Direction { Up, Down, Left, Right }
