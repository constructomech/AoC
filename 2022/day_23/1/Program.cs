using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;


Stopwatch watch = new Stopwatch();
watch.Start();


var input = File.ReadAllLines("input.txt");
var map = Map.Parse(input);

var moveRules = new List<MoveRule>();
moveRules.Add(new MoveRule { OffsetsToCheck = { new Point(0, -1), new Point(1, -1), new Point(-1, -1)}, ProposedMoveOffset = new Point(0, -1) });
moveRules.Add(new MoveRule { OffsetsToCheck = { new Point(0, 1), new Point(1, 1), new Point(-1, 1)}, ProposedMoveOffset = new Point(0, 1) });
moveRules.Add(new MoveRule { OffsetsToCheck = { new Point(-1, 0), new Point(-1, -1), new Point(-1, 1)}, ProposedMoveOffset = new Point(-1, 0) });
moveRules.Add(new MoveRule { OffsetsToCheck = { new Point(1, 0), new Point(1, -1), new Point(1, 1)}, ProposedMoveOffset = new Point(1, 0) });

Console.WriteLine("initial state");
map.Print();
for (int step = 0; step < 10; step++)
{

    // Propose moves
    var moveTargets = map.GetMoveTargets();
    var prposedMoves = new List<(Point sourcePos, Point targetPos)>();

    foreach (var moveTarget in moveTargets)
    {
        foreach (var moveRule in moveRules) 
        {
            if (CanMove(moveTarget, moveRule)) 
            {
                var newTargetPos = moveTarget;
                newTargetPos.Offset(moveRule.ProposedMoveOffset);
                prposedMoves.Add((moveTarget, newTargetPos));
                break;
            }
        }
    }

    // Commit moves
    var moves = new Dictionary<Point, Point?>(); // Target from source, or null if duplicates
    foreach (var proposeMove in prposedMoves) 
    {
        if (!moves.ContainsKey(proposeMove.targetPos)) 
        {
            moves.Add(proposeMove.targetPos, proposeMove.sourcePos);
        }
        else
        {
            moves[proposeMove.targetPos] = null;
        }
    }

    foreach(var (targetPos, sourcePos) in moves.Where(kvp => kvp.Value != null))
    {
        map[sourcePos??new Point(0, 0)] = '.';
        map[targetPos] = '#';
    }

    // Update move rule order
    var swapRule = moveRules.First();
    moveRules.RemoveAt(0);
    moveRules.Add(swapRule);

    // Console.WriteLine($"End of round {step + 1}");
    // map.Print();
}

var result = map.GetEmptyGroundTiles();

watch.Stop();
Console.WriteLine($"Result: {result}, Completed in {watch.ElapsedMilliseconds}ms");

bool CanMove(Point target, MoveRule rule) 
{
    foreach (var offset in rule.OffsetsToCheck) 
    {
        Point checkPos = target;
        checkPos.Offset(offset);
        if (map[checkPos] == '#')
        {
            return false;
        }
    }
    return true;
}

public class MoveRule
{
    public List<Point> OffsetsToCheck = new List<Point>();

    public Point ProposedMoveOffset;
}

public class Map
{
    public static Map Parse(string[] input)
    {
        var map = new Map();
        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            for (var x = 0; x < line.Length; x++)
            {
                map[new Point(x, y)] = line[x];
            }
        }
        return map;
    }

    public char this[Point point]
    {
        get
        {
            char result;
            if (this.cells.TryGetValue(point, out result))
            {
                return result;
            }
            return '.';
        }
        set
        {
            if (point.X < minX) minX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.X > maxX) maxX = point.X;
            if (point.Y > maxY) maxY = point.Y;

            this.cells[point] = value;
        }
    }

    public void Print() 
    {
        for (int y = this.minY; y <= this.maxY; y++) 
        {
            for (int x = this.minX; x <= this.maxX; x++) 
            {
                Console.Write(this[new Point(x, y)]);
            } 
            Console.WriteLine();
        }       
    }

    public int GetEmptyGroundTiles() 
    {
        int sum = 0;
        for (int y = this.minY; y <= this.maxY; y++) 
        {
            for (int x = this.minX; x <= this.maxX; x++) 
            {
                if (this[new Point(x, y)] == '.') sum++;
            } 
        }       
        return sum;
    }

    public List<Point> GetMoveTargets()
    {
        var result = new List<Point>();
        foreach (var (point, glyph) in cells.Where(kvp => kvp.Value == '#'))
        {
            foreach (var adjacentOffset in allAdjacentOffsets)
            {
                var checkPos = point;
                checkPos.Offset(adjacentOffset);
                if (this[checkPos] != '.')
                {
                    result.Add(point);
                    break;
                }
            }
        }
        return result;
    }

    List<Point> allAdjacentOffsets = new List<Point>() {
        new Point(0, 1),
        new Point(1, 0),
        new Point(0, -1),
        new Point(-1, 0),
        new Point(-1, -1),
        new Point(-1, 1),
        new Point(1, 1),
        new Point(1, -1),
    };

    private int minX = int.MaxValue;
    private int minY = int.MaxValue;
    private int maxX = int.MinValue;
    private int maxY = int.MinValue;

    private Dictionary<Point, char> cells = new Dictionary<Point, char>();
}