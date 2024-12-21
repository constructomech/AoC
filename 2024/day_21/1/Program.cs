using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;

    var numPad = new FixedBoard<char>(3, 4);
    numPad[0, 0] = '7';
    numPad[1, 0] = '8';
    numPad[2, 0] = '9';
    numPad[0, 1] = '4';
    numPad[1, 1] = '5';
    numPad[2, 1] = '6';
    numPad[0, 2] = '1';
    numPad[1, 2] = '2';
    numPad[2, 2] = '3';
    numPad[0, 3] = '\0';
    numPad[1, 3] = '0';
    numPad[2, 3] = 'A';

    var dPad = new FixedBoard<char>(3,2);
    dPad[0, 0] = '\0';
    dPad[1, 0] = '^';
    dPad[2, 0] = 'A';
    dPad[0, 1] = '<';
    dPad[1, 1] = 'v';
    dPad[2, 1] = '>';

    var finalCode = new TargetAgent();
    var robot0 = new Agent(numPad, finalCode);
    var robot1 = new Agent(dPad, robot0);
    var robot2 = new Agent(dPad, robot1);

    foreach (var code in input) {
        finalCode.TargetCode = code;

        // var plainText0 = new string(robot0.TargetCode.ToArray());
        // var codeLength0 = plainText0.Length;

        // var plainText1 = new string(robot1.TargetCode.ToArray());
        // var codeLength1 = plainText1.Length;

        var plainText2 = new string(robot2.TargetCode.ToArray());
        var codeLength2 = plainText2.Length;

        var codeNum = int.Parse(code.Substring(0, code.Length - 1));
        var complexity = codeLength2 * codeNum;
        Console.WriteLine($"{codeLength2} * {codeNum}");

        result += complexity;        
    }

    Console.WriteLine($"Result: {result}");
}

public interface IAgent {
    public IEnumerable<char> TargetCode { get; }
}

public class TargetAgent : IAgent {
    public IEnumerable<char> TargetCode { get; set; }
}

public class Agent : IAgent {
    public Agent(FixedBoard<char> targetInterface, IAgent upstream) {
        this._buttons = new Dictionary<char, Vec2>();
        this._targetInterface = targetInterface;
        this._upstream = upstream;

        this._targetInterface.ForEachCell((p, c) => {
            this._buttons.Add(c, p);
        });
    }

    public IEnumerable<char> TargetCode {
        get {
            var currentPos = this._buttons['A'];
            var targetCode = new string(this._upstream.TargetCode.ToArray()); // 029A

            foreach (char targetButton in targetCode) {
                var targetPos = this._buttons[targetButton];
                var path = FindShortestValidPath(currentPos, targetPos);
                foreach (var dir in path) {
                    currentPos = currentPos + OffsetFromDirection(dir);
                    yield return CharFromDirection(dir);
                }
                yield return 'A';
            }
        }
    }

    private IEnumerable<Direction> FindShortestValidPath(Vec2 currentPosition, Vec2 targetposition) {
        var avoidPos = this._buttons['\0'];

        var dirBias = (avoidPos.Y == 0) ? new Direction[] { Direction.Down, Direction.Right, Direction.Up, Direction.Left } : 
                                          new Direction[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

        if (currentPosition.Y != avoidPos.Y) {
            dirBias = dirBias.Reverse().ToArray();
        }
 
        foreach (var dir in dirBias) {
            while (true) {
                var goalOffset = targetposition - currentPosition;
                var curMagnitude = goalOffset.MagnitudeSqared;
                var proposedNewOffset = goalOffset - OffsetFromDirection(dir);
                var proposedMagnitude = proposedNewOffset.MagnitudeSqared;

                if (proposedMagnitude >= curMagnitude) {
                    break;
                }

                yield return dir;
                currentPosition += OffsetFromDirection(dir);
            }
        }
    }

    private static Vec2 OffsetFromDirection(Direction direction) {
        return direction switch {
            Direction.Up => new(0, -1),
            Direction.Down => new(0, 1),
            Direction.Left => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => throw new Exception("Invalid direction")
        };
    }

    private static char CharFromDirection(Direction direction) {
        return direction switch {
            Direction.Up => '^',
            Direction.Down => 'v',
            Direction.Left => '<',
            Direction.Right => '>',
            _ => throw new Exception("Invalid direction")
        };
    }


    private Dictionary<char, Vec2> _buttons;
    private FixedBoard<char> _targetInterface;
    private IAgent _upstream;
}



enum Direction { Up, Down, Left, Right }


public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public double MagnitudeSqared { get { return X * X + Y * Y; } }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);

}

public class FixedBoard<T> {
    public FixedBoard(int width, int height) => _data = new T[width, height];

    public int Width { get => _data.GetLength(0); }
    public int Height { get => _data.GetLength(1); }

    public Vec2 Extents { get => new Vec2(_data.GetLength(0), _data.GetLength(1)); }
    public T this[int x, int y] { get => _data[x, y]; set => _data[x, y] = value; }
    public T this[Vec2 pos] { get => _data[pos.X, pos.Y]; set => _data[pos.X, pos.Y] = value; }

    public bool IsInBounds(int x, int y) => x >= 0 && y >= 0 && x < this.Width && y < this.Height;
    public bool IsInBounds(Vec2 pos) => pos.X >= 0 && pos.Y >= 0 && pos.X < this.Width && pos.Y < this.Height;

    public void ForEachCell(Action<int, int, T> action) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                action(x, y, this._data[x, y]);
            }
        }
    }

    public void ForEachCell(Action<Vec2, T> action) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                action(new Vec2(x, y), this._data[x, y]);
            }
        }
    }

    public void Print(Func<T, char> resovleChar) => Print(resovleChar, null, default(T));
    public void Print(Func<T, char> resovleChar, List<Vec2>? overridePositions, T? overrideValue) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                char printVal = resovleChar(_data[x, y]);
                if (overridePositions != null && overridePositions.Contains(new Vec2(x, y))) {
                    if (overrideValue == null) throw new ArgumentNullException(nameof(overrideValue));
                    printVal = resovleChar(overrideValue);
                }
                Console.Write(printVal);
            }
            Console.WriteLine();
        }
    }

    private void PopulateBoard(string[] input, Func<char, T> transform) {
        for (var y = 0; y < this.Height; y++) {
            for (var x = 0; x < this.Width; x++) {
                this._data[x, y] = transform(input[y][x]);
            }
        }
    }

    public static FixedBoard<char> FromString(string[] input) {
        var board = new FixedBoard<char>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, c => c);
        return board;
    }

    public static FixedBoard<T> FromString(string[] input, Func<char, T> transform) {
        var board = new FixedBoard<T>(input.Length > 0 ? input[0].Length : 0, input.Length);
        board.PopulateBoard(input, transform);
        return board;
    }
    
    private T[,] _data;
}
