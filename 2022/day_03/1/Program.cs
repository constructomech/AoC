using System.IO;
using System.Collections.Generic;


long result = 0;

using (StreamReader reader = File.OpenText("input.txt")){
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            string compartmentA = line.Substring(0, line.Length / 2);
            string compartmentB = line.Substring(line.Length / 2);

            foreach (char item in compartmentA) {
                if (compartmentB.Contains(item)) {
                    long score = Fun.Score(item);
                    Console.WriteLine("{0}: {1}", item, score);
                    result += score;
                    break;
                }
            }
            // Implement
        }
    }
}

Console.WriteLine("Result: {0}", result);


public static class Fun {
    public static long Score(char item) {
        long  score = (long)item - 'a' + 1;
        if (score <= 0) {
            score = (long)item - 'A' + 27;
        }
        return score;
    }
}