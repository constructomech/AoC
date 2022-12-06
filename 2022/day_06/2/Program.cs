using System.IO;
using System.Collections.Generic;

// To fix the communication system, you need to add a subroutine to the device that detects a start-of-packet marker in the datastream.

long result = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            int pos = 0;
            while (pos <= line.Length - 14) {
                if (Fun.HasNUnique(line, pos, 14)) {
                    Console.WriteLine("{0}{1}{2}{3}", line[pos], line[pos+1], line[pos+2], line[pos+3]);
                    result = pos + 14;
                    break;
                }
                pos++;
            }
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static bool HasNUnique(string input, int pos, int count) {
        var uniqueValues = new HashSet<char>();
        for (int i = pos; i < pos + count; i++) {
            if (!uniqueValues.Add(input[i])) {
                return false;
            }
        }

        return true;
    }
}