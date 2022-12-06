using System.Collections.Generic;
using System.IO;

string[] sequence;
List<Board> boards;
List<bool> winning;

using (StreamReader reader = File.OpenText("input.txt")) {

    string rawSequence = reader.ReadLine();
    sequence = rawSequence.Split(',');

    boards = new List<Board>();
    winning = new List<bool>();

    do
    {
        reader.ReadLine(); // Blank line
        Board board = new Board();

        for (int y = 0; y < Board.BOARD_SIZE; y++) {
            string line = reader.ReadLine();
            for (int x = 0; x < Board.BOARD_SIZE; x++) {
                string stringValue = line.Substring(x * 3, 2);
                 board.setAt(x, y, Convert.ToInt32(stringValue));
            }
        }
        
        boards.Add(board);
        winning.Add(false);

    } while (!reader.EndOfStream);
}

foreach (string rawNum in sequence) {
    int num = Convert.ToInt32(rawNum);

    for (int i = 0; i < boards.Count; i++) {
        Board board = boards[i];
        bool bingo = board.Call(num);
        if (bingo) {
            winning[i] = true;

            if (allWinning(winning)) {
                int score = board.getScore();
                Console.WriteLine("BINGO, score is: {0}, answer is {1}", score, score * num);
                return;
            }
        }
    }
}

bool allWinning(List<bool> list) {
    foreach (bool item in list) {
        if (item == false) return false;
    }
    return true;
}

class Cell {
    public int value = -1;
    public bool called = false;
}

class Board {
    public const int BOARD_SIZE = 5;
    
    public Board() {
        _cells = new Cell[BOARD_SIZE * BOARD_SIZE];
    }

    public void setAt(int x, int y, int value) {
        _cells[x + y * BOARD_SIZE] = new Cell() { value = value, called = false };
    }

    public Cell getAt(int x, int y) {
        return _cells[x + y * BOARD_SIZE];
    }

    public bool Call(int number) {
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int j = 0; j < BOARD_SIZE; j++) {
                Cell cell = getAt(i, j);
                if (cell.value == number) {
                    cell.called = true;

                    bool bingoHorizonal = true;
                    for (int x = 0; x < BOARD_SIZE; x++) {
                        cell = getAt(x, j);
                        if (cell.called == false) {
                            bingoHorizonal = false;
                            break;
                        }
                    }

                    bool bingoVertical = true;
                    for (int y = 0; y < BOARD_SIZE; y++) {
                        cell = getAt(i, y);
                        if (cell.called == false) {
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
                Cell cell = getAt(i, j);
                if (!cell.called) {
                    result += cell.value;
                }
            }
        }
        return result;
    }

    Cell[] _cells; 
}


