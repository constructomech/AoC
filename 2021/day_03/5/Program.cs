// See https://aka.ms/new-console-template for more information

using System.Collections.Generic;
using System.IO;

using (StreamReader reader = File.OpenText("input.txt")) {

    List<int> list = new List<int>();
    int lines = 0;

    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {

            while (list.Count < line.Length) {
                list.Add(0);
            }

            for (int i = 0; i < line.Length; i++) {
                if (line[i] == '1') {
                    list[i]++;
                }
            }

            lines++;
        }
    }

    string gammaRate = "", episonRate = "";

    foreach (var item in list) {

        // 1's are more common
        if (item > (lines / 2)) {
            gammaRate += "1";
            episonRate += "0";
        }
        else {
            gammaRate += "0";
            episonRate += "1";
        }
    }

    int gamma = Convert.ToInt32(gammaRate, 2);
    int epison = Convert.ToInt32(episonRate, 2);

    Console.WriteLine("{0} * {1} = {2}", gammaRate, episonRate, gamma * epison);

}

