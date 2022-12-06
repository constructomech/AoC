using System.IO;
using System.Collections.Generic;

List<int> fish = new List<int>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            var split = line.Split(',');
            foreach (var item in split) {
                fish.Add(Convert.ToInt32(item));
            }
        }
    }
}

for (int day = 0; day < 256; day++) {
    //Console.Write("Day {0}: ", day);

    int lastInGeneration = fish.Count;
    for (int i = 0; i < lastInGeneration; i++) {
        //Console.Write("{0},", fish[i]);

        fish[i] -= 1;

        if (fish[i] == -1) {
            fish[i] = 6;
            fish.Add(8);
            //Console.Write("[Add 8] ");
        }
    }

    //Console.WriteLine();
}

Console.WriteLine("{0}", fish.Count);


