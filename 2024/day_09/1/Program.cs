using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void PrintFS(List<Node> nodes) {
    foreach (var node in nodes) {
        var c = '.';
        if (!node.IsFree()) {
            c = Convert.ToChar('0' + node.fileNumber);
        }
        for (var i = 0; i < node.length; i++) {
            Console.Write(c);
        }
    }
    Console.WriteLine();
}

void Pack(List<Node> nodes) {
    int freeIdx = 1;

    while (freeIdx < nodes.Count) {
        int targetIdx = nodes.Count - 1;

        var freeBlock = nodes[freeIdx];
        var fileBlock = nodes[targetIdx];
        if (freeBlock.length > fileBlock.length) {
            // Split into two
            nodes[freeIdx] = new Node(fileBlock.fileNumber, fileBlock.length);
            nodes.Insert(freeIdx + 1, new Node(-1, freeBlock.length - fileBlock.length));
            nodes.RemoveRange(targetIdx, 2);

        } else if (freeBlock.length == fileBlock.length) {
            // Just move the file and get rid of the old one and it's preceding free space            
            nodes[freeIdx] = new Node(fileBlock.fileNumber, fileBlock.length);
            nodes.RemoveRange(targetIdx - 1, 2);

        } else { // freeBlock.length < fileBlock.length
            // Insert the file and adjust the free sapce
            nodes[freeIdx] = new Node(fileBlock.fileNumber, freeBlock.length);
            nodes[targetIdx] = new Node(fileBlock.fileNumber, fileBlock.length - freeBlock.length);
        }

        // Advance to next free index
        while (freeIdx < nodes.Count && !nodes[freeIdx].IsFree()) {
            freeIdx++;
        }
    }
}

long ComputeChecksum(List<Node> nodes) {
    var checksum = 0L;

    long pos = 0;

    foreach (var node in nodes) {
        for (var i = 0; i < node.length; i++) {
            if (!node.IsFree()) {
                checksum += pos * node.fileNumber;
            }
            pos++;
        }
    }

    return checksum;    
}

void Run(string[] input) {
    long result = 0;
    var nodes = new List<Node>();
    int currentFileId = 0;
    bool free = false;

    if (input.Length > 1) throw new InvalidOperationException();
    var line = input[0];

    for (int i = 0; i < line.Length; i++) {
        var currentNode = new Node(free ? -1 : currentFileId, line[i] - '0');
        nodes.Add(currentNode);

        if (free) {
            currentFileId++;
        }
        free = !free;
    }

    //PrintFS(nodes);

    Pack(nodes);

    //PrintFS(nodes);

    result = ComputeChecksum(nodes);

    Console.WriteLine($"Result: {result}");
}

record Node ( int fileNumber, int length) {
    public bool IsFree() {
        return fileNumber == -1;
    }
}
