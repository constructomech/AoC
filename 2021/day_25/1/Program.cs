
var lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
        }
    }
}

CellType[,] world = new CellType[lines[0].Length, lines.Count];

for (int y = 0; y < lines.Count; y++) {
    for (int x = 0; x < lines[y].Length; x++) {
        var glyph = lines[y][x];
        switch (glyph) {
            case '>': world[x,y] = CellType.EastFacingCucumber; break;
            case 'v': world[x,y] = CellType.SouthFacingCucumber; break;
        }
    }
}

Console.WriteLine("Starting Map:");
PrintMap();
Console.WriteLine();

bool changed = true;
int step = 0;
var queue = new Queue<(CellType cellType, (int x, int y) from, (int x, int y) to)>();

do {
    changed = false;

    for (int y = 0; y < world.GetLength(1); y++) {
        for (int x = 0; x < world.GetLength(0); x++) {
            if (world[x,y] == CellType.EastFacingCucumber) {
                (int x, int y) targetPos = ((x + 1) % world.GetLength(0), y);
                if (world[targetPos.x, targetPos.y] == CellType.Empty) {
                    // Move
                    queue.Enqueue((CellType.EastFacingCucumber, (x, y), targetPos));
                    changed = true;
                }
            }
        }
    }

    ProcessQueue();

    for (int x = 0; x < world.GetLength(0); x++) {
        for (int y = 0; y < world.GetLength(1); y++) {
            if (world[x,y] == CellType.SouthFacingCucumber) {
                (int x, int y) targetPos = (x, (y + 1) % world.GetLength(1));
                if (world[targetPos.x, targetPos.y] == CellType.Empty) {
                    // Move
                    queue.Enqueue((CellType.SouthFacingCucumber, (x, y), targetPos));
                    changed = true;
                }
            }
        }
    }

    ProcessQueue();

    Console.WriteLine("Step {0}:", ++step);
    PrintMap();
    Console.WriteLine();

} while (changed);

// Find what iteration casues them to stop moving.




Console.WriteLine("EOL");

void ProcessQueue() {
    while (queue.Count > 0) {
        var item = queue.Dequeue();
        world[item.from.x, item.from.y] = CellType.Empty;
        world[item.to.x, item.to.y] = item.cellType;            
    }
}

void PrintMap() {
    for (int y = 0; y < world.GetLength(1); y++) {
        for (int x = 0; x < world.GetLength(0); x++) {
            char write = '.';
            if (world[x,y] == CellType.EastFacingCucumber) {
                write = '>';
            }
            else if (world[x,y] == CellType.SouthFacingCucumber) {
                write = 'v';
            }

            Console.Write(write);
        }
        Console.WriteLine();
    }
}

enum CellType : byte {
    Empty = 0,
    EastFacingCucumber,
    SouthFacingCucumber,
}