using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

long Mix(long secret, long given) {
    return secret ^ given;
}

long Prune(long secret) {
    return secret % 16777216;
}

long NextSecret(long prevSecret) {
    var intermediate = prevSecret * 64;
    var secret = Mix(prevSecret, intermediate);
    secret = Prune(secret);

    intermediate = secret / 32;
    secret = Mix(secret, intermediate);
    secret = Prune(secret);

    intermediate = secret * 2048;
    secret = Mix(secret, intermediate);
    secret = Prune(secret);

    return secret;
}

Dictionary<(short a, short b, short c, short d), int> CountAllSequences(List<List<(short price, short change)>> priceChangeTable) {
    var result = new Dictionary<(short a, short b, short c, short d), int>();

    for (var i = 0; i < priceChangeTable.Count; i++) {
        var prices = priceChangeTable[i];

        var localResult = new Dictionary<(short a, short b, short c, short d), int>();

        for (var priceIdx = 3; priceIdx < prices.Count; priceIdx++) {

            var lastFourChanges = (prices[priceIdx - 3].change, prices[priceIdx - 2].change, prices[priceIdx - 1].change, prices[priceIdx].change);

            if (!localResult.ContainsKey(lastFourChanges)) {
                localResult[lastFourChanges] = prices[priceIdx].price;
            }
        }

        foreach (var localItem in localResult) {
            result.TryAdd(localItem.Key, 0);
            result[localItem.Key] += localItem.Value;
        }
    }

    return result;
}


void Run(string[] input) {
    var secrets = input.Select(s => long.Parse(s)).ToList();
    var priceChangeTable = new List<List<(short price, short change)>>();

    foreach (var secret in secrets) {
        priceChangeTable.Add(new());
        var priceChanges = priceChangeTable.Last();       

        var current = secret;
        short previousPrice = (short)(secret % 10);

        for (int i = 0; i < 2000; i++) {
            current = NextSecret(current);

            short price = (short)(current % 10);

            short change = (short)(price - previousPrice);
            priceChanges.Add((price, change));

            previousPrice = price;
        }
    }

    var sequences = CountAllSequences(priceChangeTable);

    var max = sequences.OrderBy(kvp => -kvp.Value).First();
    Console.WriteLine($"Result: {max.Value} ({max.Key.a}, {max.Key.b}, {max.Key.c}, {max.Key.d})");
}
