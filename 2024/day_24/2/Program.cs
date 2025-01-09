using System.Diagnostics;
using System.Text.RegularExpressions;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllText("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string input) {
    var result = 0L;

    string initialValuesPattern = @"^([xy]\d{2}): (\d)";
    string connectionsPattern = @"^([a-z]{3}|[xy]\d{2}) (AND|OR|XOR) ([a-z]{3}|[xy]\d{2}) -> ([a-z]{3}|z\d{2})";

    // Extract initial values
    var initialValues = new Dictionary<string, byte>();
    foreach (Match match in Regex.Matches(input, initialValuesPattern, RegexOptions.Multiline))
    {
        initialValues.Add(match.Groups[1].Value, byte.Parse(match.Groups[2].Value));
    }

    // Extract connections
    var gates = new Dictionary<string, (string lhs, string op, string rhs)>();
    foreach (Match match in Regex.Matches(input, connectionsPattern, RegexOptions.Multiline)) {
        gates.Add(match.Groups[4].Value, (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
    }

    // Brute force: 222 pick 4 x 33ms = 98,538,115 * 33ms = 37 days
    //   The depth is 44 (indicating some kind of carry mechanic)
    //   45 named nodes are and0..44 by anding x0..44 with corresponding y0..44
    //   45 named nodes are xor0..44 by anding x0..44 with corresponding y0..44 (x0 xor y0 = z0)
    //

    var swaps = new List<(string a, string b)>() {("z19", "cph"), ("z13", "npf"), ("hgj", "z33"), ("gws", "nnt")};
    foreach (var swap in swaps) {
        PerformSwap(gates, swap);
    }

    var gateNames = BuildGateNames(initialValues, gates);

    Console.WriteLine($"AND: {gates.Count(kvp => kvp.Value.op == "AND")}, expected: 88");
    Console.WriteLine($"XOR: {gates.Count(kvp => kvp.Value.op == "XOR")}, expected: 88");
    Console.WriteLine($"OR: {gates.Count(kvp => kvp.Value.op == "OR")}, expected: 44");

    // Alias table tells me that the outputs to z09, z13, and z19 cannot be found by the pattern
    //  Of these, z13 and z19 are definately swapped as their gates have the wrong op (not XOR)
    //  z19 appears to be swapped with cph
    //  
    // By semi-automated analysis, I believe z19 <-> cph, z13 <-> npf, nnt <-> pcd

    // var x = GetValue(initialValues, 'x');
    // var y = GetValue(initialValues, 'y');
    // var desiredZ = x + y;

    // var swaps = new List<(string a, string b)>() {("z19", "cph"), ("z13", "npf")};
    // foreach (var swap in swaps) {
    //     PerformSwap(gates, swap);
    // }

    // var wrong = new SortedSet<string>() {"z19", "cph", "z13", "npf"};

    // var remainingGates = gates.Keys.Where(s => !s.StartsWith('z')).Except(["cph", "npf"]).ToList();

    // foreach (var p0 in GetPermutations(remainingGates, 2)) {
    //     var a = p0.ToArray();
    //     PerformSwap(gates, (a[0], a[1]));

    //     foreach (var p1 in GetPermutations(remainingGates.Except(p0), 2)) {
    //         var b = p1.ToArray();
    //         PerformSwap(gates, (b[0], b[1]));

    //         var maxValue = (long)Math.Pow(2, 44);
    //         bool passed = true;
    //         for (var i = 0; i < 5; i++) {
    //             var xTest = (ulong)Random.Shared.NextInt64(maxValue);
    //             var yTest = (ulong)Random.Shared.NextInt64(maxValue);
    //             var zTest = xTest + yTest;

    //             var zActual = RunSim(initialValues.ToDictionary(), gates);
    //             if (zActual != zTest) {
    //                 passed = false;
    //                 break;
    //             }
    //         }

    //         if (passed) {
    //             Console.WriteLine($"Found One: ({a[0]}, {a[1]}), ({b[0]}, {b[1]})");
    //             // goto Done;
    //         }

    //         PerformSwap(gates, (b[0], b[1]));
    //     }

    //     PerformSwap(gates, (a[0], a[1]));
    // }

    // Done:

    // Console.WriteLine($"{string.Join(",", wrong)}");

    // Well, I didn't intuit the answer correctly. Must have one or more of my manual finds incorrect.
    // Next insight:  Run the algorithm bit by bit, note the gates that lit up for each bit that is output. 
    // First bit that is wrong has an incorrect gate in the set of new gates used since last correct bit.
}

void PerformSwap(Dictionary<string, (string lhs, string op, string rhs)> gates, (string a, string b) swap) {
    var gateA = gates[swap.a];
    var gateB = gates[swap.b];
    gates[swap.a] = gateB;
    gates[swap.b] = gateA;
}

SortedDictionary<string, string> BuildGateNames(Dictionary<string, byte> knownValues, Dictionary<string, (string lhs, string op, string rhs)> gates) {
    var aliases = new SortedDictionary<string, string>();

    foreach (var gate in gates) {
        var in0 = gate.Value.lhs;
        var in1 = gate.Value.rhs;

        if (gate.Value.op == "XOR" && ((in0.StartsWith('x') && in1.StartsWith('y')) || (in1.StartsWith('x') && in0.StartsWith('y')))) {
            var lhsNum = byte.Parse(gate.Value.lhs.Substring(1));
            var rhsNum = byte.Parse(gate.Value.rhs.Substring(1));

            if (lhsNum != rhsNum) throw new InvalidOperationException();
            
            aliases.Add("xor" + lhsNum.ToString("D2"), gate.Key);
        }

        if (gate.Value.op == "AND" && ((in0.StartsWith('x') && in1.StartsWith('y')) || (in1.StartsWith('x') && in0.StartsWith('y')))) {
            var lhsNum = byte.Parse(gate.Value.lhs.Substring(1));
            var rhsNum = byte.Parse(gate.Value.rhs.Substring(1));

            if (lhsNum != rhsNum) throw new InvalidOperationException();
            
            aliases.Add("and" + lhsNum.ToString("D2"), gate.Key);
        }
    }

    // Pass two
    foreach (var gate in gates) {

        if (gate.Key.StartsWith('z')) {
            var outputNumn = byte.Parse(gate.Key.Substring(1));
            var inputAlias = "xor" + outputNumn.ToString("D2");

            // z45 is really carry-out 44
            if (gate.Value.op != "XOR" && outputNumn != 45) {
                Console.WriteLine("Wrong gate!");
                throw new InvalidOperationException();
            }

            if (outputNumn != 45 && (gate.Value.lhs == aliases[inputAlias] || gate.Value.rhs == aliases[inputAlias])) {
                aliases.Add("output" + outputNumn.ToString("D2"), gate.Key);
            }
        }

        if (gate.Value.op == "OR") {
            bool found = false;
            foreach (var input in new string[] { gate.Value.lhs, gate.Value.rhs }) {
                var mapping = aliases.Where(kvp => kvp.Value == input);
                if (mapping.Any()) {
                    string alias = mapping.First().Key;
                    if (alias.StartsWith("and")) {
                        var num = int.Parse(alias.Substring(3));                        
                        aliases.Add("carry" + num.ToString("D2"), gate.Key);
                        found = true;
                        break;
                    }                    
                }
            }
            if (!found) {
                Console.WriteLine("Can't identify carry.");
            }      
        }
    }

    return aliases;
}

bool TestConfiguration(Dictionary<string, byte> knownValues, Dictionary<string, (string lhs, string op, string rhs)> gates, ulong x, ulong y) {
    var expected = x + y;
    var inputs = knownValues.ToDictionary();

    SetValue(knownValues, 'x', x);
    SetValue(knownValues, 'y', y);

    var proposed = RunSim(inputs, gates);

    return proposed == expected;
}

ulong? RunSim(Dictionary<string, byte> knownValues, Dictionary<string, (string lhs, string op, string rhs)> gates) {
    var unsolved = gates.Keys.ToList();
    var unsolvedCount = unsolved.Count;

    while (unsolved.Count > 0) {
        for (int i = 0; i < unsolved.Count; i++) {
            var variable = unsolved[i];
            var gate = gates[variable];

            if (knownValues.ContainsKey(gate.lhs) && knownValues.ContainsKey(gate.rhs)) {
                var solution = Solve(knownValues, gate);
                knownValues.Add(variable, solution);
                unsolved.RemoveAt(i);
                i--;
            }
        }

        if (unsolvedCount == unsolved.Count) {
            // No progress. Abort!
            return null;
        }
        unsolvedCount = unsolved.Count;
    }

    return GetValue(knownValues, 'z');
}

ulong GetValue(Dictionary<string, byte> knownValues, char variable) {
    var inputBits = knownValues.Where(kvp => kvp.Key.StartsWith(variable)).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
    ulong result = 0;
    for (var i = inputBits.Count - 1; i >= 0; i--) {

        result |= (ulong)inputBits[i] << i;
        // Console.Write(inputBits[i]);
    }
    // Console.WriteLine();

    return result;
}

void SetValue(Dictionary<string, byte> knownValues, char variable, ulong value) {
    for (var i = 0; i < 44; i++) {
        var test = (ulong)1 << i;
        knownValues[variable + i.ToString("D2")] = (value & test) == test ? (byte)1 : (byte)0;
    }
}

byte Solve(Dictionary<string, byte> knownValues, (string lhs, string op, string rhs) gate) {
    var lhsValue = knownValues[gate.lhs];
    var rhsValue = knownValues[gate.rhs];

    switch (gate.op) {
        case "AND":
            return (byte)(lhsValue & rhsValue);
        case "OR":
            return (byte)(lhsValue | rhsValue);
        case "XOR":
            return (byte)(lhsValue ^ rhsValue);
    }
    throw new ArgumentOutOfRangeException();
}

IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count) {
    var i = 0;
    foreach(var item in items) {
        if(count == 1) {
            yield return new T[] { item };
        }
        else {
            foreach(var result in GetPermutations(items.Skip(i + 1), count - 1)) {
                yield return new T[] { item }.Concat(result);
            }
        }
        i++;
    }
}