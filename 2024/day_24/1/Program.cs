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
    var connections = new Dictionary<string, (string lhs, string op, string rhs)>();
    foreach (Match match in Regex.Matches(input, connectionsPattern, RegexOptions.Multiline)) {
        connections.Add(match.Groups[4].Value, (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
    }

    var final = RunSim(initialValues, connections);
    Console.WriteLine($"Result: {final}");
}

ulong RunSim(Dictionary<string, byte> knownValues, Dictionary<string, (string lhs, string op, string rhs)> gates) {
    var unsolved = gates.Keys.ToList();    

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
    }

    var outputVariables = knownValues.Where(kvp => kvp.Key.StartsWith('z')).OrderBy(kvp => kvp.Key);
    var operativeBits = outputVariables.Select(kvp => kvp.Value).ToList();
    ulong result = 0;
    for (var i = operativeBits.Count - 1; i >= 0; i--) {

        result |= (ulong)operativeBits[i] << i;
        Console.Write(operativeBits[i]);
    }
    Console.WriteLine();

    return result;
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
