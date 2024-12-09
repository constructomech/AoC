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

void PackWholeFile(List<Node> nodes) {
    int targetIdx = nodes.Count - 1;

    while (targetIdx > 0) {
        // Update the targetIdx
        while (nodes[targetIdx].IsFree()) {
            targetIdx--;
            if (targetIdx <= 0) {
                return;
            }
        }
        var fileBlock = nodes[targetIdx];

        // Find the first free block large enough
        var freeIdx = 0;
        while (!nodes[freeIdx].IsFree() || nodes[freeIdx].length < fileBlock.length) {
            freeIdx++;
            if (freeIdx >= targetIdx) {
                break;
            }            
        }
        if (freeIdx >= targetIdx) {
            // This block doesn't fit anywhere, move on to the next target block
            targetIdx--;
            continue;
        }

        var freeBlock = nodes[freeIdx];

        if (freeBlock.length > fileBlock.length) {
            // Split into two
            nodes[freeIdx] = new Node(fileBlock.fileNumber, fileBlock.length);
            nodes[targetIdx] = new Node(-1, fileBlock.length);
            nodes.Insert(freeIdx + 1, new Node(-1, freeBlock.length - fileBlock.length));
            
        } else if (freeBlock.length == fileBlock.length) {
            // Just move the file and get rid of the old one and it's preceding free space            
            nodes[freeIdx] = new Node(fileBlock.fileNumber, fileBlock.length);
            nodes[targetIdx] = new Node(-1, fileBlock.length);

        } else { // freeBlock.length < fileBlock.length
            throw new InvalidOperationException();
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

    // PrintFS(nodes);

    PackWholeFile(nodes);

    // PrintFS(nodes);

    result = ComputeChecksum(nodes);

    Console.WriteLine($"Result: {result}");
}

record Node ( int fileNumber, int length) {
    public bool IsFree() {
        return fileNumber == -1;
    }
}
