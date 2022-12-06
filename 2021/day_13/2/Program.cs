

var dots = new List<Position>();
var folds = new List<Fold>();
bool parsingInstructions = false;
int width = 0;
int height = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {
            if (line == "") {
                parsingInstructions = true;
            }
            else if (parsingInstructions) {
                string[] parts = line.Split('=');

                Direction direction = parts[0] == "fold along x" ? Direction.X : Direction.Y;
                int coordinate = Convert.ToInt32(parts[1]);
                folds.Add(new Fold() { direction = direction, coordinate = coordinate });
            }
            else {
                string[] parts = line.Split(',');
                int x = Convert.ToInt32(parts[0]);
                int y = Convert.ToInt32(parts[1]);

                if (x > width) width = x;
                if (y > height) height = y;

                dots.Add(new Position() {x = x, y = y });
            }
        }
    }
}

var matrix = new Matrix(width + 1, height + 1, dots);

foreach (var fold in folds) {
    matrix = matrix.Fold(fold);
}

matrix.print();

Console.WriteLine("Dots: {0}", matrix.DotCount);


class Matrix {
    public Matrix(int x, int y, List<Position> positions = null) {
        _dots = new bool[x, y];

        if (positions != null) {
            foreach (var pos in positions) {
                _dots[pos.x, pos.y] = true;
            }
        }
    }

    public bool getAt(int x, int y) {
        return _dots[x, y];
    }

    public void setAt(int x, int y, bool value) {
        _dots[x, y] = value;
    }

    public int Width {
        get {
            return _dots.GetLength(0);
        }
    }

    public int Height {
        get {
            return _dots.GetLength(1);
        }
    }

    public int DotCount {
        get {
            int result = 0;
            for (int y = 0; y < _dots.GetLength(1); y++) {            
                for (int x = 0; x < _dots.GetLength(0); x++) {
                    if (_dots[x, y] == true) result++;
                }
            }
            return result;
        }
    }

    public Matrix Fold(Fold fold) {
        Matrix result;
        if (fold.direction == Direction.X) {
            result = new Matrix(Math.Max(fold.coordinate, Width - fold.coordinate - 1), Height);

            for (int x = 0; x < fold.coordinate; x++) {
                for (int y = 0; y < Height; y++) {
                    result.setAt(x, y, getAt(x, y));
                }
            }

            for (int x = 0; x < Width - fold.coordinate - 1; x++) {
                for (int y = 0; y < Height; y++) {
                    if (getAt(Width - x - 1, y)) {
                        result.setAt(x, y, true);
                    }
                }
            }

        } else {
            result = new Matrix(Width, Math.Max(fold.coordinate, Height - fold.coordinate - 1));

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < fold.coordinate; y++) {
                    result.setAt(x, y, getAt(x, y));
                }
            }

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height - fold.coordinate - 1; y++) {
                    if (getAt(x, Height - y - 1)) {
                        result.setAt(x, y, true);
                    }
                }
            }
        }

        return result;
    }

    public void print() {
        for (int y = 0; y < _dots.GetLength(1); y++) {            
            for (int x = 0; x < _dots.GetLength(0); x++) {
                Console.Write(_dots[x, y] ? "#" : ".");
            }
            Console.WriteLine();
        }
    }

    bool[,] _dots;
}

class Position {
    public int x;
    public int y;
}

enum Direction {
    X,
    Y
}
class Fold {
    public Direction direction;

    public int coordinate;
}