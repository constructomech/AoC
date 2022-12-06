
List<string> lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
        }
    }
}

byte[,] matrix = new byte[lines[0].Length, lines.Count];

for (int x = 0; x < matrix.GetLength(0); x++) {
    for (int y = 0; y < matrix.GetLength(1); y++) {
        matrix[x,y] = Convert.ToByte(lines[y][x].ToString());
    }
}

int[,] bestScores = new int[matrix.GetLength(0), matrix.GetLength(1)];
for (int x = 0; x < bestScores.GetLength(0); x++) {
    for (int y = 0; y < bestScores.GetLength(1); y++) {
        bestScores[x,y] = int.MaxValue;
    }
}

int bestScore = int.MaxValue;
Path bestPath = null;

Path originPath = new Path() { x = 0, y = 0 };

Stack<Path> stack = new Stack<Path>();
stack.Push(originPath);

List<Tuple<int, int>> directions = new List<Tuple<int, int>>() { 
    new Tuple<int, int>(1, 0),
    new Tuple<int, int>(-1, 0),
    new Tuple<int, int>(0, 1),
    new Tuple<int, int>(0, -1),
};

while (stack.Count > 0) {
    Path current = stack.Pop();

    // int parentCost = current.parent != null ? current.parent.cost : 0;
    // current.cost = parentCost + matrix[current.x, current.y];

    if (current.cost < bestScore && current.cost < bestScores[current.x, current.y]) {
        bestScores[current.x, current.y] = current.cost;

        // If we happen to be at the exit, see if we should update the best path.    
        if (current.x == matrix.GetLength(0) - 1 && current.y == matrix.GetLength(1) - 1) {

            bestScore = current.cost;
            bestPath = current;

            Console.WriteLine("New best score: {0}", bestScore);
        }
        else 
        {
            foreach (var direction in directions) {
                int x = current.x + direction.Item1;
                int y = current.y + direction.Item2;
                if (x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1)) {
                    Path newPath = new Path() { x = x, y = y, parent = current, cost = current.cost + matrix[x, y]};
                    stack.Push(newPath);
                }
            }
        }
    }
}

Console.WriteLine("Best Score: {0}", bestScore);

// Path path = bestPath;
// while (path != null) {
//     Console.WriteLine("<- ({0}, {1}): {2}", path.x, path.y, path.cost);
//     path = path.parent;
// }

class Path {

    public int x;
    public int y;

    public Path parent = null;

    public int cost = 0;

}