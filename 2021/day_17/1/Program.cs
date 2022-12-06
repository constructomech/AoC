int xLow = 0, xHigh = 0, yLow = 0, yHigh = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        //target area: x=20..30, y=-10..-5
        string? line = reader.ReadLine();
        if (line != null) {
            string[] parts = line.Split(": ");
            string[] ranges = parts[1].Split(", ");
            (xLow, xHigh) = parseRange(ranges[0]);
            (yLow, yHigh) = parseRange(ranges[1]);
        }
    }
}

Console.WriteLine("x: ({0}..{1}), y: ({2}..{3})", xLow, xHigh, yLow, yHigh);

int maxHeight = 0;
int countOfSolutions = 0;

//int x = 7, y = -1;
//int x = 6, y = 0;

for (int x = -1000; x < 1000; x++) {
    for (int y = -1000; y < 1000; y++) {
        int height;
        if (simulateProjectile(x, y, out height)) {
            countOfSolutions++;

            if (height > maxHeight) {
                maxHeight = height;
            }
            Console.WriteLine("Found solution: ({0}, {1}) {2} height", x, y, height);
            //Console.WriteLine("{0}, {1}", x, y);
        }
    }
}

Console.WriteLine("Solutions: {0}, Max height: {1}", countOfSolutions, maxHeight);

bool simulateProjectile(int xVel, int yVel, out int maxHeight) {
    int xPos = 0, yPos = 0;
    bool past = false;
    maxHeight = yPos;

    // Console.WriteLine("Step 0: ({0}, {1})", xPos, yPos);
    // int step = 1;

    while (!past) {
        xPos += xVel;
        yPos += yVel;

        // Console.WriteLine("Step {0}: ({1}, {2})", step++, xPos, yPos);

        if (yPos > maxHeight) {
            maxHeight = yPos;
        }

        if (!(xPos < xLow || xPos > xHigh || yPos < yLow || yPos > yHigh)) {
            // On target
            return true;
        }

        bool yPast = yPos < yLow;
        bool xPast = (xLow < 0) ? xPos < xHigh : xPos > xHigh;
        past = (xPast || yPast);

        xVel -= Math.Sign(xVel) * 1;
        yVel -= 1;
    }
    return false;
}

(int low, int high) parseRange(string range) {
    var parts = range.Split("=");
    parts = parts[1].Split("..");
    int left = Convert.ToInt32(parts[0]);
    int right = Convert.ToInt32(parts[1]);
    return (Math.Min(left, right), Math.Max(left, right));
}
