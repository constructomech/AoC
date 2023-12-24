using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

public record Pos(int x, int y) {
    public static Pos operator+(Pos lhs, Pos rhs) {
        return new Pos(lhs.x + rhs.x, lhs.y + rhs.y);
    }
}

public class Node {
    public Node(Pos pos, Node prev) {
        Position = pos;
        Prev = prev;
    }

    public Pos Position { get; private set; }

    public Node? Prev { get; private set; }
}

class HighestPriComparer : IComparer<int> {
    public int Compare(int lhs, int rhs) {
        return rhs.CompareTo(lhs);
    }
}

static class Fun {

    static List<Pos> offsets = new List<Pos> { new Pos(1, 0), new Pos(0, 1), new Pos(-1, 0), new Pos(0, -1) };

    public static void Run(string[] data) {
        var result = int.MinValue;

        var start = new Pos(data[0].IndexOf('.'), 0);
        var end = new Pos(data[data.Length - 1].IndexOf('.'), data.Length - 1);

        var q = new PriorityQueue<Node, int>(new HighestPriComparer());
        q.Enqueue(new Node(start, null), 0);

        while (q.Count > 0) {
            Node node;
            int priority;
            q.TryDequeue(out node, out priority);

            if (node.Position == end) {
                int pathLength = -1;
                for (var current = node; current != null; current = current.Prev) {
                    pathLength++;
                }
                if (pathLength > result) {
                    result = pathLength;
                }
            }
            else {
                foreach (var offset in offsets) {
                    var targetPos = node.Position + offset;
                    // If position is valid
                    if (targetPos.x >= 0 && targetPos.y >= 0 && targetPos.y < data.Length && targetPos.x < data[targetPos.y].Length) {

                        // If this is a movable space
                        if (data[targetPos.y][targetPos.x] != '#') {

                            // If we haven't been here before
                            var visited = false;
                            for (var current = node; current != null; current = current.Prev) {
                                if (current.Position == targetPos) {
                                    visited = true;
                                }
                            }

                            if (!visited) {
                                var nextNode = new Node(targetPos, node);
                                q.Enqueue(nextNode, priority + 1);
                            }
                        }
                    }
                }
            }
        }

        Console.WriteLine($"Rusult: {result}");
    }
}