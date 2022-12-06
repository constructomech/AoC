using System.Text;

string template;
byte[] insertionRules = new byte[(int)Math.Pow(2, 16)];
var histogramCache = new Dictionary<int, long[]>();

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

var histogram = new long[(int)Math.Pow(2, 8)];
foreach (char c in template) {
    addTo(histogram, c);
}

int iteration = 0;

for (int i = 0; i < template.Length - 1; i++) {
    string pair = template.Substring(i, 2);
    byte c1 = (byte)pair[0];
    byte c2 = (byte)pair[1];
    computeHistogram(histogram, c1, c2, 0);
}

printHistogram(histogram);


void computeHistogram(long[] histogram, byte c1, byte c2, byte depth) {
    if (++iteration % 10000 == 0) {
        Console.WriteLine("Iteration {0}", iteration);
    }
    if (depth < 40) {
        var result = getCachedHistogram(c1, c2, depth);
        if (result == null) {
            result = new long[256];

            int idx = ((byte)(c1 - 'A') << 8) + (byte)(c2 -'A');
            byte insertionResult = insertionRules[idx];
            if (insertionResult != 0) {
                char c = (char)insertionResult;
                addTo(result, c);

                int nextDepth = depth + 1;
                computeHistogram(result, c1, insertionResult, (byte)(depth + 1));
                computeHistogram(result, insertionResult, c2, (byte)(depth + 1));
            }

            addHistogramToCache(c1, c2, depth, result);
        }
        addInto(histogram, result);
    }
}

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

void addInto(long[] dest, long[] source) {
    for (int i = 0; i < byte.MaxValue; i++) {
        dest[i] += source[i];
    }
}

long[] getCachedHistogram(byte c1, byte c2, byte depth) {
    int key = (depth << 16) + (c1 << 8) + c2;

    long[] histogram = null;
    histogramCache.TryGetValue(key, out histogram);

    return histogram;
}

void addHistogramToCache(byte c1, byte c2, byte depth, long[] histogram) {
    var copy = new long[(int)Math.Pow(2, 8)];
    Array.Copy(histogram, copy, (int)Math.Pow(2, 8));
    histogramCache.Add((depth << 16) + (c1 << 8) + c2, histogram);
}