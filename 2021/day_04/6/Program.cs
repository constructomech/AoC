// See https://aka.ms/new-console-template for more information

using System.Collections.Generic;
using System.IO;

List<string> readInput()
{
    List<string> result = new List<string>();
    using (StreamReader reader = File.OpenText("input.txt"))
    {
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (line != null)
            {
                result.Add(line);
            }
        }
    }
    return result;
}

char mostCommonValue(List<string> list, int position) {
    int count = 0;

    foreach (string item in list) {
        if (item[position] == '1') {
            count++;
        }
    }

    return (count >= (list.Count / 2.0)) ? '1' : '0';
}

List<string> filterList(List<string> original, char value, int position) {
    List<string> result = new List<string>();
    foreach (string item in original) {
        if (item[position] == value) {
            result.Add(item);
        }
    }
    return result;
}

List<string> input = readInput();

List<string> oxygenGenList = new List<string>(input);
int position = 0;
while (oxygenGenList.Count > 1) {
    char mostCommon = mostCommonValue(oxygenGenList, position);
    
    oxygenGenList = filterList(oxygenGenList, mostCommon, position);

    position++;
}

List<string> co2ScrubberList = new List<string>(input);
position = 0;
while (co2ScrubberList.Count > 1) {
    char leastCommon = (mostCommonValue(co2ScrubberList, position) == '1' ? '0' : '1');
    
    co2ScrubberList = filterList(co2ScrubberList, leastCommon, position);

    position++;
}

int oxygen = Convert.ToInt32(oxygenGenList[0], 2);
int co2 = Convert.ToInt32(co2ScrubberList[0], 2);

Console.WriteLine("{0} * {1} = {2}", oxygenGenList[0], co2ScrubberList[0], oxygen * co2);