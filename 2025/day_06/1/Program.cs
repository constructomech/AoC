using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var problems = new List<List<long>>();

    foreach (var line in input[..(input.Length - 1)])
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for( var i = 0; i < parts.Length; i++)
        {
            if (problems.Count <= i)
            {
                problems.Add(new List<long>());
            }
            var val = Convert.ToInt64(parts[i]);
            problems[i].Add(val);
        }

    }

    var operationStrs = input[input.Length - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    for (var i = 0; i < operationStrs.Length; i++)
    {
        var opResult = DoOp(operationStrs[i][0], problems[i]);
        result += opResult;
    }

    Console.WriteLine($"Result: {result}");
}

long DoOp(char op, List<long> operands)
{
    var result = operands[0];
    for (int i = 1; i < operands.Count; i++)
    {
        switch(op)
        {
            case '+':
                result += operands[i];
                break;
            case '*':
                result *= operands[i];
                break;
        }
    }
    return result;
}

