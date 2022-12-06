
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

    if (xRange.min >= -50 && xRange.max <= 50 && yRange.min >= -50 && yRange.max <= 50 && zRange.min >= -50 && zRange.max <= 50) {
        instructions.Add((onOff, xRange, yRange, zRange));
    }
}

Reactor reactor = new Reactor();

foreach (var instruction in instructions) {
    Console.WriteLine("Executing instuction.");
    reactor.Incorporate(instruction.on, instruction.Item2, instruction.Item3, instruction.Item4);
}

Console.WriteLine("Cubes: {0}", reactor.CubeCount);



(int min, int max) parseRange(string range) {
    var parts = range.Split('=');
    parts = parts[1].Split("..");
    return (Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
}



class Reactor {


    public int CubeCount {
        get { return _cubes.Count; }
    }

    public bool this[int x, int y, int z] {
        get {
            return _cubes.Contains((x, y, z));
        }
        set {
            if (Math.Abs(x) <= 50 && Math.Abs(y) <= 50 && Math.Abs(z) <= 50) {
                if (value) {
                    _cubes.Add((x, y, z));
                }
                else {
                    if (_cubes.Contains((x, y, z))) {
                        _cubes.Remove((x, y, z));
                    }
                }
            }
        }
    }

    public void Incorporate(bool on, (int, int) xRange, (int, int) yRange, (int, int) zRange) {
        for (int x = xRange.Item1; x <= xRange.Item2; x++) {
            for (int y = yRange.Item1; y <= yRange.Item2; y++) {
                for (int z = zRange.Item1; z <= zRange.Item2; z++) {
                    this[x, y, z] = on;
                }
            }
        }
    }

    private HashSet<(int x, int y, int z)> _cubes = new HashSet<(int x, int y, int z)>();
}