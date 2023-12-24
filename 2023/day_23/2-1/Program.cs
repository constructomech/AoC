using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
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

    public HashSet<Pos> AlsoVisited = new HashSet<Pos>();

    public Node? Prev { get; private set; }

    public bool PathHasVisited(Pos pos) {
        for (var current = this; current != null; current = current.Prev) {
            if (current.Position == pos || current.AlsoVisited.Contains(pos) )
            {
                return true;
            }
        }
        return false;
    }
}

class HighestPriComparer : IComparer<int> {
    public int Compare(int lhs, int rhs) {
        return rhs.CompareTo(lhs);
    }
}

static class Fun {

    static List<Pos> offsets = new List<Pos> { new Pos(1, 0), new Pos(0, 1), new Pos(-1, 0), new Pos(0, -1) };


    static List<Pos> GetChoices(String[] data, Pos currentPos, Node prevNode) {
        var result = new List<Pos>();

        foreach (var offset in offsets) {
            var targetPos = currentPos + offset;

            if (targetPos.x >= 0 && targetPos.y >= 0 && targetPos.y < data.Length && targetPos.x < data[targetPos.y].Length) {

                // If this is a movable space
                if (!prevNode.PathHasVisited(targetPos) && data[targetPos.y][targetPos.x] != '#') {
                    result.Add(targetPos);
                }
            }
        }
        return result;
    }

    public static void Run(string[] data) {
        var result = int.MinValue;

        var start = new Pos(data[0].IndexOf('.'), 0);
        var end = new Pos(data[data.Length - 1].IndexOf('.'), data.Length - 1);

        var q = new PriorityQueue<Node, int>(new HighestPriComparer());
        q.Enqueue(new Node(start, null), 0);

        while (q.Count > 0) {
            Node node;
            int pathLength;
            q.TryDequeue(out node, out pathLength);

            var currentPos = node.Position;
            var nextPathLength = pathLength;
            var choices = GetChoices(data, currentPos, node);

            while (choices.Count == 1) {
                currentPos = choices[0];
                node.AlsoVisited.Add(currentPos);
                nextPathLength++;

                if (currentPos == end) {
                    if (nextPathLength > result) {
                        Console.WriteLine($"Found a solution of length {nextPathLength}");
                        result = nextPathLength;
                        break;
                    }
                }

                choices = GetChoices(data, currentPos, node);
            }

            if (choices.Count == 0) {
                continue;
            }
            else {
                foreach (var choice in choices) {
                    var nextNode = new Node(choice, node);
                    q.Enqueue(nextNode, nextPathLength + 1);
                }
            }
        }

        Console.WriteLine($"Rusult: {result}");
    }
}