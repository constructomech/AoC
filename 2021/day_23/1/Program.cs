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
Path path = board.Solve();
board.Print(path);


Console.WriteLine("EOL");




class Room {
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

    public Path? Move(Board board, Player player, (int x, int y) move) {
        Path result = null;

        (int x, int y) newLocation = playerLocations[player];
        newLocation.x += move.x;
        newLocation.y += move.y;

        if (IsValidMove(board, this, playerLocations[player], newLocation)) {
            result = new Path() { parent = this };
            foreach (var item in playerLocations) {
                if (item.Key == player) {
                    result.playerLocations.Add(item.Key, newLocation);
                }
                else {
                    result.playerLocations.Add(item.Key, item.Value);
                }
            }
            result.cost = cost + getCostToMove(player) * (Math.Abs(move.x) + Math.Abs(move.y));
        }
        return result;
    }

    public static bool IsValidMove(Board board, Path path, (int x, int y) location, (int x, int y) newLocation) {
        if (newLocation.x < 0 || newLocation.x >= board.rooms.GetLength(0) || 
            newLocation.y < 0 || newLocation.y >= board.rooms.GetLength(1)) {
                return false;
        }

        Player targetPlayer = null;
        foreach(var item in path.playerLocations) {
            if (item.Value == location) {
                targetPlayer = item.Key;
                break;
            }
        }

        // If there are any players at the new location, move is invalid
        foreach (var item in path.playerLocations) {
            if (item.Value == newLocation) {
                return false;
            }
        }

        Room startRoom = board.rooms[location.x, location.y];
        Room targetRoom = board.rooms[newLocation.x, newLocation.y];

        // If the room is a wall, the move is invalid.
        if (targetRoom.isWall) {
            return false;
        }

        // Players never stop on the space immediatley outside any room.
        foreach (var position in path.playerLocations.Values) {
            if (board.rooms[position.x, position.y].isHalway && board.rooms[position.x, position.y + 1].isRoom) {
                // If we're trying to move any player other than the one already blocking a room, move is invalid.
                if (location != position) {
                    return false;
                }
            }
        }

        // Players never enter a room unless it is their destination room.
        if (startRoom.isHalway && targetRoom.isRoom) {

            var bottomRoomPos = newLocation;
            bottomRoomPos.y += 1;

            // Verify if there is any player already in the room, it's of the same name.
            foreach (var item in path.playerLocations) {
                if (item.Value == bottomRoomPos) {
                    if (item.Key.Name != targetPlayer.Name) {
                        return false;
                    }
                }
            }
        }

        // If this move represents a new player entering the hallway

        //TODO: Encode remaining Rule: 
        // Once a player stops moving in the hallway, it will stay in that spot until it can move into a room. 
        // (That is, once any player starts moving, any other players currently in the hallway are locked in place and will not move again until they can move fully into a room.
        var playerStates = new PlayerState[] { PlayerState.Stopped, PlayerState.Stopped, PlayerState.Stopped, PlayerState.Stopped };
        var forwardPath = GetInOrderPath(path);
        foreach(var step in forwardPath) {
            // Find what changed
            if (step.parent != null) {
                var move = GetMove(step.parent, step);

                var previousState = playerStates[board.playerIndex[move.player.Name]];
                playerStates[board.playerIndex[move.player.Name]] |= PlayerState.Moving;

                // Mark all other players as no longer moving and potentially as stopped in the hallway
                foreach (var otherPlayer in path.playerLocations.Keys) {
                    if (otherPlayer != move.player) {
                        if ((playerStates[board.playerIndex[otherPlayer.Name]] & PlayerState.Moving) == PlayerState.Moving) {
                            // Mark as not moving
                            playerStates[board.playerIndex[otherPlayer.Name]] &= ~PlayerState.Moving;

                            // If the stop position was in a hallway, mark as stopped in hallway
                            var otherPlayerLocation = step.playerLocations[otherPlayer];
                            if (board.rooms[otherPlayerLocation.x, otherPlayerLocation.y].isHalway) {
                                playerStates[board.playerIndex[otherPlayer.Name]] |= PlayerState.StoppedInHallway;
                            }
                        }
                    }
                }
            }

            Player movingPlayer = null;
            int stoppedPlayers = 0;
            // Review player states to see if this move would cause another player to stop in the hallway
            foreach (var player in path.playerLocations.Keys) {
                if ((playerStates[board.playerIndex[player.Name]] & PlayerState.Moving) == PlayerState.Moving) {
                    movingPlayer = player;
                }

                if ((playerStates[board.playerIndex[player.Name]] & PlayerState.StoppedInHallway) == PlayerState.StoppedInHallway) {
                    stoppedPlayers++;
                }
            }
            // If this move would cause another stopped player
            if (movingPlayer != null && movingPlayer != targetPlayer) {
                if ((playerStates[board.playerIndex[movingPlayer.Name]] & PlayerState.StoppedInHallway) == PlayerState.StoppedInHallway) {
                    var movingPlayerLocation = path.playerLocations[movingPlayer];
                    if (board.rooms[movingPlayerLocation.x, movingPlayerLocation.y].isHalway) {
                        return false;                       
                    }
                }
            }
        }

        return true;
    }

