using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");

public record Vec3(long x, long y, long z) {

    public static Vec3 operator+(Vec3 lhs, Vec3 rhs) {
        return new Vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    }

    public static Vec3 operator-(Vec3 lhs, Vec3 rhs) {
        return new Vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
    }
}

public class Projectile {
    public Vec3 Position { get; set; }

    public Vec3 Velocity { get; set; }

    public Projectile(Vec3 position, Vec3 velocity) {
        this.Position = position;
        this.Velocity = velocity;
    }
}

static class Fun {

    public static IEnumerable<int> RadiateOutward(int max)
    {
        var i = 0;
        yield return i;
    
        while (i < max)
        {
            if (i >= 0) {
                i++;
            }
            i *= -1;
            yield return i;
        }
    }

    public static (bool intersects, (long X, long Y) pos, long time) TestIntersect(Projectile one, Projectile two, (int x, int y) offset)
    {
        // Create two new projectiles with the modified velocities
        var a = new Projectile(one.Position, new Vec3(one.Velocity.x + offset.x, one.Velocity.y + offset.y, one.Velocity.z));
        var c = new Projectile(two.Position, new Vec3(two.Velocity.x + offset.x, two.Velocity.y + offset.y, two.Velocity.z));

        // Find the determinate
        var D = (a.Velocity.x * -1 * c.Velocity.y) - (a.Velocity.y * -1 * c.Velocity.x);
    
        if (D == 0) return (false, (-1, -1), -1);
    
        var Qx = (-1 * c.Velocity.y * (c.Position.x - a.Position.x)) - (-1 * c.Velocity.x * (c.Position.y - a.Position.y));
    
        var t = Qx / D;
    
        var Px = a.Position.x + t * a.Velocity.x;
        var Py = a.Position.y + t * a.Velocity.y;
    
        return (true, (Px, Py), t);
    }

    public static void Run(string[] data)
    {
        var projectiles = new List<Projectile>();

        foreach (var sample in data)
        {
            var parts = sample.Split('@', ',').Select(x => x.Trim()).ToArray();

            var projectile = new Projectile(
                new Vec3(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2])),
                new Vec3(long.Parse(parts[3]), long.Parse(parts[4]), long.Parse(parts[5]))
            );

            projectiles.Add(projectile);
        }

        var position = FindRockOrigin(projectiles);

        var result = position.x + position.y + position.z;

        Console.WriteLine($"Rusult: {result}");
    }

    // Brute force search for the rock's origin.  This isn't great.  Hoepfully the velocity is small like it is for the 
    //  projectiles in the input data.
    private static Vec3 FindRockOrigin(List<Projectile> projectiles)
    {
        var range = 300;

        foreach (var x in RadiateOutward(range))
        {
            foreach (var y in RadiateOutward(range))
            {
                // The velocities of the projectiles can be modified by the rock's velocity (x,y). This allows looking for 
                //  intersections of the modified projectiles' trajectories instead of the original ones.  If we find an
                //  intersection, that's the rock's origin if the math works the way I think it does.
                var intersect1 = TestIntersect(projectiles[1], projectiles[0], (x, y));
                var intersect2 = TestIntersect(projectiles[2], projectiles[0], (x, y));
                var intersect3 = TestIntersect(projectiles[3], projectiles[0], (x, y));

                // If the proposed rock velocity didn't cause an intersection, keep searching
                if (!intersect1.intersects || intersect1.pos != intersect2.pos || intersect1.pos != intersect3.pos) continue;

                foreach (var z in RadiateOutward(range))
                {
                    // We know at what timestamp we would intersect the rock its initial position, so we can just check where the Z would end up at
                    // Check them for the first four hailstones as well
                    var intersectZ = projectiles[1].Position.z + intersect1.time * (projectiles[1].Velocity.z + z);
                    var intersectZ2 = projectiles[2].Position.z + intersect2.time * (projectiles[2].Velocity.z + z);
                    var intersectZ3 = projectiles[3].Position.z + intersect3.time * (projectiles[3].Velocity.z + z);

                    // If the proposed rock velocity didn't cause a common z intersection, keep searching
                    if (intersectZ != intersectZ2 || intersectZ != intersectZ3) continue;

                    Console.WriteLine($"Found it! Position: {intersect1.pos.X}, {intersect1.pos.Y}, {intersectZ}  Velocity: {x}, {y}, {z}");

                    // If four projectiles align, this is likely the right answer. Not sure how to prove it.
                    return new Vec3(intersect1.pos.X, intersect1.pos.Y, intersectZ);
                }
            }
        }

        throw new Exception("Din't find shit.");
    }
}