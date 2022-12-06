using System.Text;

string template;
var insertionRules = new Dictionary<string, char>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    string line = reader.ReadLine();
    template = line;
    reader.ReadLine();

    while (!reader.EndOfStream) {
        line = reader.ReadLine();
        string[] parts = line.Split(" -> ");
        insertionRules.Add(parts[0], parts[1][0]);
    }
}

var histogram = new Dictionary<char, long>();
foreach (char c in template) {
    addTo(histogram, c);
}

var stack = new Stack<Tuple<string, int>>();
for (int i = 0; i < template.Length - 1; i++) {
    string pair = template.Substring(i, 2);
    stack.Push(new Tuple<string, int>(pair, 0));
}

while (stack.Count > 0) {
    (string pair, int depth) = stack.Pop();

    if (depth < 40) {
        char c;
        if (insertionRules.TryGetValue(pair, out c)) {
            addTo(histogram, c);

            string newPair1 = pair[0].ToString() + c;
            string newPair2 = c + pair[1].ToString();

            stack.Push(new Tuple<string, int>(newPair1, depth + 1));
            stack.Push(new Tuple<string, int>(newPair2, depth + 1));
        }
    }
}

printHistogram(histogram);


void printHistogram(Dictionary<char, long> histogram) {
    foreach ((char c, long count) in histogram) {
        Console.WriteLine("{0}: {1}", c, count);
    }
}

Dictionary<char, long> computeHistogram(string polimer) {
    var histogram = new Dictionary<char, long>();
    foreach (char c in polimer) {
        if (histogram.ContainsKey(c)) { 
            histogram[c]++;
        } else {
            histogram.Add(c, 1);
        }
    }
    return histogram;
}

void addTo(Dictionary<char, long> dest, char c) {
    if (dest.ContainsKey(c)) {
        dest[c] += 1;
    } else {
        dest.Add(c, 1);
    }
}
