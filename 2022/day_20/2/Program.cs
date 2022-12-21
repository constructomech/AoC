using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var data = new List<(long item, int initialPos)>();

// Parse
//
var input = File.ReadAllLines("input.txt");
for (var i = 0; i < input.Length; i++) 
{
    var line = input[i];

    //data.Add((Convert.ToInt64(line), i));
    data.Add((Convert.ToInt64(line) * 811589153L, i));
}

// Mix
//
for (int i = 0; i < 10; i++)
{
    for (int n = 0; n < data.Count; n++)
    {
        // Find move position.
        long pos = 0;
        while (data[(int)pos].initialPos != n) pos++;

        var (offset, initialPos) = data[(int)pos];

        if (offset != 0)
        {
            data.RemoveAt((int)pos);

            long newPos = pos + offset;
            if (newPos < 0)
            {
                // while (newPos < 0) newPos = data.Count + newPos;
                newPos = (long)data.Count + (newPos % (long)data.Count);
            }
            else if (newPos >= data.Count) 
            {
                //while (newPos >= data.Count) newPos -= data.Count;
                newPos = newPos % (long)data.Count;
            }
            else if (newPos == 0)
            {
                newPos = data.Count;
            }
            data.Insert((int)newPos, (offset, initialPos));
        }
    }
}

// Calculate answer
//
long zeroIndex = data.FindIndex(0, data.Count, val => val.item == 0);

long a = (zeroIndex + 1000L) % (long)data.Count;
long b = (zeroIndex + 2000L) % (long)data.Count;
long c = (zeroIndex + 3000L) % (long)data.Count;

long aVal = data[(int)a].item;
long bVal = data[(int)b].item;
long cVal = data[(int)c].item;

long result = aVal + bVal + cVal;

// 2434768014 is too low

watch.Stop();
Console.WriteLine($"Result {result}, completed in {watch.ElapsedMilliseconds}ms");
