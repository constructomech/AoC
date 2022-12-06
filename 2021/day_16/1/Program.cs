using BKSystem.IO;

List<string> lines = new List<string>();
var stream = new BitStream();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        string? line = reader.ReadLine();
        if (line != null) {
            foreach (char c in line) {
                int val = Convert.ToInt32(c.ToString(), 16);
                stream.Write(val, 0, 4);
            }
        }
    }
}

stream.Position = 0;

// 3 bit version
// 3 bit type ID (type id 4 is a literal, padded with 0 until length is multiple of 4 bits, 
//    each group is prefixed by 1 until the last which has a 0 prefix -- groups of 5 bits)
// 1 bit length type id (0 - next 15 bits are a number that represents the length in bits of the sub-packets,
//    1 - next 11 bits are a number that represents the number of sub-packets immediately contained by this packet)
// Next 11 or 15 bits are the length type id data.

int versionSum = 0;
parsePacket(stream);

Console.WriteLine("Position: {0}, Length: {1}", stream.Position, stream.Length);

Console.WriteLine("Version sum: {0}", versionSum);

int parsePacket(BitStream stream) {
    int bitsRead = 0;
    byte version, packetTypeId;

    bitsRead += stream.Read(out version, 0, 3);
    versionSum += version;

    bitsRead += stream.Read(out packetTypeId, 0, 3);
    switch(packetTypeId) {
        case 4: // literal
            long value;
            bitsRead += readLiteral(stream, out value);
            Console.WriteLine("[Literal v{0}: {1}]", version, value);
            break;
        default: // operator
            Console.WriteLine("(Operator v{0}", version);

            bool lengthTypeIsCountOfSubpackets;
            bitsRead += stream.Read(out lengthTypeIsCountOfSubpackets);

            if (lengthTypeIsCountOfSubpackets) {                
                int count;
                bitsRead += stream.Read(out count, 0, 11);
                Console.WriteLine("#Count of subpackets: {0}#", count);
                for (int i = 0; i < count; i++) {
                    bitsRead += parsePacket(stream);
                }
            } 
            else {  // Length type is bit count of subpackets
                int length;
                bitsRead += stream.Read(out length, 0, 15);
                Console.WriteLine("#Len of subpacket in bits: {0}#", length);
                while (length > 0) {
                    int childBitsRead = parsePacket(stream);
                    bitsRead += childBitsRead;
                    length -= childBitsRead;
                    if (length < 0) throw new InvalidProgramException();
                }
            }

            Console.WriteLine(")");
            break;
    }

    // Console.WriteLine("Version: {0}", version);
    // Console.WriteLine("PacketType: {0}", packetTypeId);

    return bitsRead;
}

int readLiteral(BitStream stream, out long result) {
    bool more;
    int countOfBits = 0, bitsRead = 0;
    result = 0;
    do {
        result = result << 4;
        bitsRead += stream.Read(out more);
        byte data;
        bitsRead += stream.Read(out data, 0, 4);
        countOfBits += 4;
    } while (more);

    if (countOfBits > 64) throw new OverflowException();

    return bitsRead;
}
