using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

public record Vec2(double x, double y) {

    public static Vec2 operator+(Vec2 lhs, Vec2 rhs) {
        return new Vec2(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Vec2 operator-(Vec2 lhs, Vec2 rhs) {
        return new Vec2(lhs.x - rhs.x, lhs.y - rhs.y);
    }
}

public class Line {
    public Vec2 start;
    public Vec2 slope;

    public Line(Vec2 start, Vec2 slope) {
        this.start = start;
        this.slope = slope;
    }

    public Vec2? Intersection(Line other) {

        var a1 = this.slope.y;
        var b1 = -this.slope.x;
        var c1 = a1 * this.start.x + b1 * this.start.y;

        var a2 = other.slope.y;
        var b2 = -other.slope.x;
        var c2 = a2 * other.start.x + b2 * other.start.y;

        var delta = a1 * b2 - a2 * b1;

        if (delta == 0) return null;

        var x = (b2 * c1 - b1 * c2) / delta;
        var y = (a1 * c2 - a2 * c1) / delta;

        return new Vec2(x, y);
    }

    public bool IsInDirection(Vec2 pointOnLine) {
        var vec = pointOnLine - this.start;

        // These vectors should have the same sign.
        return vec.x < 0 == this.slope.x < 0 && vec.y < 0 == this.slope.y < 0;
    }
}

static class Fun {

    public static void Run(string[] data) {

        var lines = new List<Line>();

        foreach (var sample in data) {
            var parts = sample.Split('@', ',').Select(x => x.Trim()).ToArray();
            // Console.WriteLine($"Parts: {string.Join(", ", parts)}");            

            var line = new Line(
                new Vec2(double.Parse(parts[0]), double.Parse(parts[1])),
                new Vec2(double.Parse(parts[3]), double.Parse(parts[4]))
            );

            lines.Add(line);
        }

        var result = 0;

        var testAreaMin = new Vec2(200000000000000, 200000000000000);
        var testAreaMax = new Vec2(400000000000000, 400000000000000);

        for (int i = 0; i < lines.Count-1; i++) {
            for (int j = i+1; j < lines.Count; j++) {
                var line1 = lines[i];
                var line2 = lines[j];

                //Console.Write($"Test [{line1.start.x},{line1.start.y} -> {line1.slope.x},{line1.slope.y}] vs. [{line2.start.x},{line2.start.y} -> {line2.slope.x},{line2.slope.y}]");

                var intersection = line1.Intersection(line2);
                if (intersection != null) {
                    var willIntersect = line1.IsInDirection(intersection);
                    var otherWillIntersect = line2.IsInDirection(intersection);
                    //Console.Write($" intersects at: {intersection.x},{intersection.y}");

                    var intersects = willIntersect && otherWillIntersect;
                    if (intersects) {
                        //Console.Write(" in the future");

                        if (intersection.x >= testAreaMin.x && intersection.x <= testAreaMax.x && intersection.y >= testAreaMin.y && intersection.y <= testAreaMax.y) {
                            //Console.Write(" and in the test area");
                            result++;
                        }
                    }
                }

                //Console.WriteLine();
            }
        }

        Console.WriteLine($"Rusult: {result}");
    }
}