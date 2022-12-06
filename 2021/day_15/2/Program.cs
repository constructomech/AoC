
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

byte[,] tile = new byte[lines[0].Length, lines.Count];

for (int x = 0; x < tile.GetLength(0); x++) {
    for (int y = 0; y < tile.GetLength(1); y++) {
        tile[x,y] = Convert.ToByte(lines[y][x].ToString());
    }
}

byte[,] matrix = new byte[tile.GetLength(0) * 5, tile.GetLength(1) * 5];

for (int tileX = 0; tileX < 5; tileX++) {
    for (int tileY = 0; tileY < 5; tileY++) {

        for (int x = 0; x < tile.GetLength(0); x++) {
            for (int y = 0; y < tile.GetLength(1); y++) {
                int increase = tileX + tileY;

                int newValue = tile[x,y] + increase;
                while (newValue > 9) newValue -= 9;

                if (newValue == 0) {
                    Console.WriteLine("whoops");
                }

                matrix[tileX * tile.GetLength(0) + x, tileY * tile.GetLength(1) + y] = (byte)newValue;
            }
        }
    }
}

// for (int y = 0; y < matrix.GetLength(1); y++) {
//     for (int x = 0; x < matrix.GetLength(0); x++) {
//         Console.Write(matrix[x,y]);
//     }
//     Console.WriteLine();
// }

int[,] bestScores = new int[matrix.GetLength(0), matrix.GetLength(1)];
for (int x = 0; x < bestScores.GetLength(0); x++) {
    for (int y = 0; y < bestScores.GetLength(1); y++) {
        bestScores[x,y] = int.MaxValue;
    }
}

int extreme = 0;

int bestScore = int.MaxValue;
Path bestPath = null;

Path originPath = new Path() { x = 0, y = 0 };

Queue<Path> queue = new Queue<Path>();
queue.Enqueue(originPath);

List<Tuple<int, int>> directions = new List<Tuple<int, int>>() { 
    new Tuple<int, int>(1, 0),
    new Tuple<int, int>(0, 1),
    new Tuple<int, int>(-1, 0),
    new Tuple<int, int>(0, -1),
};

while (queue.Count > 0) {
    Path current = queue.Dequeue();

    int currentExtreme = current.x + current.y;
    if (currentExtreme > extreme) {
        Console.WriteLine("New extreme: {0} at ({1}, {2})", currentExtreme, current.x, current.y);
        extreme = currentExtreme;
    }

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
                    queue.Enqueue(newPath);
                }
            }
        }
    }
}

Console.WriteLine("Best Score: {0}", bestScore);

class Path {

    public int x;
    public int y;

    public Path parent = null;

    public int cost = 0;

}