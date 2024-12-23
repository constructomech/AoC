using System.Diagnostics;


string AllDirs = "><^vA";
string AllNums = "0123456789A";

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


List<List<char>> GetDirs(char from, char to) {
    if (from == to) return new List<List<char>> { new List<char>() };

    var (rfrom, cfrom) = PadPos(from);
    var (rto, cto) = PadPos(to);

    int dr = rto - rfrom;
    int dc = cto - cfrom;

    var first = new List<char>();
    if (dr != 0) {
        var s = dr < 0 ? '^' : 'v';
        first.AddRange(Enumerable.Repeat(s, Math.Abs(dr)));
    }
    if (dc != 0) {
        var s = dc < 0 ? '<' : '>';
        first.AddRange(Enumerable.Repeat(s, Math.Abs(dc)));
    }

    bool isDir = AllDirs.Contains(from) || AllDirs.Contains(to);
    bool isNum = AllNums.Contains(from) || AllNums.Contains(to);

    if (cto == 0 && dr == 1 && isDir || isNum && rfrom == 0 && cto == 0)
        return new List<List<char>> { first };

    var rev = new List<char>(first);
    rev.Reverse();

    if (cfrom == 0 && dr == -1 && isDir || isNum && cfrom == 0 && rto == 0 || dr != 0 && dc != 0)
        return new List<List<char>> { first, rev };

    return new List<List<char>> { first };
}

(int, int) PadPos(char x) => x switch {
    '<' => (1, 0),
    '^' => (0, 1),
    'A' => (0, 2),
    'v' => (1, 1),
    '>' => (1, 2),
    '0' => (0, 1),
    '1' => (-1, 0),
    '2' => (-1, 1),
    '3' => (-1, 2),
    '4' => (-2, 0),
    '5' => (-2, 1),
    '6' => (-2, 2),
    '7' => (-3, 0),
    '8' => (-3, 1),
    '9' => (-3, 2),
    _ => throw new InvalidOperationException("padPos")
};

Dictionary<string, long> GetTrivialPrices() {

    var prices = new Dictionary<string, long>();
    foreach (var from in AllDirs)
        foreach (var to in AllDirs)
            prices[string.Concat(from, to)] = 1;
    return prices;
}

long TotalPrice(string bs, Dictionary<string, long> prev) {
    var x = 'A';
    long total = 0;

    foreach (var b in bs) {
        total += prev[string.Concat(x, b)];
        x = b;
    }

    return total;
}

Dictionary<string, long> GetNextPrices(Dictionary<string, long> prev, string allChs) {
    var cur = new Dictionary<string, long>();

    foreach (var from in allChs) {
        foreach (var to in allChs) {
            long best = long.MaxValue;

            foreach (var bs in GetDirs(from, to)) {

                var pr = TotalPrice(new string([.. bs, 'A']), prev);
                best = Math.Min(best, pr);
            }

            cur[string.Concat(from, to)] = best;
        }
    }

    return cur;
}

int NumPrefix(string s) => int.Parse(s.Substring(0, 3));

long Complexity(string[] lines, int times) {
    var prices = GetTrivialPrices();

    for (int i = 0; i < times; i++)
        prices = GetNextPrices(prices, AllDirs);

    prices = GetNextPrices(prices, AllNums);

    return lines.Sum(x => NumPrefix(x) * TotalPrice(x, prices));
}

void Run(string[] input) {
//    var p1 = Complexity(input, 2);
    var p2 = Complexity(input, 25);

//    Console.WriteLine($"part 1 = {p1}");
    Console.WriteLine($"part 2 = {p2}");
}

// 198187405225570 too low
// 198187405225570 