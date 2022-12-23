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
            (position, facing) = Step(map, position, facing);
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

(Point position, Facing facing) Step(Map map, Point position, Facing facing) 
{
    var offset = moveOffsets[facing];
    var prevPosition = position;
    var prevFacing = facing;

    position.Offset(offset);
    if (map.cells.ContainsKey(position))
    {
        if (map.cells[position] == '#')
        {
            return (prevPosition, facing);
        }
    }
    else // out of bounds
    {
        (position, facing) = Wrap(map, prevPosition, position, facing);

        if (map.cells[position] == '#') return (prevPosition, prevFacing);
    }
    return (position, facing);
}

(Point position, Facing facing) Wrap(Map map, Point prevPosition, Point position, Facing facing) 
{
    Point newPosition;
    const int cubeSize = 50;
    var (face, cubeRelativePos) = FaceOf(cubeSize, prevPosition);
    switch (face)
    {
        case 1:
            switch(facing) 
            {
                case Facing.Up:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 6, cubeRelativePos);
                    return (newPosition, Facing.Up);
                case Facing.Right:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 4, cubeRelativePos);
                    return (newPosition, Facing.Left);
                case Facing.Down:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 3, cubeRelativePos);
                    return (newPosition, Facing.Left);
                default: throw new InvalidOperationException();
            }
        case 2:
            switch(facing) 
            {
                case Facing.Up:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 6, cubeRelativePos);
                    return (newPosition, Facing.Right);
                case Facing.Left:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 5, cubeRelativePos);
                    return (newPosition, Facing.Right);
                default: throw new InvalidOperationException();
            }
        case 3:
            switch(facing) 
            {
                case Facing.Right:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 1, cubeRelativePos);
                    return (newPosition, Facing.Up);
                case Facing.Left:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 5, cubeRelativePos);
                    return (newPosition, Facing.Down);
                default: throw new InvalidOperationException();
            }
        case 4:
            switch(facing) 
            {
                case Facing.Right:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 1, cubeRelativePos);
                    return (newPosition, Facing.Left);
                case Facing.Down:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 6, cubeRelativePos);
                    return (newPosition, Facing.Left);
                default: throw new InvalidOperationException();
            }
        case 5:
            switch(facing) 
            {
                case Facing.Left:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 2, cubeRelativePos);
                    return (newPosition, Facing.Right);
                case Facing.Up:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 3, cubeRelativePos);
                    return (newPosition, Facing.Right);
                default: throw new InvalidOperationException();
            }
        case 6:
            switch(facing) 
            {
                case Facing.Right:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 4, cubeRelativePos);
                    return (newPosition, Facing.Up);
                case Facing.Down:
                    cubeRelativePos = FlipY(cubeSize, cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 1, cubeRelativePos);
                    return (newPosition, Facing.Down);
                case Facing.Left:
                    cubeRelativePos = SwapXY(cubeRelativePos);
                    newPosition = FaceRelativeToAbsolute(cubeSize, 2, cubeRelativePos);
                    return (newPosition, Facing.Down);
                default: throw new InvalidOperationException();
            }

        default: throw new InvalidOperationException();
    }
}

(int faceNum, Point faceRelativePosition) FaceOf(int cubeSize, Point position) 
{
    int xGridPos = position.X / cubeSize;
    int yGridPos = position.Y / cubeSize;

    var relativePos = new Point(position.X % cubeSize, position.Y % cubeSize);

    if (xGridPos == 1 && yGridPos == 0) return (2, relativePos);
    if (xGridPos == 2 && yGridPos == 0) return (1, relativePos);
    if (xGridPos == 1 && yGridPos == 1) return (3, relativePos);
    if (xGridPos == 0 && yGridPos == 2) return (5, relativePos);
    if (xGridPos == 1 && yGridPos == 2) return (4, relativePos);
    if (xGridPos == 0 && yGridPos == 3) return (6, relativePos);

    throw new InvalidOperationException();
}

Point FlipX(int cubeSize, Point position) 
{
    return new Point(cubeSize - position.X - 1, position.Y);
}

Point FlipY(int cubeSize, Point position)
{
    return new Point(position.X, cubeSize - position.Y - 1);
}

Point SwapXY(Point position)
{
    return new Point(position.Y, position.X);
}

Point FaceRelativeToAbsolute(int cubeSize, int faceNum, Point faceRelativePosition) 
{
    int xGridPos = 0;
    int yGridPos = 0;
    switch (faceNum) 
    {
        case 1:
            xGridPos = 2;
            yGridPos = 0;
            break;
        case 2:
            xGridPos = 1;
            yGridPos = 0;
            break;
        case 3:
            xGridPos = 1;
            yGridPos = 1;
            break;
        case 4:
            xGridPos = 1;
            yGridPos = 2;
            break;
        case 5:
            xGridPos = 0;
            yGridPos = 2;
            break;
        case 6:
            xGridPos = 0;
            yGridPos = 3; 
            break;    
        default: throw new InvalidOperationException();
    }

    Point position = new Point(xGridPos * cubeSize, yGridPos * cubeSize);
    position.Offset(faceRelativePosition);

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