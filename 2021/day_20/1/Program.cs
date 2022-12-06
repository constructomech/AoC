using System.IO;
using System.Collections.Generic;

List<bool> enhanceMap = new List<bool>();
Image image = new Image();

using (StreamReader reader = File.OpenText("input.txt"))
{
    string? line = reader.ReadLine();
    foreach (char c in line) {
        enhanceMap.Add(c == '#');
    }

    int y = 0;

    while (!reader.EndOfStream)
    {
        line = reader.ReadLine();
        if (line != null && line != "") {
            for (int x = 0; x < line.Length; x++) {
                if (line[x] == '#') {
                    image[x, y] = true;
                }
            }            
            y++;
        }
    }
}

image.Print();

for (int step = 0; step < 2; step++) {
    image = image.Enhance(enhanceMap);
    Console.WriteLine("Step {0}", step);
    image.Print();
}

Console.WriteLine("Pixels: {0}", image.PixelCount);

class Image {

    public int PixelCount {
        get {
            return _points.Count;
        }
    }

    public bool this[int x, int y] {
        get {
            if (_points.Contains((x, y))) {
                return pointInListAre;
            }
            return !pointInListAre;
        }
        set {
            if (value == pointInListAre) {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;

                _points.Add((x, y));
            }            
            else {
                // Could recompute min, max but I'm lazy and it won't hurt anything
                _points.Remove((x, y));
            }
        }
    }

    public void Print() {
        for (int y = minY - 3; y <= maxY + 3; y++) {
            for (int x = minX - 3; x <= maxX + 3; x++) {
                Console.Write(this[x, y] ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public Image Enhance(List<bool> key) {
        Image result = new Image();
        if (key[0]) {
            result.pointInListAre = !pointInListAre;
        }

        for (int sourceY = minY - 1; sourceY <= maxY + 1; sourceY++) {
            for (int sourceX = minX - 1; sourceX <= maxX + 1; sourceX++) {

                // if (sourceX == 2 && sourceY == -1) {
                //     Console.WriteLine("Test pixel");
                // }

                int idx = 0;
                for (int y = sourceY - 1; y <= sourceY + 1; y++) {
                    for (int x = sourceX - 1; x <= sourceX + 1; x++) {
                        //Console.WriteLine("Testing: {0}, {1}", x, y);
                        idx = idx << 1;
                        if (this[x, y] == true) {
                            idx |= 1;
                        }
                    }
                }
                bool targetPixel = key[idx];
                result[sourceX, sourceY] = targetPixel;
            }
        }
        return result;
    }

    HashSet<(int, int)> _points = new HashSet<(int, int)>();

    int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

    bool pointInListAre = true;
}

