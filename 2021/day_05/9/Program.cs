Board board = new Board();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null)
        {
            string[] rawCoords = line.Split(" -> ");

            string[] first = rawCoords[0].Split(',');
            string[] second = rawCoords[1].Split(',');

            int x1 = Convert.ToInt32(first[0]);
            int y1 = Convert.ToInt32(first[1]);
            int x2 = Convert.ToInt32(second[0]);
            int y2 = Convert.ToInt32(second[1]);

            board.place(x1, y1, x2, y2);
        }
    }
}

int result = board.countOfDangerous();

Console.WriteLine("Result: {0}", result);

// board.print();

class Board {

    private const int BOARD_SIZE = 1000;

    public void place(int x1, int y1, int x2, int y2) {
        if (x1 == x2) {
            // Console.WriteLine("Placing {0},{1} -> {2},{3}", x1, y1, x2, y2);
            for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++) {
                incrementAt(x1, y);
            }
            // print();
            // Console.WriteLine("---");
        }
        else if (y1 == y2) {
            // Console.WriteLine("Placing {0},{1} -> {2},{3}", x1, y1, x2, y2);
            for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++) {
                incrementAt(x, y1);
            }
            // print();
            // Console.WriteLine("---");
        }
        else {
            // It's a diagonal
            int yInc = 1;

            if (x1 < x2) {
                if (y1 > y2) yInc = -1;
                for (int x = x1, y = y1; x <= x2; x++, y += yInc) {
                    incrementAt(x, y);
                }
            } else {
                if (y2 > y1) yInc = -1;
                for (int x = x2, y = y2; x <= x1; x++, y += yInc) {
                    incrementAt(x, y);
                }
            }
        }

    }

    public void incrementAt(int x, int y) {
        _counts[x + y * BOARD_SIZE] += 1;
    }

    public int getAt(int x, int y) {
        return _counts[x + y * BOARD_SIZE];
    }

    public int countOfDangerous() {
        int result = 0;
        foreach (int count in _counts) {
            if (count > 1) {
                result++;
            }
        }

        return result;
    }

    public void print() {
        for (int y = 0; y < BOARD_SIZE; y++) {
            for (int x = 0; x < BOARD_SIZE; x++) {
                Console.Write("{0} ", getAt(x, y));
            }
            Console.WriteLine();
        }
    }

    private int[] _counts = new int[BOARD_SIZE * BOARD_SIZE];
}