
int FindValue(IEnumerable<int> inConsideration, int totoalBits, Comparatively preference) {
    var position = totoalBits - 1;
    while (inConsideration.Count() > 1) {
        var testBit = 1 << position;
        var ones = LinesMatching(inConsideration, testBit, position);
        var zeros = inConsideration.Except(ones);

        if (ones.Count() >= zeros.Count()) {
            inConsideration = preference == Comparatively.Greater ? ones : zeros;
        }
        else {
            inConsideration = preference == Comparatively.Greater ? zeros : ones;
        }

        position--;
    }
    return inConsideration.First();
}

IEnumerable<int> LinesMatching(IEnumerable<int> inConsideration, int testBit, int position) {
    foreach (var item in inConsideration) {
        if ((item & (1 << position)) == testBit) {
            yield return item;
        }
    }
}

using (StreamReader reader = File.OpenText("input.txt")) {

    var originalData = new List<int>();
    var totalBits = 0;

    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {
            var data = Convert.ToInt32(line, 2);
            originalData.Add(data);

            if (totalBits < line.Length) {
                totalBits = line.Length;
            }
        }
    }

    var oxygenGenRating = FindValue(originalData, totalBits, Comparatively.Greater);
    var co2ScrubberRating = FindValue(originalData, totalBits, Comparatively.Fewer);

    Console.WriteLine($"Result: {oxygenGenRating * co2ScrubberRating}");
}

enum Comparatively { Fewer, Greater, Equal }
