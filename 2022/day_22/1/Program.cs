using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;


Stopwatch watch = new Stopwatch();
watch.Start();

Dictionary<Facing, Point> moveOffsets = new Dictionary<Facing, Point> {
    { Facing.Right, new Point(1, 0) },
    { Facing.Down, new Point(0, 1) },
    { Facing.Left, new Point(-1, 0) },
    { Facing.Up, new Point(0, -1) },
};

var input = File.ReadAllLines("input.txt");
var (map, moves) = Map.Parse(input);

var facing = Facing.Right;
var position = map.Start;

foreach (var move in moves)
{
    if (move.Turn != null)
    {
        facing = Turn(facing, move.Turn);
    }
    else
    {
        for (int step = 0; step < move.Steps; step++)
        {
            position = Step(map, position, facing);
        }
    }
}

int result = 1000 * (position.Y + 1) + 4 * (position.X + 1) + (int)facing;

watch.Stop();
Console.WriteLine($"Result: {result}, Completed in {watch.ElapsedMilliseconds}ms");

// Facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^)

Facing Turn(Facing currentFacing, Facing? turn)
{
    switch (turn)
    {
        case Facing.Right:
        {
            switch(currentFacing)
            {
                case Facing.Right:  return Facing.Down;
                case Facing.Down:   return Facing.Left;
                case Facing.Left:   return Facing.Up;
                case Facing.Up:     return Facing.Right;
            }
            break;
        }
        case Facing.Left:
        {
            switch(currentFacing)
            {
                case Facing.Right:  return Facing.Up;
                case Facing.Down:   return Facing.Right;
                case Facing.Left:   return Facing.Down;
                case Facing.Up:     return Facing.Left;
            }
            break;
        }
    }
    throw new InvalidOperationException();
}

Point Step(Map map, Point position, Facing facing) 
{
    var offset = moveOffsets[facing];
    var prevPosition = position;

    position.Offset(offset);
    if (map.cells.ContainsKey(position))
    {
        if (map.cells[position] == '#')
        {
            return prevPosition;
        }
    }
    else // out of bounds
    {
        switch (facing) 
        {
            case Facing.Right:  position = new Point(0, position.Y);        break;
            case Facing.Down:   position = new Point(position.X, 0);        break;
            case Facing.Left:   position = new Point(map.maxX, position.Y); break;
            case Facing.Up:     position = new Point(position.X, map.maxY); break;
        }
        while (!map.cells.ContainsKey(position)) 
        {
            position.Offset(offset);
        }
        if (map.cells[position] == '#') return prevPosition;
    }
    return position;
}

public enum Facing
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
}
public class Move
{
    public Facing? Turn { get; set; }
    public int? Steps { get; set; }
}

public class Map
{
    public static (Map, List<Move>) Parse(string[] input)
    {
        var map = new Map();

        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            if (line.Length == 0) break;

            for (var x = 0; x < line.Length; x++)
            {
                if (map.cells.Count == 0)
                {
                    map.Start = new Point(x, y);
                }
                if (line[x] != ' ')
                {
                    map.cells.Add(new Point(x, y), line[x]);
                    if (x > map.maxX) map.maxX = x;
                    if (y > map.maxY) map.maxY = y;
                }
            }
        }

        var instructions = input[input.Length - 1];
        var moves = new List<Move>();
        int accum = 0;

        foreach (char c in instructions)
        {
            if (char.IsDigit(c))
            {
                accum = 10 * accum + (c - '0');
            }
            else
            {
                if (accum > 0)
                {
                    moves.Add(new Move() { Steps = accum });
                }
                if (c == 'L') moves.Add(new Move() { Turn = Facing.Left });
                if (c == 'R') moves.Add(new Move() { Turn = Facing.Right });
                accum = 0;
            }
        }
        if (accum > 0)
        {
            moves.Add(new Move() { Steps = accum });
        }

        return (map, moves);
    }

    public Point Start { get; set; }

    public Dictionary<Point, char> cells = new Dictionary<Point, char>();

    public int maxX;
    public int maxY;
}