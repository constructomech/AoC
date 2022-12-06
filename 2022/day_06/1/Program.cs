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
            while (pos <= line.Length - 4) {
                if (Fun.Has4Unique(line, pos)) {
                    Console.WriteLine("{0}{1}{2}{3}", line[pos], line[pos+1], line[pos+2], line[pos+3]);
                    result = pos + 4;
                    break;
                }
                pos++;
            }
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static bool Has4Unique(string input, int pos) {
        char char1 = input[pos];
        char char2 = input[pos + 1];
        char char3 = input[pos + 2];
        char char4 = input[pos + 3];

        return (char1 != char2 && char1 != char3 && char1 != char4 && char2 != char3 && char2 != char4 && char3 != char4);
    }
}