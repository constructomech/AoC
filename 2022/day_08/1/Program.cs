using System.IO;
using System.Collections.Generic;

long result = 0;

int[,]? trees = null;

using (StreamReader reader = File.OpenText("input.txt"))
{
    int lineNum = 0;
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            if (trees == null) {
                trees = new int [line.Length, line.Length];
            }

            for (int i = 0; i < line.Length; i++) {
                trees[i, lineNum] = Convert.ToInt32(line[i].ToString());
            }
        }

        lineNum++;
    }
}

int xMax = trees.GetLength(0);
int yMax = trees.GetLength(1);

for (int y = 0; y < yMax; y++) {
    for (int x = 0; x < xMax; x++) {
        if (Fun.IsVisible(trees, x, y)) {
            result++;
        }
    }
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static bool IsVisible(int[,] trees, int x, int y) {
        int xMax = trees.GetLength(0);
        int yMax = trees.GetLength(1);

        if (x == 0 || x == xMax - 1 || y == 0 || y == yMax - 1) {
            return true;
        } else {
            var treeVal = trees[x, y];
            bool visible = true;

            for (int yNeg = 0; yNeg < y; yNeg++) {
                if (treeVal <= trees[x, yNeg]) {
                    visible = false;
                }
            }
            if (visible) return true;
            visible = true;

            for (int yPos = y + 1; yPos < yMax; yPos++) {
                if (treeVal <= trees[x, yPos]) {
                    visible = false;
                    break;
                }
            }
            if (visible) return true;
            visible = true;

            for (int xNeg = 0; xNeg < x; xNeg++) {
                if (treeVal <= trees[xNeg, y]) {
                    visible = false;
                    break;
                }
            }
            if (visible) return true;
            visible = true;

            for (int xPos = x + 1; xPos < xMax; xPos++) {
                if (treeVal <= trees[xPos, y]) {
                    visible = false;
                    break;
                }
            }
            if (visible) return true;
        }

        return false;
    }
} 