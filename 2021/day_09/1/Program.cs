List<string> lines = new List<string>();

int columns = 0;
int rows = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
            columns = line.Length;
            rows++;
        }
    }
}


int [,] matrix = new int[columns, rows];

int readRow = 0;
foreach (string line in lines) {
    for (int column = 0; column < line.Length; column++) {
        matrix[column, readRow] = Convert.ToInt32(line[column].ToString());
    }
    readRow++;
}

int totalRisk = 0;

int xDim = matrix.GetLength(0);
int yDim = matrix.GetLength(1);

for (int row = 0; row < matrix.GetLength(1); row++) {
    for (int column = 0; column < matrix.GetLength(0); column++) {

        if (isLowest(matrix, column, row)) {

            Console.WriteLine("Found risk at ({0}, {1})", column, row);
            int risk = matrix[column, row] + 1;

            totalRisk += risk;
        }
    }
}

Console.WriteLine("Risk: {0}", totalRisk);


bool isLowest(int[,] matix, int x, int y) {

    // Left
    if (x > 0 && matrix[x, y] >= matrix[x - 1, y]) {
        return false;
    }

    // Right
    if (x < matrix.GetLength(0) - 1 && matrix[x, y] >= matrix[x + 1, y]) {
        return false;
    }

    // Above    
    if (y > 0 && matrix[x, y] >= matrix[x, y - 1]) {
        return false;
    }

    // Below
    if (y < matrix.GetLength(1) - 1 && matrix[x, y] >= matrix[x, y + 1]) {
        return false;
    }

    return true;
}
