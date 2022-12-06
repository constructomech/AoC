using System.IO;
using System.Collections.Generic;

// position, quantity
SortedList<int, int> positions = new SortedList<int, int>();
int total = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {

            string[] rawPositions = line.Split(',');
            foreach (var rawPos in rawPositions) {
                int pos = Convert.ToInt32(rawPos);
                total += pos;
                if (positions.ContainsKey(pos)) {
                    positions[pos]++;
                }
                else {
                    positions.Add(pos, 1);
                }
            }
        }
    }
}

int lowestFuel = int.MaxValue;
int bestPosition = -1;
for (int pos = positions.First().Key; pos < positions.Last().Key; pos++) {
    int fuel = calculateFuel(pos);
    if (fuel < lowestFuel) {
        lowestFuel = fuel;
        bestPosition = pos;
    }
}

Console.WriteLine("Best Pos: {0}, Total Fuel: {1}", bestPosition, lowestFuel);

// foreach ((var position, var count) in positions) {
//     // int deltaForOne = Math.Abs(position - bestPosition); 
//     int deltaForOne = fuelForMove(Math.Abs(position - bestPosition));
//     int groupDelta = deltaForOne * count;
//     Console.WriteLine(" - Move {0} from {1} to {2}: {3} fuel", count, position, bestPosition, groupDelta);
// }

int calculateFuel(int targetPos) {
    int totalDelta = 0;
    foreach ((var position, var count) in positions) {
        // int deltaForOne = Math.Abs(position - targetPos); 
        int deltaForOne = fuelForMove(Math.Abs(position - targetPos));
        int groupDelta = deltaForOne * count;

        totalDelta += groupDelta;
    }
    return totalDelta;
}

int fuelForMove(int steps) {
    int result = 0;
    for (int step = 0; step < steps; step++) {
        result += (step + 1);
    }
    return result;
}
