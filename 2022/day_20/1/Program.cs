using System.Collections.Immutable;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var data = new List<(int item, bool completed)>();

// Parse
//
var input = File.ReadAllLines("input.txt");
foreach (var line in input)
{
    data.Add((Convert.ToInt32(line), false));
}

// Mix
//
bool complete = false;
while (!complete)
{
    // Find first position that is not completed.
    int pos = 0;
    while (pos < data.Count && data[pos].completed) pos++;
    if (pos < data.Count)
    {
        int offset = data[pos].item;

        data.RemoveAt(pos);
        int newPos = pos + offset;
        if (newPos < 0)
        {
            while (newPos < 0) newPos = data.Count + newPos;                
        }
        else if (newPos >= data.Count) 
        {
            while (newPos >= data.Count) newPos -= data.Count;
        }
        else if (newPos == 0)
        {
            newPos = data.Count;
        }
        data.Insert(newPos, (offset, true));
    }
    else 
    {
        complete = true;
    }
}

// Calculate answer
//
int zeroIndex = data.IndexOf((0, true));

int a = (zeroIndex + 1000) % data.Count;
int b = (zeroIndex + 2000) % data.Count;
int c = (zeroIndex + 3000) % data.Count;

int aVal = data[a].item;
int bVal = data[b].item;
int cVal = data[c].item;

int result = aVal + bVal + cVal;

watch.Stop();
Console.WriteLine($"Result {result}, completed in {watch.ElapsedMilliseconds}ms");
