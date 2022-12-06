using VCSKicksCollection;

var lines = new List<string>();
int maxLength = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {            
            maxLength = Math.Max(maxLength, line.Length);
            lines.Add(line);
        }
    }
}

Board board = new Board(lines, maxLength);
Path? path = board.Solve();
if (path != null) {
    board.Print(path);
} else {
    Console.WriteLine("No solution found.");
}

class Cell {
    public bool isWall { get; set;}

    public bool isHalway { get; set; }

    public bool isRoom { get; set; }
}

class Player {
    public char Name { get; set; }
}

class Path : IComparable<Path> {

    public int CompareTo(Path other) {
        return cost.CompareTo(other.cost);
    }

    public Path? Move(Board board, Player player, (int x, int y) newLocation) {
        Path result = null;

        if (IsValidMove(board, player, playerLocations[player], newLocation)) {
            result = new Path() { parent = this };
            foreach (var item in playerLocations) {
                if (item.Key == player) {
                    result.playerLocations.Add(item.Key, newLocation);
                }
                else {
                    result.playerLocations.Add(item.Key, item.Value);
                }
            }

            (int x, int y) move = newLocation;
            move.x -= playerLocations[player].x;
            move.y -= playerLocations[player].y;

            result.cost = cost + getCostToMove(player) * (Math.Abs(move.x) + Math.Abs(move.y));
        }
        return result;
    }

