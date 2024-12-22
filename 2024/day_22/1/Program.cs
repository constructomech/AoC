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

void Run(string[] input) {
    var result = 0L;

    var secrets = input.Select(s => long.Parse(s)).ToList();

    foreach (var secret in secrets) {
        var current = secret;
        for (int i = 0; i < 2000; i++) {
            current = NextSecret(current);
        }

        result += current;
        Console.WriteLine(current);
    }    

    Console.WriteLine($"Result: {result}");
}
