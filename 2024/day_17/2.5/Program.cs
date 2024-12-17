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

long ComboOperand(List<long> registers, int operand) {
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

// Repeat:  bst REG_A       # REG_A % 8 -> REG_B
//          bxl 7           # REG_B % 7 -> REG_B
//          cdv REG_B       # REG_A / 2**REG_B -> REG_C
//          adv 3           # REG_A / 2**3 -> REG_A
//          bxl 7           # REG_B ^ 7 -> REG_B
//          bxc             # REG_B ^ REG_C -> REG_B
//          out REG_B       # output REG_B % 8
//          jnz 0           # if REG_A != 0 goto :Repeat
//
bool RunCompiledProgramExpecting(long start, List<int> expectedOutput) {   
    var a = start;
    var op = 0;

    while (a != 0) {
        var b = a % 8;
        b = b ^ 7;
        var c = a / (long)Math.Pow(2, b);
        a = a / 8;
        b = b ^ 7;
        b = c ^ b;
        var o = b % 8;
        if (o != expectedOutput[op]) {
            return false;
        }
        op++;
    }

    return op == expectedOutput.Count;
}

void Run(string input) {

    // Parse

    string registersPattern = @"Register \w+: (\d+)";
    var registers = Regex.Matches(input, registersPattern)
                        .Select(m => long.Parse(m.Groups[1].Value))
                        .ToList();

    // Regex to match the program field
    string programPattern = @"Program: ([\d,]+)";
    var program = Regex.Match(input, programPattern)
                    .Groups[1].Value
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

    // Simulate

    Parallel.For(123456701234567, 7422630647200635, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
    {
        var runResult = RunCompiledProgramExpecting(i, program);
        if (runResult) {

            Console.WriteLine();
            Console.WriteLine($"Found: {i}");
            // Break out of the loop for all threads (note: Parallel.For doesn't allow a true "break")
            Environment.Exit(0); // Or another thread-safe way to exit the parallel work
        }
    });
}
