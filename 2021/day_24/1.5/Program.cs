var cache = new Dictionary<(int w, int z, int c, int d, int e), (int w, int x, int y, int z)>();
Queue<int> inputQueue = new Queue<int>();

var startTime = DateTime.UtcNow;

(long low, long high) range = (11111111111111, 99999999999999);
//(long low, long high) range = (99996758315581, 99996758315581);

for (long input = range.high; input >= range.low; input--) {

    if (input % 1000000 == 0) {
        var currentTime = DateTime.UtcNow;
        var elapsed = currentTime - startTime;
        double percentComplete = (double)(range.high - input) / (range.high - range.low);

        Console.WriteLine("Testing input {0}; elapsed {1} minutes; {2}% complete", input, Math.Round(elapsed.TotalMinutes, 0), Math.Round(percentComplete * 100, 4));
    }

    inputQueue.Clear();
     
    bool invalidInput = false;
    string stringInput = input.ToString();
    foreach (char c in stringInput) {
        if (c == '0') {
            invalidInput = true;
            break;
        }
        int num = c - '0';
        inputQueue.Enqueue(num);
    }

    if (!invalidInput) {

        (int w, int x, int y, int z) machineState = (0, 0, 0, 0);

        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 11, 6);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 11, 14);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 15, 13);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -14, 1);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 10, 6);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, 0, 13);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -6, 6);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 13, 3);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -3, 8);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 13, 14);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 1, 15, 4);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -2, 7);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -9, 15);
        machineState = Sub(inputQueue.Dequeue(), machineState.z, 26, -2, 1);

        if (machineState.z == 0) {
            Console.WriteLine("Found valid serial number: {0}", input);
            break;
        }
     }
}

Console.WriteLine("EOL");

(int w, int x, int y, int z) Sub(int w, int z, int c, int d, int e) {
    (int w, int x, int y, int z) result;
    if (!cache.TryGetValue((w, z, c, d, e), out result)) {
        int x = 0, y = 0;
        unchecked {
            // int x *= 0
            // x += z
            // x %= 26
            x = z % 26;
            z /= c;
            x += d;
            // x = (x == w) ? 1 : 0;
            // x = (x == 0) ? 1 : 0;   -- invert
            x = (x == w) ? 0 : 1;
            //int y = y * 0;
            // y += 25;
            y = 25;
            y *= x;
            y += 1;
            z *= y;
            // y = y * 0
            // y = y + w
            y = w;
            y += e;
            y *= x;
            z += y;
        }
//        Console.WriteLine("w={0}, x={1}, y={2}, z={3}", w, x, y, z);
        return (w, x, y, z);
    }
    return result;
}
