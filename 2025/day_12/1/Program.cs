using System.Diagnostics;
using System.Numerics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var presents = new List<bool[,]>();
    var regions = new List<(Vec2 size, int[] targetPresents)>();

    for (var i = 0; i < input.Length; i++)
    {
        if (input[i].EndsWith(':'))
        {
            var present = new bool[3,3];

            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    present[x, y] = input[i + y + 1][x] == '#';
                }
            }

            presents.Add(present);
            i += 4;
        }
        else
        {
            var parts = input[i].Split(':');
            var dims = parts[0].Split('x');

            var width = Convert.ToInt32(dims[0]);
            var height = Convert.ToInt32(dims[1]);

            var counts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s)).ToArray();

            regions.Add((new Vec2(width, height), counts));
        }
    }

    // Process regions in parallel
    int completed = 0;
    int total = regions.Count;
    
    var result = regions.AsParallel()
        .WithDegreeOfParallelism(Environment.ProcessorCount)
        .Count(region => {
            var fits = SolveRegion(region, presents);
            var done = Interlocked.Increment(ref completed);
            if (done % 50 == 0)
            {
                Console.WriteLine($"Progress: {done}/{total}");
            }
            return fits;
        });

    Console.WriteLine($"Result: {result}");
}

bool SolveRegion((Vec2 size, int[] targetPresents) region, List<bool[,]> presents)
{
    var byType = CreatePlacements(region.size, presents);
    
    // Precompute the minimum cells each piece type needs (smallest mask)
    var minCellsPerType = new int[byType.Length];
    for (int t = 0; t < byType.Length; t++)
    {
        minCellsPerType[t] = byType[t].Count > 0 
            ? byType[t].Min(p => (int)BigInteger.PopCount(p.mask)) 
            : 0;
    }
    
    bool Search(int[] remainingCounts, BigInteger forbiddenMask)
    {
        if (remainingCounts.All(c => c == 0)) return true;

        // Find piece type with fewest valid placements remaining (MRV on piece types)
        int bestType = -1;
        int bestCount = int.MaxValue;
        List<Placement>? bestPlacements = null;
        BigInteger coverableByRemaining = BigInteger.Zero;
        
        for (int t = 0; t < remainingCounts.Length; t++)
        {
            if (remainingCounts[t] == 0) continue;
            
            var validPlacements = new List<Placement>();
            foreach (var p in byType[t])
            {
                if ((p.mask & forbiddenMask) == 0)
                {
                    validPlacements.Add(p);
                    coverableByRemaining |= p.mask;
                }
            }
            
            if (validPlacements.Count == 0) return false;
            if (validPlacements.Count < remainingCounts[t]) return false;
            
            if (validPlacements.Count < bestCount)
            {
                bestCount = validPlacements.Count;
                bestType = t;
                bestPlacements = validPlacements;
            }
        }
        
        if (bestType == -1) return false;
        
        // Prune: check if there's enough "room" in coverable cells
        int minCellsNeeded = 0;
        for (int t = 0; t < remainingCounts.Length; t++)
        {
            minCellsNeeded += remainingCounts[t] * minCellsPerType[t];
        }
        int availableCells = (int)BigInteger.PopCount(coverableByRemaining);
        if (availableCells < minCellsNeeded) return false;

        // Try each valid placement for the most constrained piece type
        foreach (var p in bestPlacements!)
        {
            remainingCounts[bestType]--;
            if (Search(remainingCounts, forbiddenMask | p.mask))
                return true;
            remainingCounts[bestType]++;
        }
        
        return false;
    }

    // Clone targetPresents since Search modifies it
    var counts = (int[])region.targetPresents.Clone();
    return Search(counts, BigInteger.Zero);
}

List<Placement>[] CreatePlacements(Vec2 size, List<bool[,]> pieces)
{
    var result = new List<Placement>[pieces.Count];
    for (int t = 0; t < pieces.Count; t++)
    {
        result[t] = new List<Placement>();
    }

    for (var h = 0; h < size.Y - 2; h++)
    {
        for (var w = 0; w < size.X - 2; w++)
        {
            for (var piece = 0; piece < pieces.Count; piece++)
            {
                var seenMasks = new HashSet<BigInteger>();
                for (var rotation = 0; rotation < 4; rotation++)
                {
                    var mask = CreateMask(size, new Vec2(w, h), pieces[piece], rotation);
                    // Only add if this rotation produces a unique mask (avoid duplicates from symmetric pieces)
                    if (seenMasks.Add(mask))
                    {
                        result[piece].Add(new Placement() { pieceType = piece, mask = mask });
                    }
                }
            }
        }            
    }

    return result;
}

BigInteger CreateMask(Vec2 size, Vec2 pos, bool[,] piece, int rotation)
{
    BigInteger result = BigInteger.Zero;
    var rotatedPiece = RotatePiece(piece, rotation);

    for (var x = 0; x < rotatedPiece.GetLength(0); x++)
    {
        for (var y = 0; y < rotatedPiece.GetLength(1); y++)
        {
            if (rotatedPiece[x, y])
            {
                var globalX = pos.X + x;
                var globalY = pos.Y + y;

                var idx = globalX + globalY * size.X;
                result |= BigInteger.One << idx;
            }
        }
    }
    return result;
}

bool[,] RotatePiece(bool[,] piece, int rotation)
{
    if (rotation == 0) return piece;

    var width = piece.GetLength(0);
    var height = piece.GetLength(1);

    for (var r = 0; r < rotation; r++)
    {
        var rotated = new bool[height, width];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                rotated[y, width - 1 - x] = piece[x, y];
            }
        }
        piece = rotated;
        // After rotation, dimensions swap
        (width, height) = (height, width);
    }

    return piece;
}

struct Placement
{
    public int pieceType; // Need to remember this so I can select placements that are within the required piece counts.
    public BigInteger mask; // Locations that this placement will take up,
}

public record Vec2 (int X, int Y) {

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}
