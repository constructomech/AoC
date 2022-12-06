
List<Pair> data = new List<Pair>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            string[] halves = line.Split('|');
            string[] input = halves[0].Trim().Split(' ');
            string[] output = halves[1].Trim().Split(' ');

            List<string> newInput = new List<string>();
            foreach (string item in input) {
                var arr = item.ToArray();
                Array.Sort(arr);
                newInput.Add(new string(arr));
            }

            List<string> newOutput = new List<string>();
            foreach (string item in output) {
                var arr = item.ToArray();
                Array.Sort(arr);
                newOutput.Add(new string(arr));
            }
            Pair pair = new Pair(newInput, newOutput);
            data.Add(pair);            
        }
    }
}

int sum = 0;
foreach (Pair pair in data) {

    string signalForZero = "";
    string signalForOne = "";
    string signalForTwo = "";
    string signalForThree = "";
    string signalForFour = "";
    string signalForFive = "";
    string signalForSix = "";
    string signalForSeven = "";
    string signalForEight = "";
    string signalForNine = "";


    // Set up base cases, 1, 4, 7, 8
    foreach (Digit input in pair.input) {
        switch (input.signals.Length) {
            case 2:
                input.value = DigitValue.One;
                signalForOne = input.signals;
                break;
            case 3:
                input.value = DigitValue.Seven;
                signalForSeven = input.signals;
                break;
            case 4:
                input.value = DigitValue.Four;
                signalForFour = input.signals;
                break;
            case 7:
                input.value = DigitValue.Eight;
                signalForEight = input.signals;
                break;
        }
    }

    char v = '\0';
    // Identify 6
    foreach (Digit input in pair.input) {
        if (input.signals.Length == 6 && isMissingOneIn(input.signals, signalForOne)) {
            input.value = DigitValue.Six;
            signalForSix = input.signals;

            string temp = inAandNotB(signalForOne, signalForSix);
            v = temp[0];
            break;
        }
    }

    char w = '\0';
    // Identify 5
    foreach (Digit input in pair.input) {
        if (input.signals.Length == 5 && !input.signals.Contains(v)) {
            input.value = DigitValue.Five;
            signalForFive = input.signals;

            string temp = inAandNotB(signalForEight, signalForFive);
            temp = temp.Replace(v.ToString(), "");
            w = temp[0];
            break;
        }
    }

    // Identify 2 & 3
    foreach (Digit input in pair.input) {
        if (input.signals.Length == 5 && input.value == DigitValue.Unknown) {
            if (input.signals.Contains(w)) {
                input.value = DigitValue.Two;
                signalForTwo = input.signals;
            }
            else {
                input.value = DigitValue.Three;
                signalForThree = input.signals;
            }
        }
    }

    // Identify 0 & 9
    foreach (Digit input in pair.input) {
        if (input.value == DigitValue.Unknown) {
            if (input.signals.Contains(w)) {
                input.value = DigitValue.Zero;
                signalForZero = input.signals;
            }
            else {
                input.value = DigitValue.Nine;
                signalForNine = input.signals;
            }
        }
    }

    string outputValue = "";

    // Populate output digits
    foreach (Digit output in pair.output) {
        foreach (Digit input in pair.input) {
            if (output.signals == input.signals) {
                output.value = input.value;
                outputValue += ((int)output.value).ToString();
                break;
            }
        }
    }

    int outputNum = Convert.ToInt32(outputValue);
    sum += outputNum;
}

Console.WriteLine("Sum: {0}", sum);


bool isMissingOneIn(string input, string other) {
    int foundCount = 0;
    foreach (char c in other) {
        if (input.Contains(c)) {
            foundCount++;
        }
    }
    return (foundCount == other.Length - 1);
}

string inAandNotB(string a, string b) {
    string temp = string.Copy(a);
    foreach (char c in b) {
        temp = temp.Replace(c.ToString(), "");
    }
    return temp;
}

enum DigitValue {
    Zero = 0,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Unknown,
}

class Digit {
    public string signals;
    public DigitValue value = DigitValue.Unknown;
}
class Pair {

    public Pair(IEnumerable<string> inputVal, IEnumerable<string> outputVal) {
        this.input = new List<Digit>();
        foreach (var item in inputVal) {
            this.input.Add(new Digit() { signals = item });
        }

        this.output = new List<Digit>();
        foreach (var item in outputVal) {
            this.output.Add(new Digit() { signals = item });
        }
    }
    public List<Digit> input;
    public List<Digit> output;
}