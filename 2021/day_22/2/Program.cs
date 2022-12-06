
var lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        //on x=10..12,y=10..12,z=10..12
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
        }
    }
}

var instructions = new List<(bool on, (int xMin, int xMax), (int yMin, int yMax), (int zMin, int zMax))>();

foreach (string line in lines) {
    var parts = line.Split(' ');

    bool onOff = parts[0] == "on" ? true : false;

    parts = parts[1].Split(',');
    
    var xRange = parseRange(parts[0]);
    var yRange = parseRange(parts[1]);
    var zRange = parseRange(parts[2]);

    instructions.Add((onOff, xRange, yRange, zRange));
}

Reactor reactor = new Reactor();

foreach (var instruction in instructions) {
    Console.WriteLine("Executing instuction.");
    reactor.Incorporate(instruction.on, instruction.Item2, instruction.Item3, instruction.Item4);
}

reactor.Print();

Console.WriteLine("Cubes: {0}", reactor.CubeCount);



(int min, int max) parseRange(string range) {
    var parts = range.Split('=');
    parts = parts[1].Split("..");
    return (Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
}


class Reactor {

    public Reactor() {
        universe.on = false;
        universe.xMin = int.MinValue;
        universe.xMax = int.MaxValue;
        universe.yMin = int.MinValue;
        universe.yMax = int.MaxValue;
        universe.zMin = int.MinValue;
        universe.zMax = int.MaxValue;
    }

    public void Print() {
        universe.Print();
    }

    public long CubeCount {
        get {
            long result = 0;
            foreach (var volume in universe.oppositeVolumes) {
                result += volume.Size;
            }
            return result;
        }
    }

    public void Incorporate(bool on, (int, int) xRange, (int, int) yRange, (int, int) zRange) {

        Volume newVolume = new Volume() {
            on = on,
            xMin = xRange.Item1,
            xMax = xRange.Item2,
            yMin = yRange.Item1,
            yMax = yRange.Item2,
            zMin = zRange.Item1,
            zMax = zRange.Item2
        };

        universe.Incorporate(newVolume);
//        universe.Print();
    }

    Volume universe = new Volume();
}

class Volume {

    public long Size {
        get {
            long sign = on ? 1 : -1;
            long result = sign * (long)(xMax - xMin + 1) * (long)(yMax - yMin + 1) * (long)(zMax - zMin + 1);

            foreach (var subVol in oppositeVolumes) {
                result += subVol.Size;
            }

            return result;
        }
    }
    public bool Intersects(Volume other) {
        bool disjoint = xMax < other.xMin || xMin > other.xMax || 
                        yMax < other.yMin || yMin > other.yMax || 
                        zMax < other.zMin || zMin > other.zMax;
        return !disjoint;
    }

    public void Print(int depth = 0) {
        for (int i = 0; i < depth; i++) {
            Console.Write("   ");
        }
        Console.Write("{0}", on ? "+ " : "- ");
        for (int i = 0; i < depth; i++) {
            Console.Write("   ");
        }
        Console.WriteLine("({0},{1},{2}) to ({3},{4},{5})   [size: {6}]", xMin, yMin, zMin, xMax, yMax, zMax, Size);

        foreach(var sub in oppositeVolumes) {
            sub.Print(depth + 1);
        }
    }

    public Volume ShallowClone() {
        Volume result = new Volume();
        result.on = on;
        result.xMin = xMin;
        result.xMax = xMax;
        result.yMin = yMin;
        result.yMax = yMax;
        result.zMin = zMin;
        result.zMax = zMax;
        return result;
    }

    public Volume Intersect(Volume other) {
        Volume result = new Volume();
        result.on = other.on;
        result.xMin = Math.Max(xMin, other.xMin);
        result.xMax = Math.Min(xMax, other.xMax);
        result.yMin = Math.Max(yMin, other.yMin);
        result.yMax = Math.Min(yMax, other.yMax);
        result.zMin = Math.Max(zMin, other.zMin);
        result.zMax = Math.Min(zMax, other.zMax);
        return result;
    }

    public void Incorporate(Volume other, bool root = true) {

        if (xMin == -5 && yMin == -27 && zMin == -14) {
            Console.WriteLine("Debug me");
        }

        if (Intersects(other)) {
            Volume intersection = Intersect(other);

            Volume remove = intersection.ShallowClone();
            remove.on = false;

            foreach (var subVolume in oppositeVolumes) {
                subVolume.Incorporate(remove, false);
            }
            
            if (!root && !intersection.on && !on) {
                intersection.on = true;
            }

            if (intersection.on != on) {
                oppositeVolumes.Add(intersection);
            }
        }
    }

    public bool on;

    public int xMin, xMax;

    public int yMin, yMax; 
    public int zMin, zMax;

    public List<Volume> oppositeVolumes = new List<Volume>();
}