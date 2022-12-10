using System.IO;
using System.Collections.Generic;

long result = 0;

int x = 1;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            var parts = line.Split(' ');
            switch (parts[0]) {
                case "noop":
                    Fun.IncrementCycle(x);
                    break;
                case "addx":
                    for (int i = 0; i < 2; i++) {
                        Fun.IncrementCycle(x);
                    }
                    x += Convert.ToInt32(parts[1]);
                    break;
            }
            // Implement
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static void IncrementCycle(int x) {
        cycle++;
    
        if ((cycle == 20) || ((cycle - 20) % 40 == 0)) {
            total += (x * cycle);
            Console.WriteLine(total);
        }
    }
    static int cycle = 0;
    static int total = 0;
}