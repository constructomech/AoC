using System.IO;
using System.Collections.Generic;
using System.Drawing;

// x represents distance to the right and y represents distance down

var map = new Map();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        var lineSet = new List<Point>();

        var coordPairs = line.Split(" -> ");
        foreach (var coordPair in coordPairs)
        {
            var coords = coordPair.Split(',');
            lineSet.Add(new Point(Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1])));
        }
        map.AddTerrain(lineSet);
    }
}

int totalSand = 0;
while (map.PourSand())
{
    totalSand++;
}

Console.WriteLine("Result: {0}", totalSand + 1);


class Map
{
    public void AddTerrain(List<Point> segments)
    {
        for (int i = 0; i < segments.Count - 1; i++) {
            AddTerrainSegment(segments[i], segments[i + 1]);
        }
    }

    public char this[Point pos]
    {
        get 
        {
            if (pos.Y >= lowerBound + 2) 
            {
                return '#';
            }

            char temp;
            if (!this.map.TryGetValue(pos, out temp))
            {
                temp = '.';
            }
            return temp;
        }
    }


    // A unit of sand always falls down one step if possible. If the tile immediately below is blocked (by rock or sand), 
    //   the unit of sand attempts to instead move diagonally one step down and to the left. If that tile is blocked, the 
    //   unit of sand attempts to instead move diagonally one step down and to the right.
    public bool PourSand()
    {
        var previousPos = new Point(500, -1);
        var currentPos = entryPos;

        while (currentPos != previousPos)
        {
            previousPos = currentPos;
            currentPos = FallVerticallyUntilObstructed(previousPos);

            var downAndLeft = new Point(currentPos.X - 1, currentPos.Y + 1);
            if (this[downAndLeft] == '.') 
            {
                currentPos = downAndLeft;
            }
            else 
            {
                var downAndRight = new Point(currentPos.X + 1, currentPos.Y + 1);
                if (this[downAndRight] == '.') 
                {
                    currentPos = downAndRight;
                }
            }
        }

        // If no more sand can be poured.
        if (currentPos == entryPos) 
        {
            return false;
        }
        
        map[currentPos] = 'o';
        return true;
    }

    private Point FallVerticallyUntilObstructed(Point currentPos)
    {
        while (this[currentPos] == '.')
        {
            currentPos.Y++;
        }
        currentPos.Y--;

        return currentPos;
    }

    private void AddTerrainSegment(Point start, Point end) {

        if (start.Y > lowerBound) lowerBound = start.Y;
        if (end.Y > lowerBound) lowerBound = end.Y;

        if (start.X == end.X) {
            for (int y = Math.Min(start.Y, end.Y); y <= Math.Max(start.Y, end.Y); y++) 
            {
                map[new Point(start.X, y)] = '#';
            }
        }
        else 
        {
            for (int x = Math.Min(start.X, end.X); x <= Math.Max(start.X, end.X); x++) 
            {
                map[new Point(x, start.Y)] = '#';
            }
        }
    }

    int lowerBound = 0;

    Point entryPos = new Point(500, 0);

    Dictionary<Point, char> map = new();
}

