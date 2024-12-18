using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


const int REG_A = 0;
const int REG_B = 1;
const int REG_C = 2;
const int REG_IP = 3;

long ComboOperand(List<long> registers, int operand) {
    switch (operand) {
        case 0:
        case 1:
        case 2:
        case 3: return operand;
        case 4: return registers[REG_A];
        case 5: return registers[REG_B];
        case 6: return registers[REG_C];
    }
    throw new InvalidOperationException();
}

// Repeat:  bst REG_A       # REG_A % 8 -> REG_B
//          bxl 7           # REG_B % 7 -> REG_B
//          cdv REG_B       # REG_A / 2**REG_B -> REG_C
//          adv 3           # REG_A / 2**3 -> REG_A
//          bxl 7           # REG_B ^ 7 -> REG_B
//          bxc             # REG_B ^ REG_C -> REG_B
//          out REG_B       # output REG_B % 8
//          jnz 0           # if REG_A != 0 goto :Repeat
//
// bool RunCompiledProgramExpecting(long start, List<int> expectedOutput) {   
//     var a = start;
//     var op = 0;

//     while (a != 0) {
//         var b = a % 8;
//         b = b ^ 7;
//         var c = a / (long)Math.Pow(2, b);
//         a = a / 8;
//         b = b ^ 7;
//         b = c ^ b;
//         var o = b % 8;
//         if (o != expectedOutput[op]) {
//             return false;
//         }
//         op++;
//     }

//     return op == expectedOutput.Count;
// }

int? RunProgram(List<long> registers, List<int> program) {
    while (registers[REG_IP] < program.Count) {

        var opcode = program[(int)registers[REG_IP]];
        var operand = program[(int)registers[REG_IP] + 1];

        switch(opcode) {
            case 0: // adv
                var advResult = registers[REG_A] / (int)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_A] = advResult;
                break;
            case 1: // bxl
                var bxlResult = registers[REG_B] ^ operand;
                registers[REG_B] = bxlResult;
                break;
            case 2: // bst
                var bstResult = ComboOperand(registers, operand) % 8;
                registers[REG_B] = bstResult;
                break;
            case 3: // jnz
                if (registers[REG_A] != 0) {
                    registers[REG_IP] = operand;
                    continue;
                }
                break;
            case 4: // bxc
                var bxcResult = registers[REG_B] ^ registers[REG_C];
                registers[REG_B] = bxcResult;
                break;
            case 5: // out
                var outResult = ComboOperand(registers, operand) % 8;
                registers[REG_IP] +=2;
                return (int)outResult;
            case 6: // bdv
                var bdvResult = registers[REG_A] / (int)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_B] = bdvResult;
                break;
            case 7: // cdv
                var cdvResult = registers[REG_A] / (int)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_C] = cdvResult;
                break;
        }
        registers[REG_IP] +=2;
    }
    return null;
}

void PrintRun(long a, List<int> program) {
    int? result = null;

    List<long> registers = new List<long>() { a, 0, 0, 0 };

    while(true) {

        result = RunProgram(registers, program);
        if (result == null) break;

        Console.Write($"{result},");
    }
    Console.WriteLine();
}

bool Validate(List<long> registers, List<int> program) {
    int i = 0;
    int? result = null;
    do {
        result = RunProgram(registers, program);
        Console.WriteLine(result);
        if (result == null) {
            break;
        } else if (result != program[i]) {
            return false;
        }
        i++;
    } while (result != null);

    return i == program.Count;
}

long? FindProgram(List<long> registers, List<int> program, int index, long a) {
    if (index == -1) {
        return a;
    }

    for (uint i = 0; i < 8; i++) {
        // Each test run needs a copy of the register set.
        var runRegisters = registers.ToList();

        var candidate = (a << 3) | i;
        runRegisters[REG_IP] = 0;
        runRegisters[REG_A] = candidate;
        runRegisters[REG_B] = 0;
        runRegisters[REG_C] = 0;

        var output = RunProgram(runRegisters, program);
        if (output == program[index]) {

            Console.WriteLine($"[{index}] Possible value: {candidate}");
            PrintRun(a, program);
            Console.WriteLine();

            // This value of a produces the right output, recursively explore the next index
            var result = FindProgram(registers, program, index - 1, candidate);
            if (result.HasValue) return result;
        }
    }
    return null;
}

void Run(string input) {

    // Parse

    string registersPattern = @"Register \w+: (\d+)";
    var registers = Regex.Matches(input, registersPattern)
                        .Select(m => long.Parse(m.Groups[1].Value))
                        .ToList();
    registers.Add(0); // Add the instruction pointer register

    // Regex to match the program field
    string programPattern = @"Program: ([\d,]+)";
    var program = Regex.Match(input, programPattern)
                    .Groups[1].Value
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();


    var runRegisters = registers.ToList();
    var winner = FindProgram(runRegisters, program, program.Count - 1, 0);

    runRegisters[REG_IP] = 0;
    runRegisters[REG_A] = winner.Value;
    runRegisters[REG_B] = 0;
    runRegisters[REG_C] = 0;

    bool result = Validate(runRegisters, program);
    Console.WriteLine($"Validation: {result}");
}
