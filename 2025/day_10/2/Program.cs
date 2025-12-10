using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var machines = new List<(List<bool> lightTarget, List<List<int>> buttons, List<int> joltageTarget)>();
    foreach (var line in input)
    {
        var parts = line.Split(' ');
        var lightTarget = parts[0].Substring(1, parts[0].Length - 2).Select(c => c == '#').ToList();

        var buttons = new List<List<int>>();
        foreach (var part in parts[1..(parts.Length - 1)])
        {
            var button = part[1..(part.Length - 1)].Split(',').Select(i => Convert.ToInt32(i)).ToList();
            buttons.Add(button);
        }

        var lastPart = parts[parts.Length - 1];
        var joltageTarget = lastPart[1..(lastPart.Length - 1)].Split(',').Select(c => Convert.ToInt32(c)).ToList();

        machines.Add((lightTarget, buttons, joltageTarget));
    }

    foreach (var machine in machines)
    {
        var minPresses = SolveMachine(machine.buttons, machine.joltageTarget);
        if (minPresses.HasValue)
        {
            Console.WriteLine($"Found min presses: {minPresses}\n");
            result += minPresses.Value;
        }
    }

    Console.WriteLine($"Result: {result}");
}

long? SolveMachine(List<List<int>> buttons, List<int> joltageTarget) {
    int rows = joltageTarget.Count;
    int cols = buttons.Count;

    PrintMachine(buttons, joltageTarget);
    
    // Build matrix (rows represent joltage impact and columns the button that impacts them)
    var matrix = new double[rows, cols + 1];
    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < cols; c++)
        {
            if (buttons[c].Contains(r)) {
                matrix[r, c] = 1.0;
            }
            else {
                matrix[r, c] = 0.0;
            }
        }
        matrix[r, cols] = joltageTarget[r];
    }

    PrintMatrix(matrix, "initial");

    // Gaussian Elimination
    var pivotRow = 0;
    var pivotCols = new List<int>();

    for (var c = 0; c < cols && pivotRow < rows; c++)
    {
        var sel = -1;
        for (var r = pivotRow; r < rows; r++) 
        {
            if(Math.Abs(matrix[r, c]) > 0)
            {
                sel = r;
                break;
            }
        }
        
        if (sel == -1) continue;
        
        pivotCols.Add(c);
        
        // Swap
        for (var k = c; k <= cols; k++)
        {
            var tmp = matrix[pivotRow, k];
            matrix[pivotRow, k] = matrix[sel, k];
            matrix[sel, k] = tmp;
        }
        
        // Normalize
        var val = matrix[pivotRow, c];
        for (var k = c; k <= cols; k++)
        {
            matrix[pivotRow, k] /= val;
        }
        
        // Eliminate
        for (var r = 0; r < rows; r++)
        {
            if (r != pivotRow)
            {
                var f = matrix[r, c];
                for (var k = c; k <= cols; k++)
                {
                    matrix[r, k] -= f * matrix[pivotRow, k];
                } 
            }
        }
        pivotRow++;
    }

    PrintMatrix(matrix, "Post Gaussian Elimination:");

    // Identify free variable columns for brute force
    var freeCols = new List<int>();
    for (var c = 0; c < cols; c++)
    {
        if (!pivotCols.Contains(c))
        {
            freeCols.Add(c);
        }
    }

    var freeBounds = new long[freeCols.Count];
    if (freeCols.Count > 0)
    {
        // Determine bounds for free variables
        for (var i = 0; i < freeCols.Count; i++)
        {
            var freeCol = freeCols[i];
            long bound = long.MaxValue;
            bool hasEffect = false;
            for (var r = 0; r < rows; r++)
            {
                if (buttons[freeCol].Contains(r))
                {
                    hasEffect = true;
                    if (joltageTarget[r] < bound)
                    {
                    bound = joltageTarget[r];
                    }
                }
            }
            if (!hasEffect) bound = 0;
            freeBounds[i] = bound;
        }

        PrintFreeCols(freeCols, freeBounds);
    }

    long? minTotal = null;
    
    // Recursion
    void Rec(int freeIdx, long[] currentFreeVals)
    {
        if (freeIdx == freeCols.Count)
        {
            // Calculate pivot variables
            long currentSum = 0;
            foreach (var v in currentFreeVals)
            {
                currentSum += v;
            }
            
            bool possible = true;
            for (var i = 0; i < pivotCols.Count; i++)
            {
                var pCol = pivotCols[i];
                var pRow = i; 
                
                var val = matrix[pRow, cols];
                for (var j = 0; j < freeCols.Count; j++)
                {
                    val -= matrix[pRow, freeCols[j]] * currentFreeVals[j];
                }
                
                var lVal = (long)Math.Round(val);
                if (val < -1e-5 || Math.Abs(val - lVal) > 1e-5)
                {
                    possible = false;
                    break;
                }
                
                currentSum += lVal;
            }
            
            if (possible)
            {
                if (minTotal == null || currentSum < minTotal.Value)
                {
                    minTotal = currentSum;
                }
            }
            return;
        }
        
        // Iterate free variable
        long bound = freeBounds[freeIdx];
        for (var v = 0L; v <= bound; v++)
        {
            currentFreeVals[freeIdx] = v;
            Rec(freeIdx + 1, currentFreeVals);
        }
    }
    
    Rec(0, new long[freeCols.Count]);
    
    return minTotal;
}

void PrintMachine(List<List<int>> buttons, List<int> joltageTarget)
{
    var buttonStrs = buttons.Select(b => $"({string.Join(",", b)})");
    var joltageStr = $"{{{string.Join(",", joltageTarget)}}}";
    Console.WriteLine($"{string.Join(" ", buttonStrs)} {joltageStr}");
}

void PrintFreeCols(List<int> freeCols, long[] freeBounds)
{
    Console.WriteLine("\nFree Columns:");
    for (int i = 0; i < freeCols.Count; i++)
    {
        Console.WriteLine($"Col {freeCols[i]}: [0, {freeBounds[i]}]");
    }
}

void PrintMatrix(double[,] matrix, string label = "Matrix")
{
    int rows = matrix.GetLength(0);
    int cols = matrix.GetLength(1);
    
    Console.WriteLine($"\n{label}:");
    Console.WriteLine(new string('-', cols * 9 + 5));
    
    for (int r = 0; r < rows; r++)
    {
        Console.Write("| ");
        for (int c = 0; c < cols; c++)
        {
            // Add separator before the last column (augmented part)
            if (c == cols - 1)
            {
                Console.Write("| ");
            }
            Console.Write($"{matrix[r, c],8:F2} ");
        }
        Console.WriteLine("|");
    }
    Console.WriteLine(new string('-', cols * 9 + 5));
}