    private static List<Path> GetInOrderPath(Path tail) {
        var result = new List<Path>();
        Path current = tail;
        while (current != null) {
            result.Insert(0, current);
            current = current.parent;
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

    public Dictionary<Player, (int x, int y)> playerLocations = new Dictionary<Player, (int x, int y)>();

    public int cost;

    public Path? parent = null;
}

class Board {
    public Board(List<string> lines, int maxLength) {
        int y = 0;
        this.rooms = new Room[maxLength, lines.Count];

        foreach (var line in lines) {

            for (int x = 0; x < maxLength; x++) {
                Room room = new Room();

                if (x >= line.Length) {
                    room.isWall = true;
                }
                else {
                    switch(line[x]) {
                        case ' ':
                        case '#':
                            room.isWall = true;
                            break;
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                            Player player = new Player() { Name = line[x] };
                            room.isRoom = true;
                            playerStartingLocations.Add(player, (x, y));
                            break;
                        case '.':
                            room.isHalway = true;
                            break;
                    }
                }
                rooms[x, y] = room;
            }
            y++;
        }
    }

    public Path Solve() {
        Path winner = null;

        PriorityQueue<Path> queue = new PriorityQueue<Path>();
        Path root = new Path() { playerLocations = playerStartingLocations };
        queue.Enqueue(root);

        while (queue.Count > 0) {
            Path path = queue.Dequeue();

            if (path.parent != null) {
                Console.WriteLine("Before:");
                Print(path.parent);
                Console.WriteLine("After:");
            }

            Print(path);
            Console.WriteLine("Queue contains {0} items.", queue.Count);
            Console.WriteLine();
            
            // If at goal state
            if (IsAtGoalState(path.playerLocations)) {
                winner = path;
                break;
            }

            // Add children
            foreach (var player in playerStartingLocations.Keys) {
                (int x, int y)[] cardinalDirections = { (0, -1), (0, 1), (-1, 0), (1, 0)};

                foreach (var cardinalDirection in cardinalDirections) {

                    Path? move = path.Move(this, player, cardinalDirection);
                    if (move != null) { // is valid
                        queue.Enqueue(move);
                    }
                }
            }
        }

        return winner;
    }

    public bool IsAtGoalState(Dictionary<Player, (int x, int y)> playerLocations) {
        // Verify all players are in rooms
        foreach ((int x, int y) in playerLocations.Values) {
            if (!rooms[x,y].isRoom) {
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
        for (int y = 0; y < rooms.GetLength(1); y++) {
            for (int x = 0; x < rooms.GetLength(0); x++) {
                 string roomText = "";

                 Player? occupiedBy = null;
                 foreach (var item in path.playerLocations) {
                     if (item.Value == (x,y)) {
                         occupiedBy = item.Key;
                         break;
                     }
                 }

                 if (rooms[x,y].isWall) roomText = "#";
                 if (rooms[x,y].isHalway) roomText = (occupiedBy != null) ? occupiedBy.Name.ToString() : ".";
                 if (rooms[x,y].isRoom) roomText = (occupiedBy != null) ? occupiedBy.Name.ToString() : ".";
                
                 Console.Write(roomText);
            }
            Console.WriteLine();
        }
        Console.WriteLine("Cost: {0}", path.cost);
    }

    public Room[,] rooms;
    Dictionary<Player, (int x, int y)> playerStartingLocations = new Dictionary<Player, (int x, int y)>();

    public Dictionary<char, int> playerIndex = new Dictionary<char, int>() { { 'A', 0 }, { 'B', 1 }, { 'C', 2 }, { 'D', 3 } };
}

[Flags]
enum PlayerState {
    Stopped = 0,
    Moving = 1,
    StoppedInHallway = 2
}