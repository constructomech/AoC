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

        switch(parts[1])
        {
            case "+": op = (lhs, rhs) => lhs + rhs;  break;
            case "-": op = (lhs, rhs) => lhs - rhs;  break;
            case "*": op = (lhs, rhs) => lhs * rhs;  break;
            case "/": op = (lhs, rhs) => lhs / rhs;  break;
        }

        monkies.Add(monkeyName, new Monkey(parts[0], parts[2], op!));
    }
}

// Implement here
var value = monkies["root"].Compute(monkies);

watch.Stop();
Console.WriteLine($"Result: {value}, Completed in {watch.ElapsedMilliseconds}ms");


class Monkey {
    public Monkey(long value)
    {
        this.fixedValue = value;
    }

    public Monkey(string lhs, string rhs, Func<long, long, long> op)
    {
        this.lhs = lhs;
        this.rhs = rhs;
        this.op = op;
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

    private string? lhs;

    private string? rhs;

    private Func<long, long, long>? op;

    private long? fixedValue;
}

