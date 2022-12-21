using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var monkies = new Dictionary<string, Monkey>();

var input = File.ReadAllLines("input.txt");
foreach (var line in input)
{
    var parts = line.Split(':');
    var monkeyName = parts[0];

    parts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 1)
    {
        monkies.Add(monkeyName, new Monkey(Convert.ToInt32(parts[0])));
    }
    else
    {
        Func<long, long, long>? op = null;
        Func<long, long, long>? lhsMissingOp = null;
        Func<long, long, long>? rhsMissingOp = null;

        switch(parts[1])
        {
            case "+": 
                op = (lhs, rhs) => lhs + rhs;
                lhsMissingOp = (rhs, result) => result - rhs;
                rhsMissingOp = (lhs, result) => result - lhs;
                break;
            case "-": 
                op = (lhs, rhs) => lhs - rhs;  
                lhsMissingOp = (rhs, result) => rhs + result;
                rhsMissingOp = (lhs, result) => lhs - result;
                break;
            case "*": 
                op = (lhs, rhs) => lhs * rhs;  
                lhsMissingOp = (rhs, result) => result / rhs;
                rhsMissingOp = (lhs, result) => result / lhs;
                break;
            case "/": 
                op = (lhs, rhs) => lhs / rhs;  
                lhsMissingOp = (rhs, result) => rhs * result;
                rhsMissingOp = (lhs, result) => lhs / result;
                break;
        }

        monkies.Add(monkeyName, new Monkey(parts[0], parts[2], op!, lhsMissingOp, rhsMissingOp));
    }
}

// Implement here
var root = monkies["root"];
var lhs = root.lhs;
var rhs = root.rhs;

Monkey? fowardSolve = null;
Monkey? backwardSolve = null;
if (monkies[lhs].Contains(monkies, "humn"))
{
    fowardSolve = monkies[rhs];
    backwardSolve = monkies[lhs];
}
else
{
    fowardSolve = monkies[lhs];
    backwardSolve = monkies[rhs];
}

var result = fowardSolve.Compute(monkies);
var unknown = backwardSolve.ComputeUnknown(monkies, result);

watch.Stop();
Console.WriteLine($"Result: {unknown}, Completed in {watch.ElapsedMilliseconds}ms");


class Monkey {
    public Monkey(long value)
    {
        this.fixedValue = value;
    }

    public Monkey(string lhs, string rhs, Func<long, long, long> op, Func<long, long, long> lhsMissingOp, Func<long, long, long> rhsMissingOp)
    {
        this.lhs = lhs;
        this.rhs = rhs;
        this.op = op;
        this.lhsMissingOp = lhsMissingOp;
        this.rhsMissingOp = rhsMissingOp;
    }

    public long ComputeUnknown(Dictionary<string, Monkey> monkies, long targetValue)
    {
        if (lhs == "humn" || monkies[lhs].Contains(monkies, "humn"))
        {
            var rhsValue = monkies[rhs].Compute(monkies);

            var newTargetValue = lhsMissingOp(rhsValue, targetValue);

            if (lhs == "humn") return newTargetValue;
            return monkies[lhs].ComputeUnknown(monkies, newTargetValue);
        }
        else
        {
            var lhsValue = monkies[lhs].Compute(monkies);

            var newTargetValue = rhsMissingOp(lhsValue, targetValue);

            if (rhs == "humn") return newTargetValue;
            return monkies[rhs].ComputeUnknown(monkies, newTargetValue);
        }
    }

    public long Compute(Dictionary<string, Monkey> monkies) {
        if (this.fixedValue != null) 
        {
            return this.fixedValue??0;
        }
        else
        {
            var lhsMonkey = monkies[this.lhs];
            var rhsMonkey = monkies[this.rhs];

            var lhsValue = lhsMonkey.Compute(monkies);
            var rhsValue = rhsMonkey.Compute(monkies);

            return this.op(lhsValue, rhsValue);  
        }
    }

    public bool Contains(Dictionary<string, Monkey> monkies, string name)
    {
        if (this.fixedValue != null) return false;

        if (this.lhs == name) return true;
        if (this.rhs == name) return true;

        return monkies[this.lhs].Contains(monkies, name) || monkies[this.rhs].Contains(monkies, name);
    }

    public string? lhs;

    public string? rhs;

    private Func<long, long, long>? op;
    private Func<long, long, long>? lhsMissingOp;
    private Func<long, long, long>? rhsMissingOp;

    private long? fixedValue;
}

