using System.Diagnostics;
using System.Text.RegularExpressions;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

// Starting value of A is TBD
// B & C = 0
//
// Repeat:  bst REG_A       # REG_A % 8 -> REG_B                start % 8 = a               REG_A = start,  REG_B = a,  REG_C = 0
//          bxl 7           # REG_B % 7 -> REG_B                b = a + 7 - 2*(a % 8)       REG_A = start,  REG_B = b,  REG_C = 0
//          cdv REG_B       # REG_A / 2**REG_B -> REG_C         c = start / 2**b            REG_A = start,  REG_B = b,  REG_C = c
//          adv 3           # REG_A / 2**3 -> REG_A             d = start / 8               REG_A = d       REG_B = b,  REG_C = c
//          bxl 7           # REG_B ^ 7 -> REG_B                e = b + 7 - 2*(b % 8)       REG_A = d       REG_B = e,  REG_C = c
//          bxc             # REG_B ^ REG_C -> REG_B            f = e ^ c                   REG_A = d       REG_B = f,  REG_C = c
//          out REG_B       # output REG_B % 8                  output f % 8
//          jnz 0           # if REG_A != 0 goto :Repeat
//
// Expected output: 2,4,1,7,7,5,0,3,1,7,4,1,5,5,3,0
//
// Observations: 
//   Flip n bits = x + ((2**n - 1) − 2*(x % 2**n))
//   Flip 3 bits = x ^ 7 = x + (7 - 2*(x % 8))
// 
//   The ^ 7 followed by % 8 seems to be inverting bits and then shifting them off 3 at a time.
//   Maybe it's just inverting each number from LSD to MSD and outputting?
//   Obsrved: An input of 8 digit number in REG_A yields 9 outputs
//   Maybe I'm looking for a 15 digit number?

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

bool RunProgramExpecting(List<long> registers, List<int> program, List<int> expectedOutput) {
    var ip = 0;   // instruction pointer
    var op = 0;   // output pointer

    while (ip < program.Count) {

        var opcode = program[ip];
        var operand = program[ip + 1];

        switch(opcode) {
            case 0: // adv
                var advResult = registers[REG_A] / (long)Math.Pow(2, ComboOperand(registers, operand));
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
                }
                break;
            case 4: // bxc
                var bxcResult = registers[REG_B] ^ registers[REG_C];
                registers[REG_B] = bxcResult;
                break;
            case 5: // out
                var outResult = ComboOperand(registers, operand) % 8;
                if (outResult != expectedOutput[op]) return false;
                op++;
                break;
            case 6: // bdv
                var bdvResult = registers[REG_A] / (long)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_B] = bdvResult;
                break;
            case 7: // cdv
                var cdvResult = registers[REG_A] / (long)Math.Pow(2, ComboOperand(registers, operand));
                registers[REG_C] = cdvResult;
                break;
        }
        ip +=2;
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
    // for (long i = 123456701234567; i < 7422630647200635; i++) {

    //     if (i % 100000 == 0) Console.WriteLine($"{i}");

    //     var runRegisters = registers.ToList();
    //     runRegisters[REG_A] = i;

    //     var runResult = RunProgramExpecting(runRegisters, program, program);

    //     if (runResult) {
    //         Console.WriteLine($"Found: {i}");
    //         break;
    //     }
    // }
    Parallel.For(123456701234567, 7422630647200635, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
    {
        // Ensure only one thread prints at a time (to avoid interleaving issues)
        if (i % 1000000 == 0)
            Console.Write(".");

        var runRegisters = registers.ToList();
        runRegisters[REG_A] = i;

        var runResult = RunProgramExpecting(runRegisters, program, program);

        if (runResult)
        {
            Console.WriteLine();
            Console.WriteLine($"Found: {i}");
            // Break out of the loop for all threads (note: Parallel.For doesn't allow a true "break")
            Environment.Exit(0); // Or another thread-safe way to exit the parallel work
        }
    });
}
