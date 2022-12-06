using System.IO;
using System.Collections.Generic;

List<long> elfCalories = new List<long>();
long max = 0;
int elfIndex = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    long accum = 0;

    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            if (line.Length == 0) {
               elfCalories.Add(accum);
               if (accum > max) { 
                    max = accum; 
                    elfIndex = elfCalories.Count;
                }
                accum = 0;
            } 
            else
            {
                accum += Convert.ToInt64(line);
            }
        }
    }
}

elfCalories.Sort();
long total = 0;

for (int i = 0; i < 3; i++) 
{
    total += elfCalories[ elfCalories.Count - i - 1];
}

for (int i = 0; i < elfCalories.Count; i++) 
{
    Console.WriteLine("{0}: {1}", i, elfCalories[i]);
}

Console.WriteLine("Max: {0}", total);
