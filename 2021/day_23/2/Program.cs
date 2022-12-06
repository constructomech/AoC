using VCSKicksCollection;

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

List<string> goalMapSource = new List<string>() { 
"#############",
"#...........#",
"###A#B#C#D###",
"  #A#B#C#D#",
"  #A#B#C#D#",
"  #A#B#C#D#",
"  #########"
};

char[,] startingMap = ParseMap(lines);
char[,] goalMap = ParseMap(goalMapSource);
int iter = 0;

var result = Solve(startingMap, goalMap, Heuristic);

if (result != null) {
    Console.WriteLine();
    for (int step = 0; step < result.Count; step++) {
        Console.WriteLine("Step {0}", step);
        Print(result[step]);
        Console.WriteLine("Cost: {0}", State.fScore[result[step]]);
    }
}


List<char[,]> Solve(char[,] start, char[,] goal, Func<char[,], int> h) {
    
    var result = new List<char[,]>();

    Console.WriteLine("Start:");
    Print(start);

    Console.WriteLine("Goal:");
    Print(goal);

    State.openSet.Enqueue(new QueueItem() { Map = start });

    State.gScore[start] = 0;

    State.fScore[start] = h(start);

    while (State.openSet.Count > 0) {
        var item = State.openSet.Dequeue();
        var current = item.Map;

        if (iter++ % 10000 == 0) {
            Console.WriteLine("Current:");
            Print(current);
            Console.WriteLine("Cost: {0}", item.FScore);
        }

        if (IsEqual(current, goal)) {
            return ReconstructPath(State.cameFrom, current);
        }

        foreach (var neighbor in GetNeighbors(current)) {
            
            // Console.WriteLine("Neighbor:");
            // Print(neighbor);

            int tentativeGScore = GetGScore(current) + ComputeMoveCost(current, neighbor);
            // Console.WriteLine("Tentative Score: {0}", tentativeGScore);
            if (tentativeGScore < GetGScore(neighbor)) {

                State.cameFrom[neighbor] = current;
                State.gScore[neighbor] = tentativeGScore;
                State.fScore[neighbor] = tentativeGScore + h(neighbor);
                // Console.WriteLine("FScore: {0}", tentativeGScore + h(neighbor));

                // If neighbor not in open set (not implemented)
                State.openSet.Enqueue(new QueueItem() { Map = neighbor });
            }
        }
    }

    return result;
}

IEnumerable<char[,]> GetNeighbors(char[,] map) {

    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            if (IsPlayer(map[x, y]) && !PackedInCorrectRoom(map, x, y)) {
                if (y == 1) { // is in the hallway
                    // return best legal move to target room
                    int roomX = GetTargetRoomX(map[x, y]);
                    for (int roomY = 5; roomY >= 2; roomY--) {
                        if (CanPathBetween(map, (x, y), (roomX, roomY))) {
                            char[,] copy = Copy(map);
                            copy[x, y] = '.';
                            copy[roomX, roomY] = map[x, y];
                            yield return copy;
                            break;
                        }
                    }
                } else { // is in a room
                    // return all legal moves to hallway positions
                    int hallwayY = 1;
                    for (int hallwayX = 1; hallwayX <= 11; hallwayX++) {
                        // Don't block rooms
                        if (hallwayX != 3 && hallwayX != 5 && hallwayX != 7 && hallwayX != 9) {
                            if (CanPathBetween(map, (x, y), (hallwayX, hallwayY))) {
                                char[,] copy = Copy(map);
                                copy[x, y] = '.';
                                copy[hallwayX, hallwayY] = map[x, y];
                                yield return copy;
                            }
                        }
                    }
                }
            }
        }
    }
}

bool PackedInCorrectRoom(char[,] map, int x, int y) {
    char player = map[x, y];
    if (GetTargetRoomX(player) == x) {
        for (int roomY = 5; roomY >= Math.Max(y, 2); roomY--) {
            if (y == roomY) {
                return true;
            }
            else if (map[x, roomY] != map[x, y]) { // Different player type
                return false;                
            }
        }
    }
    return false;
}

bool CanPathBetween(char[,] map, (int x, int y) start, (int x, int y) end) {
    foreach ((int x, int y) in GetPathSteps(map, start, end)) {
        if (map[x, y] != '.') {
            return false;
        }
    }
    return true;
}

IEnumerable<(int x, int y)> GetPathSteps(char[,] map, (int x, int y) startLocation, (int x, int y) endLocation) {
    if (startLocation.y != 1) {
        // Room to hallway
        for (int y = startLocation.y - 1; y >= endLocation.y; y--) {
            yield return (startLocation.x, y);
        }

        bool cont = (startLocation.x != endLocation.x);
        int x = startLocation.x;
        int inc = (startLocation.x > endLocation.x) ? -1 : 1;
        while (cont) {
            x += inc;

            yield return (x, endLocation.y);
            cont = (x != endLocation.x);
        }
    }
    else {
        // Hallway to room
        bool cont = (startLocation.x != endLocation.x);
        int x = startLocation.x;
        int inc = (startLocation.x > endLocation.x) ? -1 : 1;
        while (cont) {
            x += inc;

            yield return ((x, endLocation.y));
            cont = (x != endLocation.x);
        }
        for (int y = startLocation.y; y <= endLocation.y; y++) {
            yield return (startLocation.x, y);
        }
    }
}

