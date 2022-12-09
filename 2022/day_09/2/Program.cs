using System.IO;
using System.Collections.Generic;

const int totalKnots = 10;
var visited = new HashSet<Pos>();

var knotPos = new List<Pos>();
for (int i = 0; i < totalKnots; i++) {
    knotPos.Add(new Pos { X = 0, Y = 0 });
}

visited.Add(new Pos { X = 0, Y = 0 });

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            var parts = line.Split(' ');
            int count = Convert.ToInt32(parts[1]);

            Console.WriteLine("== {0} {1} ==", parts[0], count);

            for (int i = 0; i < count; i++) {
                switch(parts[0]) {
                    case "R":
                        var pos = knotPos[0];
                        pos.X++;
                        knotPos[0] = pos;
                        break;
                    case "L":
                        pos = knotPos[0];
                        pos.X--;
                        knotPos[0] = pos;
                        break;
                    case "U":
                        pos = knotPos[0];
                        pos.Y--;
                        knotPos[0] = pos;
                        break;                
                    case "D":
                        pos = knotPos[0];
                        pos.Y++;
                        knotPos[0] = pos;
                        break;
                }

                for (int j= 1; j < totalKnots; j++) {
                    knotPos[j] = Fun.MoveTail(knotPos[j - 1], knotPos[j]);
                }

                visited.Add(knotPos[totalKnots - 1]);
            }
        }
    }
}



Console.WriteLine("Result: {0}", visited.Count);



public struct Pos {
    public int X { get; set; }
    public int Y { get; set; }
}

static class Fun {
    public static Pos MoveTail(Pos headPos, Pos tailPos) {
        bool touching = Math.Abs(headPos.X - tailPos.X) <= 1 && Math.Abs(headPos.Y - tailPos.Y) <= 1;
        bool sameRow = headPos.Y == tailPos.Y;
        bool sameCol = headPos.X == tailPos.X;

        if (!touching && !sameRow && !sameCol) {
            // Move diagonally
            if (tailPos.X - headPos.X > 0) {
                tailPos.X--;
            } else if (headPos.X - tailPos.X > 0) {
                tailPos.X++;
            }

            if (tailPos.Y - headPos.Y > 0) {
                tailPos.Y--;
            } else if (headPos.Y - tailPos.Y > 0) {
                tailPos.Y++;
            }
        }
        else {
            if (tailPos.X - headPos.X > 1) {
                tailPos.X--;
            } else if (headPos.X - tailPos.X > 1) {
                tailPos.X++;
            }

            if (tailPos.Y - headPos.Y > 1) {
                tailPos.Y--;
            } else if (headPos.Y - tailPos.Y > 1) {
                tailPos.Y++;
            }
        }

        return tailPos;
    }
}