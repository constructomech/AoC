using System.Diagnostics;


var Numpad = Pad.Parse(flat: "*0A123456789", cols: 3, skip: '*');
var Dirpad = Pad.Parse(flat: "<v>*^A", cols: 3, skip: '*');
var Memo = new Dictionary<(Vec2, Vec2, int), long>();

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {

    var part1 = input.Select(code => GetComplexity(code, 2)).Sum();
    var part2 = input.Select(code => GetComplexity(code, 25)).Sum();

    Console.WriteLine($"Part 1: {part1}");
    Console.WriteLine($"Part 2: {part2}");
}

long GetComplexity(string sequence, int numDirPads) {
    var total = 0L;
    var start = Numpad.KeyMap['A'];

    foreach (var end in sequence.Select(key => Numpad.KeyMap[key])) {
        total += GetSequenceCost(start, end, pad: Numpad, robot: numDirPads);
        start = end;
    }

    return total * long.Parse(sequence.Substring(0, sequence.Length - 1));
}

long GetSequenceCost(Vec2 start, Vec2 end, Pad pad, int robot) {

    if (pad == Dirpad && Memo.TryGetValue((start, end, robot), out var cached)) {
        return cached;
    }

    var result = long.MaxValue;
    var queue = new Queue<State>([new State(Pos: start, Sequence: "")]);

    while (queue.Count != 0) {
        var state = queue.Dequeue();
        if (!pad.PosMap.ContainsKey(state.Pos)) {
            continue;
        }

        if (state.Pos == end) {
            var remote = pad == Dirpad;
            var target = remote ? robot - 1 : robot;

            result = long.Min(result, CalculateRobotCost($"{state.Sequence}A", robot: target));
            continue;
        }

        if (state.Pos.Y < end.Y) queue.Enqueue(state.Up());
        if (state.Pos.Y > end.Y) queue.Enqueue(state.Down());
        if (state.Pos.X < end.X) queue.Enqueue(state.Right());
        if (state.Pos.X > end.X) queue.Enqueue(state.Left());
    }

    if (pad == Dirpad) Memo[(start, end, robot)] = result;
    return result;
}

long CalculateRobotCost(string sequence, int robot) {
    if (robot == 0) {
        return sequence.Length;
    }

    var total = 0L;
    var start = Dirpad.KeyMap['A'];

    foreach (var end in sequence.Select(key => Dirpad.KeyMap[key])) {
        total += GetSequenceCost(start, end, pad: Dirpad, robot);
        start = end;
    }

    return total;
}

public record struct State(Vec2 Pos, string Sequence) {
    public State Up() => new(Pos + Vec2.Up, $"{Sequence}^");
    public State Down() => new(Pos + Vec2.Down, $"{Sequence}v");
    public State Left() => new(Pos + Vec2.Left, $"{Sequence}<");
    public State Right() => new(Pos + Vec2.Right, $"{Sequence}>");
}


public class Pad {
    public Dictionary<Vec2, char> PosMap { get; }
    public Dictionary<char, Vec2> KeyMap { get; }

    private Pad(Dictionary<Vec2, char> posMap)
    {
        PosMap = posMap;
        KeyMap = posMap.ToDictionary(
           keySelector: kvp => kvp.Value,
           elementSelector: kvp => kvp.Key);
    }

    public static Pad Parse(string flat, int cols, char skip) {
        var keys = new Dictionary<Vec2, char>();
        var rows = flat.Length / cols;

        for (var y = 0; y < rows; y++)
            for (var x = 0; x < cols; x++)
            {
                var pos = new Vec2(x, y);
                var key = flat[y * cols + x];

                if (key != skip)
                {
                    keys[pos] = key;
                }
            }

        return new Pad(keys);
    }
}

public record Vec2(int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 Up { get => new(0, 1); }

    public static Vec2 Down { get => new(0, -1); }

    public static Vec2 Left { get => new(-1, 0); }

    public static Vec2 Right { get => new(1, 0); }


    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}