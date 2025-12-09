using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var result = 0L;
    var points = new List<Vec2>();
    foreach (var line in input)
    {
        points.Add(Vec2.FromString(line));
    }

    var areas = new List<(long area, Vec2 p0, Vec2 p1)>();    
    for (var i = 0; i < points.Count; i++)
    {
        for (var j = i + 1; j < points.Count; j++)
        {
            var pi = points[i];
            var pj = points[j];
            var area = (Math.Abs((long)pi.X - pj.X) + 1) * (Math.Abs((long)pi.Y - pj.Y) + 1);
            areas.Add((area, pi, pj));
        }
    }
    areas.Sort((a, b) => b.area.CompareTo(a.area));

    var verticalSegments = new List<(int x, int fromY, int toY)>();
    var horizontalSegments = new List<(int y, int fromX, int toX)>();
    var current = points[0];
    foreach (var point in points[1..].Append(points[0]))
    {
        if (current.X == point.X)
        {
            verticalSegments.Add((current.X, Math.Min(current.Y, point.Y), Math.Max(current.Y, point.Y)));
        }
        else
        {
            Trace.Assert(current.Y == point.Y);
            horizontalSegments.Add((current.Y, Math.Min(current.X, point.X), Math.Max(current.X, point.X)));
        }
        current = point;
    }

    verticalSegments.Sort((a, b) => a.x.CompareTo(b.x));

    // Transform vertical segements to horizontal scan lines
    var scanlines = new List<(int y, int fromX, int toX)>();
    var minY = verticalSegments.Min(s => s.fromY);
    var maxY = verticalSegments.Max(s => s.toY);
    var minX = verticalSegments.Min(s => s.x);
    var maxX = verticalSegments.Max(s => s.x);

    for (var y = minY; y <= maxY; y++)
    {
        var relevantVerticalSegments = verticalSegments.Where(s => s.fromY <= y && s.toY >= y).ToList();

        bool on = true;
        var previous = relevantVerticalSegments[0];
        foreach (var verticalSegment in relevantVerticalSegments[1..])
        {
            bool isHorizontalSegment = horizontalSegments.Any(s => s.y == y && s.fromX == previous.x && s.toX == verticalSegment.x);
            if (on || isHorizontalSegment)
            {
                scanlines.Add((y, previous.x, verticalSegment.x));
            }

            if (!isHorizontalSegment)
            {
                on = !on;
            }
            previous = verticalSegment;
        }
    }

    // Merge adjacent scanline segments
    for (var i = 0; i < scanlines.Count - 1; i++)
    {
        var segment0 = scanlines[i];
        var segment1 = scanlines[i + 1];
        if (segment0.y == segment1.y && segment0.toX >= segment1.fromX)
        {
            scanlines[i] = (segment0.y, segment0.fromX, segment1.toX);
            scanlines.RemoveAt(i + 1);
        }
    }

    var maxArea = 0L;
    foreach (var scanline in scanlines)
    {
        maxArea += scanline.toX - scanline.fromX + 1;
    }

    var candidateAreas = areas.Where(a => a.area < maxArea).ToList(); // && a.area > 1410470448).ToList(); // Found from wrong answer

    // Find the highest volume area that is fully contained
    for (var idx = 0; idx < candidateAreas.Count; idx++)
    {
        var area = candidateAreas[idx];
        if (area.area <= maxArea)
        {
            if (AreaContainedInRegion(scanlines, area))
            {
                result = area.area;
                break;
            }
        }
    }

    Console.WriteLine($"Result: {result}");
}

bool AreaContainedInRegion(List<(int y, int fromX, int toX)> horizontalSegments, (long area, Vec2 p0, Vec2 p1) area)
{
    var fromY = Math.Min(area.p0.Y, area.p1.Y);
    var toY = Math.Max(area.p0.Y, area.p1.Y);
    var fromX = Math.Min(area.p0.X, area.p1.X);
    var toX = Math.Max(area.p0.X, area.p1.X);

    var segmentIdx = 0;

    for (var y = fromY; y <= toY; y++)
    {
        bool containsAreaForY = false;
        while (segmentIdx < horizontalSegments.Count && horizontalSegments[segmentIdx].y < y) segmentIdx++;

        while (segmentIdx < horizontalSegments.Count && horizontalSegments[segmentIdx].y == y)
        {
            if (horizontalSegments[segmentIdx].fromX <= fromX && horizontalSegments[segmentIdx].toX >= toX)
            {
                containsAreaForY = true;
                break;
            }
            segmentIdx++;
        }

        if (!containsAreaForY)
        {
            return false;
        }
    }

    return true;
}

public record Vec2 (int X, int Y) {
    public static Vec2 FromString(string s) {
        var parts = s.Split(',');
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) => new(a.X * b, a.Y * b);
}
