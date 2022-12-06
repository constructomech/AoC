using VCSKicksCollection;

List<string> lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
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
                newValue = (newValue - 1) % 9 + 1;

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

Path originPath = new Path() { x = 0, y = 0 };

var queue = new PriorityQueue<Path>();

queue.Enqueue(originPath);

(int x, int y)[] directions = new(int, int)[] { 
    (1, 0),
    (0, 1),
    (-1, 0),
    (0, -1),
};

const byte VISITED = byte.MaxValue;

while (queue.Count > 0) {
    Path current = queue.Dequeue();

    // If we happen to be at the exit, see if we should update the best path.    
    if (current.x == matrix.GetLength(0) - 1 && current.y == matrix.GetLength(1) - 1) {
        Console.WriteLine("Best score: {0}", current.cost);
        break;
    }

    if (matrix[current.x, current.y] != VISITED) {

        matrix[current.x, current.y] = VISITED;

        foreach (var direction in directions) {
            (int x, int y) targetPos = (current.x + direction.x, current.y + direction.y);

            if (targetPos.x >= 0 && targetPos.x < matrix.GetLength(0) && targetPos.y >= 0 && targetPos.y < matrix.GetLength(1)) {

                if (matrix[targetPos.x, targetPos.y] != VISITED) {
                    Path newPath = new Path() { x = targetPos.x, y = targetPos.y, parent = current, cost = current.cost + matrix[targetPos.x, targetPos.y]};
                    queue.Enqueue(newPath);
                }
            }
        }
    }
}

class Path : IComparable<Path> {

    public int CompareTo(Path other) {
        return cost.CompareTo(other.cost);
    }

    public int x;
    public int y;

    public Path parent = null;

    public int cost = 0;
}