    public bool IsValidMove(Board board, Player targetPlayer, (int x, int y) startLocation, (int x, int y) targetLocation) {

        // Not valid to move zero spaces
        if (targetLocation == startLocation) {
            return false;
        }

        if (GetPlayerAtPosition(playerLocations, targetLocation) != null) {
            return false;
        }

        // Players never stop on the space immediatley outside any room.
        if (board.cells[targetLocation.x, targetLocation.y].isHalway && board.cells[targetLocation.x, targetLocation.y + 1].isRoom) {
            return false;
        }

        // If the player can move into it's target room, all other moves are invalid.
        // (int x, int y) roomDesinationLocaiton1 = (Board.GetPlayerTargetRoomX(targetPlayer), 3);
        // Player? playerInBestRoomLocation = GetPlayerAtPosition(playerLocations, roomDesinationLocaiton1);
        // if (playerInBestRoomLocation == null) {
        //     if (targetLocation != roomDesinationLocaiton1) {
        //         if (!IsPathObstructed(board, targetPlayer, startLocation, roomDesinationLocaiton1)) {
        //             return false;
        //         }
        //     }
        // } else if (playerInBestRoomLocation.Name == targetPlayer.Name) {
        //     (int x, int y) roomDesinationLocaiton2 = (Board.GetPlayerTargetRoomX(targetPlayer), 2);
        //     if (GetPlayerAtPosition(playerLocations, roomDesinationLocaiton2) == null) {
        //         if (targetLocation != roomDesinationLocaiton2) {
        //             if (!IsPathObstructed(board, targetPlayer, startLocation, roomDesinationLocaiton2)) {
        //                 return false;
        //             }
        //         }
        //     }
        // }

        // Check if the path from current location to new location is unobstructed
        if (IsPathObstructed(board, targetPlayer, startLocation, targetLocation)) {
            return false;
        }

        var locations = GetPathSteps(board, startLocation, targetLocation);
        foreach (var location in locations) {
            Player? playerAtLocation = GetPlayerAtPosition(playerLocations, location);
            if (playerAtLocation != null && playerAtLocation != targetPlayer) {
                return false;
            }
        }

        // Verify if there is any player already in the room, it's of the same name.
        Cell startRoom = board.cells[startLocation.x, startLocation.y];
        Cell targetRoom = board.cells[targetLocation.x, targetLocation.y];

        if (startRoom.isHalway && targetRoom.isRoom) {

            // Verify if the target is an empty room, that we're targetting the farthest in cell.
            (int x, int y) oneBelow = (targetLocation.x, targetLocation.y + 1);
            if (board.cells[oneBelow.x, oneBelow.y].isRoom && !playerLocations.Values.Contains(oneBelow)) {
                return false;
            }

            // Verify if there is any player already in the room, it's of the same name.
            var checkOffsets = new (int x, int y)[] { (0, 1), (0, -1) };
            foreach (var offset in checkOffsets) {
                var checkLocation = targetLocation;
                checkLocation.x += offset.x;
                checkLocation.y += offset.y;
                Player? playerAtLocation = GetPlayerAtPosition(playerLocations, checkLocation);
                if (playerAtLocation != null && board.rooms.Contains(checkLocation)) {
                    if (playerAtLocation.Name != targetPlayer.Name) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool IsPathObstructed(Board board, Player player, (int x, int y) startLocation, (int x, int y) endLocation) {
        var locations = GetPathSteps(board, startLocation, endLocation);
        foreach (var location in locations) {
            Player? playerAtLocation = GetPlayerAtPosition(playerLocations, location);
            if (playerAtLocation != null && playerAtLocation != player) {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<(int x, int y)> GetPathSteps(Board board, (int x, int y) startLocation, (int x, int y) endLocation) {
        var result = new List<(int x, int y)>();
        if (board.rooms.Contains(startLocation)) {
            // Room to hallway
            for (int y = startLocation.y; y >= endLocation.y; y--) {
                result.Add((startLocation.x, y));
            }

            bool cont = (startLocation.x != endLocation.x);
            int x = startLocation.x;
            int inc = (startLocation.x > endLocation.x) ? -1 : 1;
            while (cont) {
                x += inc;

                result.Add((x, endLocation.y));
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

                result.Add((x, endLocation.y));
                cont = (x != endLocation.x);
            }
            for (int y = startLocation.y; y <= endLocation.y; y++) {
                result.Add((startLocation.x, y));
            }
        }
        return result;
    }

    private static (Player player, (int x, int y) previousLocation, (int x, int y) nextLocation) GetMove(Path from, Path to) {
        foreach (var player in from.playerLocations.Keys) {
            var previousLocation = from.playerLocations[player];
            var nextLocation = to.playerLocations[player];
            if (nextLocation != previousLocation) {
                // Found the move
                return (player, previousLocation, nextLocation);
            }
        }
        throw new InvalidDataException();
    }

    private static int getCostToMove(Player player) {        
        switch(player.Name) {
            case 'A': return 1;
            case 'B': return 10;
            case 'C': return 100;
            case 'D': return 1000;
        }
        throw new InvalidDataException();
    }

    public static Player? GetPlayerAtPosition(Dictionary<Player, (int x, int y)> playerLocations, (int x, int y) checkLocation) {
        foreach (var item in playerLocations) {
            if (item.Value == checkLocation) return item.Key;
        }
        return null;
    }

    public Dictionary<Player, (int x, int y)> playerLocations = new Dictionary<Player, (int x, int y)>();

    public int cost;

    public Path? parent = null;
}

class Board {
    public Board(List<string> lines, int maxLength) {
        int y = 0;
        this.cells = new Cell[maxLength, lines.Count];

        foreach (var line in lines) {

            for (int x = 0; x < maxLength; x++) {
                Cell cell = new Cell();

                if (x >= line.Length) {
                    cell.isWall = true;
                }
                else {
                    switch(line[x]) {
                        case ' ':
                        case '#':
                            cell.isWall = true;
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                            Player player = new Player() { Name = line[x] };
                            cell.isRoom = true;
                            rooms.Add((x, y));
                            playerStartingLocations.Add(player, (x, y));
                            break;
                        case '.':
                            cell.isHalway = true;
                            hallways.Add((x, y));
                            break;
                    }
                }
                cells[x, y] = cell;
            }
            y++;
        }
    }

    public Path Solve() {
        Path winner = null;

        PriorityQueue<Path> queue = new PriorityQueue<Path>();
        Path root = new Path() { playerLocations = playerStartingLocations };
        queue.Enqueue(root);

        long iter = 0;
        while (queue.Count > 0) {
            Path path = queue.Dequeue();

            iter++;
            if (iter % 10000 == 0) {
                if (path.parent != null) {
                    Console.WriteLine("Before:");
                    Print(path.parent);
                    Console.WriteLine("After:");
                }

                Print(path);
                Console.WriteLine("Queue contains {0} items.", queue.Count);
                Console.WriteLine();
            }
           
            // If at goal state
            if (IsAtGoalState(path.playerLocations)) {
                winner = path;
                break;
            }

            var optimalMoves = GetOptimalMoves(path);
            if (optimalMoves.Count > 0) {
                foreach (var move in optimalMoves) {
                    queue.Enqueue(move);
                }
            }
            else {
                // Add children
                foreach (var player in playerStartingLocations.Keys) {

                    // If player is elgible to move
                    if (IsPlayerEligibleToMove(path, player)) {

                        // Get potential destinations for this player
                        var desintations = GetPlayerPotentialDestinations(path.playerLocations, player);
                        foreach (var destination in desintations) {

                            Path? move = path.Move(this, player, destination);
                            if (move != null) { // is valid
                                queue.Enqueue(move);
                            }
                        }
                    }
                }
            }
        }

        return winner;
    }

    public List<Path> GetOptimalMoves(Path path) {
        List<Path> result = new List<Path>();

        foreach (Player player in path.playerLocations.Keys) {

            (int x, int y) roomDesinationLocaiton1 = (Board.GetPlayerTargetRoomX(player), 3);
            if (path.playerLocations[player] != roomDesinationLocaiton1) {
                Path? optimalPath = path.Move(this, player, roomDesinationLocaiton1);
                if (optimalPath != null) {
                    result.Add(optimalPath);
                }
                else {
                    (int x, int y) roomDesinationLocaiton2 = (Board.GetPlayerTargetRoomX(player), 2);
                    optimalPath = path.Move(this, player, roomDesinationLocaiton2);
                    if (optimalPath != null) {
                        result.Add(optimalPath);
                    }
                }
            }
        }
        return result;
    }

    public bool IsPlayerEligibleToMove(Path path, Player player) {
        if (hallways.Contains(path.playerLocations[player])) {
            return true;
        }

        // If the player is already in the correct room (packed) they are not elgible to move
        if (path.playerLocations[player].x == GetPlayerTargetRoomX(player)) {
            var playerLocation = path.playerLocations[player];

            // More self hate
            if (playerLocation.y == 3) {
                return false;
            }
            // else if (playerLocation.y == 2) {
            //     var otherLocation = playerLocation;
            //     playerLocation.y += 1;
            //     Player? playerAtLocation = Path.GetPlayerAtPosition(path.playerLocations, otherLocation);
            //     if (playerAtLocation != null && playerAtLocation.Name == player.Name) {
            //         return false;
            //     }
            // }
        }

        // Check if this player has already moved twice (out to hallway, back to room)
        int moves = 0;
        var location = path.playerLocations[player];
        Path? current = path;
        while (current != null) {
            if (path.playerLocations[player] != location) {
                moves++;
                location = path.playerLocations[player];
            }
            current = current.parent;
        }
        return (moves < 2);
    }

    public List<(int x, int y)> GetPlayerPotentialDestinations(Dictionary<Player, (int x, int y)> playerLocations, Player player) {
        var result = new List<(int x, int y)>();

        // If the player is in a room, select moves to a hallway (returns any unoccupied hallway position)
        if (rooms.Contains(playerLocations[player])) {
            foreach (var hallwayLocation in hallways) {
                if (!playerLocations.Values.Contains(hallwayLocation)) {
                    result.Add(hallwayLocation);
                }
            }
        }

        // If the player is in a hallway, select moves to a room (returns any room position that is valid as a target for this player)
        int targetX = GetPlayerTargetRoomX(player);
        if (hallways.Contains(playerLocations[player])) {
            foreach (var roomLocation in rooms) {
                if (roomLocation.x == targetX && !playerLocations.Values.Contains(roomLocation)) {
                    result.Add(roomLocation);
                }
            }
        }

        return result;
    }

    public static int GetPlayerTargetRoomX(Player player) {
        // I hate myself for this
        switch (player.Name) {
            case 'A': return 3;
            case 'B': return 5;
            case 'C': return 7;
            case 'D': return 9;
        }
        throw new InvalidDataException();
    }

    public bool IsAtGoalState(Dictionary<Player, (int x, int y)> playerLocations) {
        // Verify all players are in rooms
        foreach ((int x, int y) in playerLocations.Values) {
            if (!cells[x,y].isRoom) {
                return false;
            }
        }

        int[] playerXLocations = { -1, -1, -1, -1 };

        // Verify players of like type are in consistent x positions
        foreach (var item in playerLocations) {
            if (playerXLocations[playerIndex[item.Key.Name]] == -1) {
                playerXLocations[playerIndex[item.Key.Name]] = item.Value.x;
            } else if (playerXLocations[playerIndex[item.Key.Name]] != item.Value.x) {
                return false;
            }
        }

        return true;
    }

    public void Print(Path path) {
        for (int y = 0; y < cells.GetLength(1); y++) {
            for (int x = 0; x < cells.GetLength(0); x++) {
                 string roomText = "";

                 Player? occupiedBy = null;
                 foreach (var item in path.playerLocations) {
                     if (item.Value == (x,y)) {
                         occupiedBy = item.Key;
                         break;
                     }
                 }

                 if (cells[x,y].isWall) roomText = "#";
                 if (cells[x,y].isHalway) roomText = (occupiedBy != null) ? occupiedBy.Name.ToString() : ".";
                 if (cells[x,y].isRoom) roomText = (occupiedBy != null) ? occupiedBy.Name.ToString() : ".";
                
                 Console.Write(roomText);
            }
            Console.WriteLine();
        }
        Console.WriteLine("Cost: {0}", path.cost);
    }

    public Cell[,] cells;

    List<(int x, int y)> hallways = new List<(int x, int y)>();
    public List<(int x, int y)> rooms = new List<(int x, int y)>();

    Dictionary<Player, (int x, int y)> playerStartingLocations = new Dictionary<Player, (int x, int y)>();

    public Dictionary<char, int> playerIndex = new Dictionary<char, int>() { { 'A', 0 }, { 'B', 1 }, { 'C', 2 }, { 'D', 3 } };
}
