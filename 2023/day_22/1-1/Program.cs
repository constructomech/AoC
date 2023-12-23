using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Xml.XPath;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

public record Pos(int x, int y, int z);

public class Brick {

    public Pos Corner1 { get; set; }
    public Pos Corner2 { get; set; }

    public IEnumerable<Pos> GetPositions() {
        if (IsXBrick) {
            var minX = Math.Min(Corner1.x, Corner2.x);
            var maxX = Math.Max(Corner1.x, Corner2.x);
            for (int x = minX; x <= maxX; x++) {
                yield return new Pos(x, Corner1.y, Corner1.z);
            }
        }
        else if (IsYBrick) {
            var minY = Math.Min(Corner1.y, Corner2.y);
            var maxY = Math.Max(Corner1.y, Corner2.y);
            for (int y = minY; y <= maxY; y++) {
                yield return new Pos(Corner1.x, y, Corner1.z);
            }
        }
        else if (IsZBrick) {
            var minZ = Math.Min(Corner1.z, Corner2.z);
            var maxZ = Math.Max(Corner1.z, Corner2.z);
            for (int z = minZ; z <= maxZ; z++) {
                yield return new Pos(Corner1.x, Corner1.y, z);
            }
        }
        else {  // Single block
            yield return new Pos(Corner1.x, Corner1.y, Corner1.z);
        }
    }

    public Brick MoveDown() {
        Brick result = new Brick();
        result.Corner1 = new Pos(Corner1.x, Corner1.y, Corner1.z - 1);
        result.Corner2 = new Pos(Corner2.x, Corner2.y, Corner2.z - 1);
        return result;
    }

    public bool IsXBrick {
        get {
            return Corner1.x != Corner2.x;
        }
    }

    public bool IsYBrick {
        get {
            return Corner1.y != Corner2.y;
        }
    }

    public bool IsZBrick {
        get {
            return Corner1.z != Corner2.z;
        }
    }
}

public class Field {
    public void addBrick(Brick brick) {
        _bricks.Add(brick);
        
        var brickPositions = brick.GetPositions();
        foreach (var brickPosition in brickPositions) {
            _occupiedPositions.Add(brickPosition);
        }
    }

    private Brick MoveBrickDown(Brick brick) {
        foreach (var brickPosition in brick.GetPositions()) {
            _occupiedPositions.Remove(brickPosition);
        }

        var movedBrick = brick.MoveDown();

        foreach (var brickPosition in movedBrick.GetPositions()) {
            _occupiedPositions.Add(brickPosition);
        }

        return movedBrick;
    }

    public void Compress() {
        int atRestIndex = -1;

        // First sort by min Z
        _bricks.Sort((a, b) => Math.Min(a.Corner1.z, a.Corner2.z).CompareTo(Math.Min(b.Corner1.z, b.Corner2.z)));

        while (true) {
            var i = atRestIndex + 1;
            if (i >= _bricks.Count) {
                break;
            }

            if (IsAtRest(_bricks[i])) {
                atRestIndex++;
            }
            else {
                // Move down as long as no target positions are occupied
                do {
                    _bricks[i] = MoveBrickDown(_bricks[i]);
                } while (!IsAtRest(_bricks[i]));
                atRestIndex = 0;
            }
        }
    }

    private bool BrickCanFall(SetMinusSet<Pos> occupied, Brick brick) {
        var testPositions = GetTestPositions(brick.GetPositions());

        foreach (var testPosition in testPositions) {
            if (testPosition.z == 0 || occupied.Contains(testPosition)) {
                return false;
            }
        }
        return true;
    }

    public int FindRemovableCount() {
        var result = 0;

        foreach (var brick in _bricks) {

            bool anyOtherBrickCanFall = false;
            foreach (var otherBrick in _bricks) {

                if (brick != otherBrick) {
                    var occupiedPositions = getOccupiedPositions(brick, otherBrick);

                    if (BrickCanFall(occupiedPositions, otherBrick)) {
                        anyOtherBrickCanFall = true;
                        break;
                    }
                }
            }

            if (!anyOtherBrickCanFall) {
                result++;
            }
        }

        return result;
    }

    private IEnumerable<Pos> GetTestPositions(IEnumerable<Pos> brickPositions) {
        foreach (var brickPosition in brickPositions) {
            yield return new Pos(brickPosition.x, brickPosition.y, brickPosition.z - 1);
        }
    }

    public bool IsAtRest(Brick brick) {
        IEnumerable<Pos> testPositions = null;
        if (brick.IsZBrick) {
            var minZ = Math.Min(brick.Corner1.z, brick.Corner2.z);
            testPositions = new List<Pos>() { new Pos(brick.Corner1.x, brick.Corner1.y, minZ - 1) };
        }
        else {
            testPositions = GetTestPositions(brick.GetPositions());
        }

        var occupiedPositions = getOccupiedPositions(brick);

        foreach (var testPosition in testPositions) {
            if (testPosition.z == 0) {
                return true;
            } else if (occupiedPositions.Contains(testPosition)) {
                return true;
            }
        }

        return false;
    }

    private SetMinusSet<Pos> getOccupiedPositions(params Brick[] exceptionBricks) {
        var exceptionPositions = new HashSet<Pos>();
        foreach (var exceptionBrick in exceptionBricks) {

            if (exceptionBrick != null) {
                var exceptionBrickPositions = exceptionBrick.GetPositions();

                foreach (var exceptionBrickPosition in exceptionBrickPositions) {
                    exceptionPositions.Add(exceptionBrickPosition);
                }
            }
        }

        return new SetMinusSet<Pos>(_occupiedPositions, exceptionPositions);
    }


    private List<Brick> _bricks = new List<Brick>();
    private HashSet<Pos> _occupiedPositions = new HashSet<Pos>();
}

class SetMinusSet<T> {
    public SetMinusSet(HashSet<T> main, HashSet<T> minus) {
        this.main = main;
        this.minus = minus;
    }

    public bool Contains(T item) {
        return main.Contains(item) && !minus.Contains(item);
    }

    private HashSet<T> main;
    private HashSet<T> minus;
}

static class Fun {
    public static void Run(string[] input) {

        var field = new Field();
        foreach (var line in input)
        {
            var parts = line.Split('~');
            var corner1Strs = parts[0].Split(',');
            var corner2Strs = parts[1].Split(',');
            var corner1 = new Pos(Convert.ToInt32(corner1Strs[0]), Convert.ToInt32(corner1Strs[1]), Convert.ToInt32(corner1Strs[2]));
            var corner2 = new Pos(Convert.ToInt32(corner2Strs[0]), Convert.ToInt32(corner2Strs[1]), Convert.ToInt32(corner2Strs[2]));

            var brick = new Brick { Corner1 = corner1, Corner2 = corner2 };
            field.addBrick(brick);
        }

        field.Compress();

        var result = field.FindRemovableCount();

        Console.WriteLine("Result: {0}", result);
    }
}