int ComputeMoveCost(char[,] before, char[,] after) {
    char player = ' ';
    (int x, int y) startPosition = (0, 0), endPosition = (0, 0), move = (0, 0);

    for (int y = 0; y < before.GetLength(1); y++) {
        for (int x = 0; x < before.GetLength(0); x++) {
            if (before[x, y] != after[x, y]) {
                if (IsPlayer(before[x,y])) {
                    player = before[x, y];
                    startPosition = (x, y);
                } 
                if (IsPlayer(after[x,y])) {
                    endPosition = (x, y);
                }
            }
        }
    }

    move = endPosition;
    move.x -= startPosition.x;
    move.y -= startPosition.y;

    return GetCostToMove(player) * (Math.Abs(move.x) + Math.Abs(move.y));
}

void Print(char[,] map) {
    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            Console.Write(map[x, y]);
        }
        Console.WriteLine();
    }
}

char[,] Copy(char[,] map) {
    char[,] copy = new char[map.GetLength(0), map.GetLength(1)];
    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            copy[x, y] = map[x, y];
        }
    }
    return copy;
}

bool IsEqual(char[,] m1, char[,] m2) {
    for (int y = 0; y < m1.GetLength(1); y++) {
        for (int x = 0; x < m1.GetLength(0); x++) {
            if (m1[x, y] != m2[x, y]) {
                return false;
            }
        }
    }
    return true;
}

int Heuristic(char[,] map) {
    // We'll (under)estimate the cost as moving each incorrect placement along it's shortest path
    //  without regard for others in the way.
    var needToMove = new List<(char player, (int x, int y) current)>();

    for (int y = 0; y < map.GetLength(1); y++) {
        for (int x = 0; x < map.GetLength(0); x++) {
            if (IsPlayer(map[x, y]) && !IsInCorrectRoom(map[x, y], x, y)) {
                needToMove.Add((map[x, y], (x, y)));
            }
        }
    }

    int cost = 0;
    foreach((char player, (int x, int y) current) in needToMove) {
        int moves = Math.Abs(GetTargetRoomX(player) - current.x);
        moves += (current.y - 1) * 2;   // Estimate vertical distance as to the hallway and back.
        cost += moves * GetCostToMove(player);
    }
    return cost;
}

int GetGScore(char[,] map) {
    int result;
    if (State.gScore.TryGetValue(map, out result)) {
        return result;
    }
    else {
        return int.MaxValue;
    }
}

int GetCostToMove(char c) {
    switch(c) {
        case 'A': return 1;
        case 'B': return 10;
        case 'C': return 100;
        case 'D': return 1000;
    }
    throw new InvalidDataException();
}

bool IsPlayer(char c) {
    switch (c) {
        case 'A':
        case 'B':
        case 'C':
        case 'D':
            return true;
    }
    return false;
}

bool IsInCorrectRoom(char c, int x, int y) {
    int roomX = GetTargetRoomX(c);
    return x == roomX && y > 1;
}

int GetTargetRoomX(char c) {
    switch (c) {
        case 'A': return 3;
        case 'B': return 5;
        case 'C': return 7;
        case 'D': return 9;
    }
    throw new InvalidDataException();
}


List<char[,]> ReconstructPath(Dictionary<char[,], char[,]> cameFrom, char[,] current) {
    List<char[,]> totalPath = new List<char[,]>();
    totalPath.Add(current);
    while (cameFrom.Keys.Contains(current)) {
        current = cameFrom[current];
        totalPath.Insert(0, current);
    }
    return totalPath;
}

char[,] ParseMap(List<string> lines) {
    int maxLength = lines.Max(s => s.Length);
    var result = new char[maxLength, lines.Count];
    for (int y = 0; y < lines.Count; y++) {
        for (int x = 0; x < maxLength; x++) {
            if (x < lines[y].Length) {
                result[x, y] = lines[y][x];
            } else {
                result[x, y] = ' ';
            }
        }
    }
    return result;
}

class State {
    public static PriorityQueue<QueueItem> openSet = new PriorityQueue<QueueItem>();
    public static Dictionary<char[,], char[,]> cameFrom = new Dictionary<char[,], char[,]>();
    public static Dictionary<char[,], int> gScore = new Dictionary<char[,], int>();
    public static Dictionary<char[,], int> fScore = new Dictionary<char[,], int>();
}
class QueueItem : IComparable<QueueItem> {
    public int CompareTo(QueueItem other) {
        return FScore.CompareTo(other.FScore);
    }

    public int FScore { 
        get {
            int result = int.MaxValue;
            State.fScore.TryGetValue(Map, out result);
            return result;
        }
    }

    public char[,] Map { get; set; }
}