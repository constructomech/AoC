using System.IO;
using System.Collections.Generic;

long result = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            // Implement
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}