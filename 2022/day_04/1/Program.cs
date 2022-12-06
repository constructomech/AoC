using System.IO;
using System.Collections.Generic;


int total = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            var ranges = line.Split(',');

            var lhsParts = ranges[0].Split('-');
            Range lhs = new Range { Lower = Convert.ToInt32(lhsParts[0]), Upper = Convert.ToInt32(lhsParts[1]) };
            var rhsParts = ranges[1].Split('-');
            Range rhs = new Range { Lower = Convert.ToInt32(rhsParts[0]), Upper = Convert.ToInt32(rhsParts[1]) };

            if (Fun.Contains(lhs, rhs) || Fun.Contains(rhs, lhs)) {
                total++;
            }
            // Implement
        }
    }
}

// See https://aka.ms/new-console-template for more information
Console.WriteLine("{0}", total);

public class Range {
    public int Lower { get; set; }
    public int Upper { get; set; }
}

public class Fun {
    public static bool Contains(Range lhs, Range rhs) {
        return (lhs.Lower <= rhs.Lower && lhs.Upper >= rhs.Upper);
    }
}