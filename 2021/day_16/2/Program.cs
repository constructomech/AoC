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

long result;
parsePacket(stream, out result);

Console.WriteLine("Position: {0}, Length: {1}", stream.Position, stream.Length);

Console.WriteLine("Result: {0}", result);

int parsePacket(BitStream stream, out long result) {
    int bitsRead = 0;
    byte version, packetTypeId;

    bitsRead += stream.Read(out version, 0, 3);
    bitsRead += stream.Read(out packetTypeId, 0, 3);
    switch(packetTypeId) {

        case 4: // literal
            bitsRead += readLiteral(stream, out result);
            Console.WriteLine("[Literal v{0}: {1}]", version, result);
            break;

        default: // operator
            Operator op = (Operator)packetTypeId;
            Console.WriteLine("(Operator {0} v{1}", op, version);

            bool lengthTypeIsCountOfSubpackets;
            bitsRead += stream.Read(out lengthTypeIsCountOfSubpackets);

            List<long> results = new List<long>();

            if (lengthTypeIsCountOfSubpackets) {                
                int count;
                bitsRead += stream.Read(out count, 0, 11);
                //Console.WriteLine("#Count of subpackets: {0}#", count);
                for (int i = 0; i < count; i++) {
                    long subResult;
                    bitsRead += parsePacket(stream, out subResult);
                    results.Add(subResult);
                }
            } 
            else {  // Length type is bit count of subpackets
                int length;
                bitsRead += stream.Read(out length, 0, 15);
                //Console.WriteLine("#Len of subpacket in bits: {0}#", length);
                while (length > 0) {
                    long subResult;
                    int childBitsRead = parsePacket(stream, out subResult);
                    results.Add(subResult);

                    bitsRead += childBitsRead;
                    length -= childBitsRead;
                    if (length < 0) throw new InvalidProgramException();
                }
            }

            result = performOp(op, results);

            Console.WriteLine(")");
            break;
    }

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
        result += data;
        countOfBits += 4;
    } while (more);

    if (countOfBits > 64) throw new OverflowException();

    return bitsRead;
}

long performOp(Operator op, List<long> args) {
    long result = 0;
    switch(op) {
        case Operator.Sum:
            result = args.Sum();
            break;
        case Operator.Product:
            result = 1;
            args.ForEach((item) => result *= item);
            break;
        case Operator.Minimum:
            result = long.MaxValue;
            args.ForEach((item) => result = Math.Min(result, item));
            break;
        case Operator.Maximum:
            result = long.MinValue;
            args.ForEach((item) => result = Math.Max(result, item));
            break;
        case Operator.GreatherThan:
            if (args.Count != 2) throw new InvalidDataException();
            result = (args[0] > args[1]) ? 1 : 0;
            break;
        case Operator.LessThan:
            if (args.Count != 2) throw new InvalidDataException();
            result = (args[0] < args[1]) ? 1 : 0;
            break;
        case Operator.EqualTo:
            result = (args[0] == args[1]) ? 1 : 0;
            if (args.Count != 2) throw new InvalidDataException();
            break;
    }
    return result;
}

enum Operator {
    Sum = 0,
    Product = 1,

    Minimum = 2,

    Maximum = 3,

    GreatherThan = 5,

    LessThan = 6,

    EqualTo = 7,
}