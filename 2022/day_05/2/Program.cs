using System.IO;
using System.Collections.Generic;

var stacks = new List<List<char>>();

long result = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    
    string? line;
    bool cont = false;
    do {
        line = reader.ReadLine();
        cont = line.Contains('[');
        if (cont) {
            int boxNum = 0;
            int pos = 0;
            while (line.Length - pos >= 3) {
                var item = line.Substring(pos, 3);

                if (stacks.Count <= boxNum) { 
                    stacks.Add(new List<char>());
                }

                char box = item[1];
                if (box != ' ') {
                    stacks[boxNum].Add(box);
                }

                pos += 4;
                boxNum++;
            }
        }
    } while (cont);

    reader.ReadLine();
        
    while (!reader.EndOfStream)
    {
        line = reader.ReadLine();
        if (line != null) {
            // "move 7 from 6 to 8"
            var parts = line.Split(' ');
            int quantity = Convert.ToInt32(parts[1]);
            int from = Convert.ToInt32(parts[3]) - 1;
            int to = Convert.ToInt32(parts[5]) - 1;

            for (int i = quantity - 1; i >= 0; i--) {
                var temp = stacks[from][i];
                stacks[from].RemoveAt(i);
                stacks[to].Insert(0, temp);
            }
        }                
    }
}

foreach (var stack in stacks) {
    Console.Write("{0}", stack[0]);
}


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}