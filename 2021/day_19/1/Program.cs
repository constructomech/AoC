using System.Text;

var clouds = new List<Cloud>();
int scannerNumber = -1;
Cloud currentCloud = null;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null && line.Length > 0) {
            if (line.StartsWith("--- scanner ")) {
                if (currentCloud != null) {
                    clouds.Add(currentCloud);
                }
                scannerNumber++;
                currentCloud = new Cloud() { SensorNumber = scannerNumber };
            }
            else {
                var parts = line.Split(',');
                int x = Convert.ToInt32(parts[0]);
                int y = Convert.ToInt32(parts[1]);
                int z = Convert.ToInt32(parts[2]);

                currentCloud.Beacons.Add(new Point() { X = x, Y = y, Z = z });
            }
        }
    }
    clouds.Add(currentCloud);
}

Queue<Cloud> toMatch = new Queue<Cloud>(clouds);
Cloud accum = toMatch.Dequeue();

while (toMatch.Count > 0) {
    Cloud current = toMatch.Dequeue();

    (bool match, Cloud? combined) = FindMatch(accum, current);    
    if (match) {
        accum = combined;
    }
    else {
        toMatch.Enqueue(current);
    }
}

Console.WriteLine("Size of combined: {0}", accum.Count);


(bool match, Cloud? combined) FindMatch(Cloud cloud1, Cloud cloud2) {
    // 24 possible rotations - 90 increments in all axes (reflect x, reflect y, reflect z, rotate x, rotate y, rotate z)
    // For 2 sensors, check every beacon in A vs every one in B for all 24 possible translations to see if there are 12+ common beacons.

    int sumOfBeacons = cloud1.Count + cloud2.Count;

    foreach (var beacon1 in cloud1.Beacons) {
        foreach (var permutation in cloud2.GetPermutations()) {
            foreach (var beacon2 in permutation.Beacons) {
                // Console.WriteLine("Permuted {0}", permutation);

                Point difference = beacon1.Subtract(beacon2);
                // Console.WriteLine("Difference: {0} (of {1} and {2})", difference, beacon1, beacon2);

                Cloud cloud2translated = permutation.Translate(difference);
                // Console.WriteLine("Translated {0}", cloud2translated);

                Cloud combined = cloud1.Combine(cloud2translated);
                // Console.WriteLine("Combined {0}", combined);

                if (sumOfBeacons - combined.Count >= 12) {
                    Console.WriteLine("Sensor 1 ({0}) and Sensor 2 ({1}) combine to {2} beacons.", cloud1.Count, permutation.Count, combined.Count);
                    return (true, combined);
                }
            }
        }
    }
    
    return (false, null);
}


struct Point {

    public Point Roll() {
        return new Point() { X = X, Y = Z, Z = -Y };
    }

    public Point Turn() {
        return new Point() { X = -Y, Y = X, Z = Z };
    }

    public Point Subtract(Point other) {
        return new Point() { X = X - other.X, Y = Y - other.Y, Z = Z - other.Z }; 
    }

    public override string ToString() {
        return X + "," + Y + "," + Z;
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}

class Cloud {

    public int SensorNumber {
        get; set;
    }

    public int Count {
        get {
            return Beacons.Count;
        }
    }

    public List<Point> Beacons {
        get {
            return _beacons;
        }
    }

    public override string ToString() {
        var builder = new StringBuilder();
        builder.AppendFormat("Cloud {0}\n", SensorNumber);
        foreach(var point in Beacons) {
            builder.AppendFormat("  {0}\n", point);
        }
        builder.AppendFormat("{0} beacons.\n", Count);
        return builder.ToString();
    }

    public IEnumerable<Cloud> GetPermutations() {
        Cloud result = this;
        foreach (int cycle in new int[] { 0, 1 }) {
            foreach (int step in new int[] { 0, 1, 2 }) {
                result = result.Roll();
                yield return result;

                foreach (int i in new int[] { 0, 1, 2 }) {
                    result = result.Turn();
                    yield return result;
                }
            }
            result = result.Roll().Turn().Roll();
        }
    }

    public Cloud Translate(Point difference) {
        var result = new Cloud() { SensorNumber = SensorNumber };
        foreach (var point in Beacons) {
            Point translated = new Point() { X = point.X + difference.X, Y = point.Y + difference.Y, Z = point.Z + difference.Z };
            result.Beacons.Add(translated);
        }
        return result;
    }

    public Cloud Roll() {
        var result = new Cloud() { SensorNumber = SensorNumber };
        foreach (var point in Beacons) {
            Point rolled = point.Roll();
            result.Beacons.Add(rolled);
        }
        return result;
    }

    public Cloud Turn() {
        var result = new Cloud() { SensorNumber = SensorNumber };
        foreach (var point in Beacons) {
            Point rolled = point.Turn();
            result.Beacons.Add(rolled);
        }
        return result;
    }

    // public Cloud Compute(Func<Point, Point> translation) {
    //     var result = new Cloud() { SensorNumber = SensorNumber };
    //     foreach (var point in Beacons) {
    //         Point translated = translation(point);
    //         result.Beacons.Add(translated);
    //     }
    //     return result;
    // }

    public Cloud Clone() {
        var copy = new Cloud();
        foreach (var point in _beacons) {
            copy.Beacons.Add(point);
        }
        return copy;
    }

    public Cloud Combine(Cloud other) {
        var points = new HashSet<Point>();
        foreach (var beacon in Beacons) {
            points.Add(beacon);
        }
        foreach (var beacon in other.Beacons) {
            points.Add(beacon);
        }
        Cloud result = new Cloud() { SensorNumber = -1 };
        foreach(var beacon in points) {
            result.Beacons.Add(beacon);
        }
        return result;
    }

    private List<Point> _beacons = new List<Point>();
}