List<string> lines = new List<string>();

int columns = 0;
int rows = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
            columns = line.Length;
            rows++;
        }
    }
}


int [,] matrix = new int[columns, rows];

int readRow = 0;
foreach (string line in lines) {
    for (int column = 0; column < line.Length; column++) {
        matrix[column, readRow] = Convert.ToInt32(line[column].ToString());
    }
    readRow++;
}

List<Basin> basins = new List<Basin>();

for (int row = 0; row < matrix.GetLength(1); row++) {
    for (int column = 0; column < matrix.GetLength(0); column++) {

        if (matrix[column, row] != 9 && !alreadyInBasin(column, row)) {
            //Console.WriteLine("Creating new basin starting with {0}, {1}", column, row);
            Basin newBasin = populateBasin(column, row);
            basins.Add(newBasin);
        }
    }
}

int answer = 1;
basins.Sort((a, b) => b.Size.CompareTo(a.Size));

for (int i = 0; i < 3; i++) {
    answer *= basins[i].Size;
}

Console.WriteLine("Answer: {0}", answer);

Basin populateBasin(int x, int y) {
    Basin newBasin = new Basin();

    Queue<Location> toProcess = new Queue<Location>();
    toProcess.Enqueue(new Location() {x = x, y = y});

    while (toProcess.Count > 0) {
        Location current = toProcess.Dequeue();

        if (matrix[current.x, current.y] < 9) {

            if (!newBasin.contains(current)) {
                newBasin.Add(current);
            }

            // Queue adjacent
            // Left
            if (current.x > 0) {
                var targetLocation = new Location() {x = current.x - 1, y = current.y};
                if (!newBasin.contains(targetLocation)) {
                    toProcess.Enqueue(targetLocation);
                }
            }

            // Right
            if (current.x < matrix.GetLength(0) - 1) {
                var targetLocation = new Location() {x = current.x + 1, y = current.y};
                if (!newBasin.contains(targetLocation)) {
                    toProcess.Enqueue(targetLocation);
                }
            }

            // Above    
            if (current.y > 0) {
                var targetLocation = new Location() {x = current.x, y = current.y - 1};
                if (!newBasin.contains(targetLocation)) {
                    toProcess.Enqueue(targetLocation);
                }
            }

            // Below
            if (current.y < matrix.GetLength(1) - 1) {
                var targetLocation = new Location() {x = current.x, y = current.y + 1};
                if (!newBasin.contains(targetLocation)) {
                    toProcess.Enqueue(targetLocation);
                }
            }
        }
    }

    return newBasin;    
}

bool alreadyInBasin(int x, int y) {
    foreach (Basin basin in basins) {
        if (basin.contains(x, y)) {
            return true;
        }
    }
    return false;
}

class Location {
    public int x;
    public int y;
}

class Basin {

    public int Size {
        get {
            return locations.Count;
        }
    }

    public void Add(Location location) {
        locations.Add(location);
    }

    public bool contains(int x, int y) {
        Location targetLocation = new Location() { x = x, y = y };
        return contains(targetLocation);
    }
    public bool contains(Location targetLocation) {
        foreach (var location in locations) {
            if (location.x == targetLocation.x && location.y == targetLocation.y) {
                return true;
            }
        }
        return false;
    }

    List<Location> locations = new List<Location>();
}