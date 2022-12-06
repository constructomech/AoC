using System.IO;
using System.Collections.Generic;


long result = 0;

using (StreamReader reader = File.OpenText("input.txt")){
    while (!reader.EndOfStream)
    {
        string? line1 = reader.ReadLine();
        string? line2 = reader.ReadLine();
        string? line3 = reader.ReadLine();

        char inCommon = Fun.hasInCommon(line1, line2, line3);

        long score = Fun.Score(inCommon);
        result += score;
    }
}

Console.WriteLine("Result: {0}", result);


public static class Fun {

    public static char hasInCommon(string s1, string s2, string s3) {
        foreach (char c1 in s1) {
            if (s2.Contains(c1) && s3.Contains(c1)) {
                return c1;
            }
        }
        throw new InvalidOperationException();
    }

    public static long Score(char item) {
        long  score = (long)item - 'a' + 1;
        if (score <= 0) {
            score = (long)item - 'A' + 27;
        }
        return score;
    }
}