List<Instruction> program = new List<Instruction>();
List<int> inputInstructions = new List<int>();
var cache = new Dictionary<(int ip, int inp, int w, int x, int y, int z), (int ip, int w, int x, int y, int z)>();


using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {            
            program.Add(Instruction.Parse(line));
        }
    }
}

for (int i = 0; i < program.Count; i++) {
    if (program[i].Operation == Op.Inp) {
        inputInstructions.Add(i);
    }
}

int w, x, y, z;

for (int i = 1; i <= 9; i++) {

    var results = Test(51983999947999);

    long? commonResult = null;
    foreach(var result in results.Values) {
        if (commonResult == null) {
            commonResult = result;
        } else {
            if (commonResult != result) {
                commonResult = null;
                Console.WriteLine("FAIL");
                break;
            }
        }
    }
    if (commonResult != null) {
        Console.WriteLine("PASS");
    }
}

IEnumerable<Queue<int>> GenerateTestData(int commonPlace, int varyDigit, int varyMin, int varyMax) {

    for (int j = varyMin; j <= varyMax; j++) {
        var result = new Queue<int>();

        for (int i = 0; i < 14; i++) {
            if (i == varyDigit) {
                result.Enqueue(j);
            } else {
                result.Enqueue(commonPlace);
            }
        }

        yield return result;
    }
}

//Dictionary<long, long> Test(IEnumerable<Queue<int>> generator) {
Dictionary<long, long> Test(long value) {
    Queue<int> inputQueue = new Queue<int>();
    foreach (char c in value.ToString()) {
        int num = c - '0';
        inputQueue.Enqueue(num);
    }

    var outputs = new Dictionary<long, long>();

    // foreach (var inputQueue in generator) {

        w = x = y = z = 0;

        string inputString = "";
        long input = 0;

        bool invalidInput = false;
        foreach (var inputValue in inputQueue) {
            if (inputValue == 0) {
                invalidInput = true;
                break;
            }
            inputString += inputValue.ToString();
        }
        input = Convert.ToInt64(inputString);

        if (!invalidInput) { 

            //(int ip, int inp, int w, int x, int y, int z) priorMachineState = (-1, 0, w, x, y, z);

            for (int i = 0; i < program.Count; i++) {

                var instruction = program[i];

                int a, b;
                switch (instruction.Operation) {
                    case Op.Inp:
                        // Cache the results of the last sequence up to an input
                        // if (priorMachineState.ip != -1) {
                        //     if (!cache.ContainsKey(priorMachineState)) {
                        //         cache.Add(priorMachineState, (i, w, x, y, z));
                        //     }
                        // }
                        int inp = inputQueue.Dequeue();
                        // priorMachineState = (i, inp, w, x, y, z);

                        // (int ip, int w, int x, int y, int z) nextMachineState;
                        // if (cache.TryGetValue(priorMachineState, out nextMachineState)) {
                        //     w = nextMachineState.w;
                        //     x = nextMachineState.x;
                        //     y = nextMachineState.y;
                        //     z = nextMachineState.z;
                        //     i = nextMachineState.ip - 1; // Loop will increment
                        //     Console.WriteLine("Input {0}: w={1}, x={2}, y={3}, z={4} (cached)", 14 - inputQueue.Count, w, x, y, z);
                        // }
                        // else {
                            SetRegister(instruction.Operand1, inp);
                            //Console.WriteLine("Input {0}: w={1}, x={2}, y={3}, z={4}", 14 - inputQueue.Count, w, x, y, z);
                        // }
                        break;
                    case Op.Add:
                        a = ReadValue(instruction.Operand1);
                        b = ReadValue(instruction.Operand2);
                        SetRegister(instruction.Operand1, a + b);
                        break;
                    case Op.Mul:
                        a = ReadValue(instruction.Operand1);
                        b = ReadValue(instruction.Operand2);
                        SetRegister(instruction.Operand1, a * b);
                        break;
                    case Op.Div:
                        a = ReadValue(instruction.Operand1);
                        b = ReadValue(instruction.Operand2);
                        SetRegister(instruction.Operand1, a / b);
                        break;
                    case Op.Mod:
                        a = ReadValue(instruction.Operand1);
                        b = ReadValue(instruction.Operand2);
                        SetRegister(instruction.Operand1, a % b);
                        break;
                    case Op.Eql:       
                        a = ReadValue(instruction.Operand1);
                        b = ReadValue(instruction.Operand2);
                        int outputVal = (a == b) ? 1 : 0;
                        SetRegister(instruction.Operand1, outputVal);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
    //            Console.WriteLine("   {0}: w={1}, x={2}, y={3}, z={4}", i, w, x, y, z);
            }

            Console.WriteLine("Run complete: w={0}, x={1}, y={2}, z={3}", w, x, y, z);

            // if (z == 0) { // Valid
            //     Console.WriteLine("Found valid serial number: {0}", input);
            //     break;
            // }

            outputs.Add(input, z);
        }
    // }
    return outputs;
}

Console.WriteLine("EOL");

int ReadValue(string name) {
    switch (name) {
        case "w": return w;
        case "x": return x;
        case "y": return y;
        case "z": return z;
    }
    return Convert.ToInt32(name);
}

void SetRegister(string name, int value) {
    switch (name) {
        case "w": w = value; break;
        case "x": x = value; break;
        case "y": y = value; break;
        case "z": z = value; break;
        default:
            throw new InvalidDataException();
    }
}

enum Op {
    Inp,
    Add,
    Mul,
    Div,
    Mod,
    Eql,
}

class Instruction {

    public static Instruction Parse(string line) {
        Instruction result = new Instruction();
        var parts = line.Split(' ');
        result.Operation = OperationFromString(parts[0]);

        result.Operand1 = parts[1];

        if (result.Operation != Op.Inp) {
            result.Operand2 = parts[2];
        }
        return result;
    }
    public Op Operation { get; set; }

    public string Operand1 { get; set; }

    public string Operand2 { get; set; }

    private static Op OperationFromString(string operation) {
        switch (operation) {
            case "inp": return Op.Inp;
            case "add": return Op.Add;
            case "mul": return Op.Mul;
            case "div": return Op.Div;
            case "mod": return Op.Mod;
            case "eql": return Op.Eql;
        }
        throw new InvalidDataException();
    }
}

