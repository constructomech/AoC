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

int ComboOperand(List<int> registers, int operand) {
    switch (operand) {
        case 0:
        case 1:
        case 2:
        case 3:
            return operand;
        case 4:
            return registers[REG_A];
        case 5:
            return registers[REG_B];
        case 6:
            return registers[REG_C];
    }
    throw new InvalidOperationException();
}

void Run(string input) {

    // Parse

    string registersPattern = @"Register \w+: (\d+)";
    var registers = Regex.Matches(input, registersPattern)
                        .Select(m => int.Parse(m.Groups[1].Value))
                        .ToList();

    string programPattern = @"Program: ([\d,]+)";
    var program = Regex.Match(input, programPattern)
                    .Groups[1].Value
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

    // Simulate

    Console.WriteLine($"Program: {string.Join(",", program)}");

    bool firstOutput = true;

    var ip = 0;
    var loopCount = 0;
    var instructionCount = 0;
    var outputCount = 0;

    while (ip < program.Count) {

        var opcode = program[ip];
        var operand = program[ip + 1];

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
                    ip = operand - 2;
                    loopCount++;
                }
                break;
            case 4: // bxc
                var bxcResult = registers[REG_B] ^ registers[REG_C];
                registers[REG_B] = bxcResult;
                break;
            case 5: // out
                var outResult = ComboOperand(registers, operand) % 8;
                Console.Write("{0}{1}", firstOutput ? "" : ",", outResult);
                firstOutput = false;
                outputCount++;
                break;
            case 6: // bdv
                var bdvResult = registers[REG_A] / (int)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_B] = bdvResult;
                break;
            case 7: // cdv
                var cdvResult = registers[REG_A] / (int)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_C] = cdvResult;
                break;
        }
        ip +=2;
        instructionCount++;
    }

    Console.WriteLine($"{instructionCount} instructions, {loopCount} loops, {outputCount} outputs");
}
