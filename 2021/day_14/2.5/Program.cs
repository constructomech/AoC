using System.Text;

string template;
byte[] insertionRules = new byte[short.MaxValue];
using (StreamReader reader = File.OpenText("input.txt"))
{
    string line = reader.ReadLine();
    template = line;
    reader.ReadLine();

    while (!reader.EndOfStream) {
        line = reader.ReadLine();
        string[] parts = line.Split(" -> ");
        char c1 = parts[0][0];
        char c2 = parts[0][1];
        char o = parts[1][0];
        int idx = ((byte)(c1 - 'A') << 8) + (byte)(c2 -'A');
        insertionRules[idx] = (byte)o;
    }
}

var histogram = new long[byte.MaxValue];
foreach (char c in template) {
    addTo(histogram, c);
}

var stack = new Stack<StackItem>();
for (int i = 0; i < template.Length - 1; i++) {
    string pair = template.Substring(i, 2);
    stack.Push(new StackItem() { c1 = (byte)pair[0], c2 = (byte)pair[1], depth = 0});
}

while (stack.Count > 0) {
    StackItem item = stack.Pop();

    if (item.depth < 40) {
        int idx = ((byte)(item.c1 - 'A') << 8) + (byte)(item.c2 -'A');
        byte insertionResult = insertionRules[idx];
        if (insertionResult != 0) {
            char c = (char)insertionResult;
            addTo(histogram, c);

            stack.Push(new StackItem() { c1 = item.c1, c2 = insertionResult, depth = (byte)(item.depth + 1) });
            stack.Push(new StackItem() { c1 = insertionResult, c2 = item.c2, depth = (byte)(item.depth + 1) });
        }
    }
}

printHistogram(histogram);

void printHistogram(long[] histogram) {
    for (int i = 0; i < byte.MaxValue; i++) {
        long result = histogram[i];
        if (histogram[i] != 0) {
            Console.WriteLine("{0}: {1}", (char)i, result);
        }
    }
}

void addTo(long[] histogram, char c) {
    histogram[(byte)c]++;
}

struct StackItem {
    public byte c1;
    public byte c2;
    public byte depth;
}