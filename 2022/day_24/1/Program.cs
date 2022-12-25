using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");
var map = Map.Parse(input);
var playerPos = map.Start;

Console.WriteLine("Initial State");
map.Print();
var posSuperposition = new HashSet<Point>();

posSuperposition.Add(map.Start);

int step = 0;
while (true)
{
    // Advance board to see what positions will be available.
    map.Step(step++);

    // Add all valid moves to check list.    
    var nextPosSuperposition = new HashSet<Point>();

//    Console.Write("[");
    foreach (var pos in posSuperposition)
    {
//        Console.Write($"({pos.X}, {pos.Y}) ");
        foreach (var validMove in map.ValidMovesFrom(pos))
        {
            nextPosSuperposition.Add(validMove);
        }
    }
//    Console.WriteLine("]");

    // Update the position to the next position.
    posSuperposition = nextPosSuperposition;

    if (nextPosSuperposition.Contains(map.End)) break;
}

watch.Stop();
Console.WriteLine($"Step {step}, Completed in {watch.ElapsedMilliseconds}ms");


public class Map
{
    public static Map Parse(string[] input)
    {
        bool startSet = false;

        var map = new Map();
        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            for (var x = 0; x < line.Length; x++)
            {
                var glyph = line[x];
                var point = new Point(x, y);
                switch (glyph)
                {
                    case '#':
                        map[point] = glyph;
                        break;
                    case '.':
                        map[point] = glyph;
                        if (startSet == false)
                        {
                            map.Start = point;
                            startSet = true;
                        }
                        map.End = point;
                        break;
                    case '^':
                    case '<':
                    case 'v':
                    case '>':
                        map[point] = '.';
                        map.Blizzards.Add((point, glyph), true);
                        break;
                    default: throw new InvalidOperationException();
                }

            }
        }
        return map;
    }

    public Point Start { get; private set; }

    public Point End { get; private set; }

    public SortedList<(Point, char), bool> Blizzards = new SortedList<(Point, char), bool>(new PointComparer());

    public char this[Point point]
    {
        get
        {
            if (point.X < minX || point.Y < minY || point.X > maxX || point.Y > maxY) return ' ';

            if (this.Blizzards.ContainsKey((point, '^')))
            {
                return '^';
            }
            else if (this.Blizzards.ContainsKey((point, '<')))
            {
                return '<';
            }
            else if (this.Blizzards.ContainsKey((point, 'v')))
            {
                return 'v';
            }
            else if (this.Blizzards.ContainsKey((point, '>')))
            {
                return '>';
            }
            else
            {
                char result;
                if (this.cells.TryGetValue(point, out result))
                {
                    return result;
                }
            }
            return '.';
        }
        private set
        {
            if (point.X < minX) minX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.X > maxX) maxX = point.X;
            if (point.Y > maxY) maxY = point.Y;

            this.cells[point] = value;
        }
    }

    public void Step(int step) 
    {
//        Console.WriteLine($"Minute {step}");
//        this.Print();

        var newBlizzards = new SortedList<(Point, char), bool>(new PointComparer());

        foreach (var (location, glyph) in this.Blizzards.Keys) 
        {
            Point offset, opposite;
            switch(glyph) 
            {
                case '^': offset = new Point(0, -1);  opposite = new Point(0, 1);  break;
                case '<': offset = new Point(-1, 0);  opposite = new Point(1, 0);  break;
                case 'v': offset = new Point(0, 1);   opposite = new Point(0, -1); break;
                case '>': offset = new Point(1, 0);   opposite = new Point(-1, 0); break;
                default:  throw new InvalidOperationException();
            }           
            location.Offset(offset);
            if (this[location] == '#') 
            {
                do 
                {
                    location.Offset(opposite);
                } while (this[location] != '#');
                location.Offset(offset);
            }
            newBlizzards.Add((location, glyph), true);
        }
        this.Blizzards = newBlizzards;
    }

    public IEnumerable<Point> ValidMovesFrom(Point pos)
    {
        foreach (var offset in this.MoveOffsets)
        {
            Point checkPos = pos;
            checkPos.Offset(offset);
            if (this[checkPos] == '.') {
                yield return checkPos;                
            }
        }
    }

    public void Print()
    {
        for (int y = this.minY; y <= this.maxY; y++)
        {
            for (int x = this.minX; x <= this.maxX; x++)
            {
                var point = new Point(x, y);

                Console.Write(this[point]);
            }
            Console.WriteLine();
        }
    }

    public List<Point> MoveOffsets = new List<Point>() {
        new Point(0, 0),
        new Point(0, 1),
        new Point(1, 0),
        new Point(0, -1),
        new Point(-1, 0),
    };

    private int minX = int.MaxValue;
    private int minY = int.MaxValue;
    private int maxX = int.MinValue;
    private int maxY = int.MinValue;

    private Dictionary<Point, char> cells = new Dictionary<Point, char>();

    class PointComparer : IComparer<(Point, char)>
    {
        public int Compare((Point, char) lhs, (Point, char) rhs)
        {
            int result = lhs.Item1.X.CompareTo(rhs.Item1.X);
            if (result == 0)
            {
                result = lhs.Item1.Y.CompareTo(rhs.Item1.Y);
            }
            if (result == 0)
            {
                result = lhs.Item2.CompareTo(rhs.Item2);
            }
            return result;
        }
    }
}