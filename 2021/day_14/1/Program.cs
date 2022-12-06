using System.Text;

string polimer;
var insertionRules = new Dictionary<string, char>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    string line = reader.ReadLine();
    polimer = line;
    reader.ReadLine();

    while (!reader.EndOfStream) {
        line = reader.ReadLine();
        string[] parts = line.Split(" -> ");
        insertionRules.Add(parts[0], parts[1][0]);
    }
}

for (int step = 1; step <= 10; step++) {
    StringBuilder nextPolimer = new StringBuilder();
    for (int i = 0; i < polimer.Length - 1; i++) {
        nextPolimer.Append(polimer[i]);

        string lookup = polimer.Substring(i, 2);
        char c;
        if (insertionRules.TryGetValue(lookup, out c)) {
            nextPolimer.Append(c);
        }
    }
    nextPolimer.Append(polimer.Last());
    polimer = nextPolimer.ToString();
    Console.WriteLine("Step {0}:", step);

    var histogram = new Dictionary<char, int>();
    foreach (char c in polimer) {
        if (histogram.ContainsKey(c)) { 
            histogram[c]++;
        } else {
            histogram.Add(c, 1);
        }
    }

    foreach ((char c, int count) in histogram) {
        Console.WriteLine("{0}: {1} ({2}%)", c, count, (double)count / polimer.Length * 100);
    }
    Console.WriteLine();
}

