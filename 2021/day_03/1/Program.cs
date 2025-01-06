
using (StreamReader reader = File.OpenText("input.txt")) {

    var bitsSetToOneCount = new List<int>();
    int lines = 0;

    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {
            var data = Convert.ToInt32(line, 2);

            while (bitsSetToOneCount.Count < line.Length) {
                bitsSetToOneCount.Add(0);
            }

            for (int i = 0; i < line.Length; i++) {
                if ((data & (1 << i)) != 0) {
                    bitsSetToOneCount[line.Length - i - 1]++;
                }
            }

            lines++;
        }
    }

    int gamma = 0;
    int epison = 0;

    for (var i = 0; i < bitsSetToOneCount.Count; i++) {

        var item = bitsSetToOneCount[i];

        // 1's are more common
        if (item > (lines / 2)) {
            gamma |= (1 << (bitsSetToOneCount.Count - i - 1));
        }
        else {
            epison |= (1 << (bitsSetToOneCount.Count - i - 1));
        }
    }    

    Console.WriteLine($"Result: {gamma * epison}");

}

