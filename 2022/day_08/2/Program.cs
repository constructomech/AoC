using System.IO;
using System.Collections.Generic;

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

int score = 0;
int maxScore = 0;

for (int y = 0; y < yMax; y++) {
    for (int x = 0; x < xMax; x++) {
        score = Fun.ScenicScore(trees, x, y);
        if (score > maxScore) {
            maxScore = score;
        }
    }
}

Console.WriteLine("Result: {0}", maxScore);


static class Fun {
    public static int ScenicScore(int[,] trees, int x, int y) {
        int xMax = trees.GetLength(0);
        int yMax = trees.GetLength(1);

        int treeVal = trees[x, y];

        if (x == 0 || x == xMax - 1 || y == 0 || y == yMax - 1) {
            return 0;
        }

        int distYNeg = 0;
        for (int yNeg = y - 1; yNeg >= 0; yNeg--) {
            distYNeg++;
            if (trees[x, yNeg] >= treeVal) break;
        }

        int distYPos = 0;
        for (int yPos = y + 1; yPos < yMax; yPos++) {
            distYPos++;
            if (trees[x, yPos] >= treeVal) break;
        }
        
        int distXNeg = 0;
        for (int xNeg = x - 1; xNeg >= 0; xNeg--) {
            distXNeg++;
            if (trees[xNeg, y] >= treeVal) break;
        }

        int distXPos = 0;

        for (int xPos = x + 1; xPos < xMax; xPos++) {
            distXPos++;
            if (trees[xPos, y] >= treeVal) break;
        }

        return distYNeg * distYPos * distXNeg * distXPos;
    }
} 