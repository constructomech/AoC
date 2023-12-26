using System.Diagnostics;
using MathNet.Numerics;


Pos[] OFFSETS = new Pos[] { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


static int Modulus(int k, int n) => (k %= n) < 0 ? k + n : k; 

void Run(string[] grid) {

    var height = grid.Length;
    var width = grid[0].Length;
    Debug.Assert(height == width && grid[height/2][width/2] == 'S', "Approach won't work if start isn't exactly in the center of a square grid");

    // It turns out that 2,6501,365 is a multiple of the maze size + 65 (which is itself the distance 
    //  to the edge from the starting point in the exact center of a 131x131 square)
    // From manual observation, the periodicity of the step count (Y) is equal to the size of the grid.
    //  Therefore, we can calculate the number of steps at 3 boarder crossings and use that to extrapolate
    //  the number of steps at the final crossing, which is the answer. I hit a rounding error on the final 
    //  answer -1 but I tried truncating at that did the trick. Clearly there's a percision issue.
    //  Also: MathNet.Numerics FTW!

    var xFinal = Math.DivRem(26501365, width, out var remainder);
    var borderCrossings = new int[] { remainder, remainder + width, remainder + 2*width };

    var visited = new HashSet<Pos>();
    var queue = new Queue<Pos>();
    queue.Enqueue(new Pos(width / 2, height / 2));
    var total = new int[] { 0, 0 }; // [even, odd]
    var Y = new List<int>();

    for (int step = 1; step <= borderCrossings[^1]; step++) {
        int queueCount = queue.Count;

        for (int count = 0; count < queueCount; count++) {
            Pos currentPos = queue.Dequeue();

            foreach (var offset in OFFSETS) {
                Pos newPos = currentPos + offset;

                if (visited.Contains(newPos) || grid[Modulus(newPos.y, height)][Modulus(newPos.x, width)] == '#')
                    continue;

                visited.Add(newPos);
                queue.Enqueue(newPos);
                total[step % 2]++;
            }
        }

        if (borderCrossings.Contains(step))
            Y.Add(total[step % 2]);
    }

    var X = new double[] { 0, 1, 2 };
    var YVector = Y.Select(y => (double)y).ToArray();
    var func = Fit.PolynomialFunc(X, YVector, 2);
    var yFinal = func(xFinal);
    var result = (Int64)yFinal;

    Console.WriteLine($"Result: {result}");
}

public record Pos(int x, int y) {
    public static Pos operator+(Pos lhs, Pos rhs) {
        return new Pos(lhs.x + rhs.x, lhs.y + rhs.y);
    }
}
