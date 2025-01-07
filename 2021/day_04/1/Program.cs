string[] sequence;
List<Board> boards;

using (StreamReader reader = File.OpenText("input.txt")) {

    var rawSequence = reader.ReadLine()!;
    sequence = rawSequence.Split(',');

    boards = new List<Board>();
 
    do
    {
        reader.ReadLine(); // Blank line
        Board board = new Board();

        for (int y = 0; y < Board.BOARD_SIZE; y++) {
            string line = reader.ReadLine()!;
            for (int x = 0; x < Board.BOARD_SIZE; x++) {
                string stringValue = line.Substring(x * 3, 2);
                 board[x, y] = (Convert.ToInt32(stringValue), false);
            }
        }
        
        boards.Add(board);

    } while (!reader.EndOfStream);
}

foreach (string rawNum in sequence) {
    int num = Convert.ToInt32(rawNum);

    foreach (var board in boards) {
        bool bingo = board.Call(num);
        if (bingo) {
            int score = board.getScore();
            Console.WriteLine("BINGO, score is: {0}, answer is {1}", score, score * num);
            return;
        }
    }
}

class Board {
    public const int BOARD_SIZE = 5;
    
    public Board() {
        _cells = new (int value, bool called)[BOARD_SIZE * BOARD_SIZE];
    }

    public (int value, bool called) this[int x, int y] {
        get { var cell = _cells[x + y * BOARD_SIZE]; return (cell.value, cell.called); }
        set { _cells[x + y * BOARD_SIZE] = value; }
    }

    public bool Call(int number) {
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int j = 0; j < BOARD_SIZE; j++) {
                var cell = this[i, j];
                if (cell.value == number) {
                    cell.called = true;
                    this[i, j] = cell;

                    bool bingoHorizonal = true;
                    for (int x = 0; x < BOARD_SIZE; x++) {
                        if (this[x, j].called == false) {
                            bingoHorizonal = false;
                            break;
                        }
                    }

                    bool bingoVertical = true;
                    for (int y = 0; y < BOARD_SIZE; y++) {
                        if (this[i, y].called == false) {
                            bingoVertical = false;
                            break;
                        }
                    }

                    if (bingoHorizonal || bingoVertical) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public int getScore() {
        int result = 0;
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int j = 0; j < BOARD_SIZE; j++) {
                var cell = this[i, j];
                if (!cell.called) {
                    result += cell.value;
                }
            }
        }
        return result;
    }

    (int value, bool called)[] _cells; 
}


