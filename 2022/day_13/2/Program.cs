using System.IO;
using System.Collections.Generic;
using System.Text;

//If both values are integers, the lower integer should come first. If the left integer is lower than the right integer, the inputs are in the right order. 
//  If the left integer is higher than the right integer, the inputs are not in the right order. Otherwise, the inputs are the same integer; 
//  continue checking the next part of the input.
//
//If both values are lists, compare the first value of each list, then the second value, and so on. If the left list runs out of items first, 
//  the inputs are in the right order. If the right list runs out of items first, the inputs are not in the right order. If the lists are the same length 
//  and no comparison makes a decision about the order, continue checking the next part of the input.
//
//If exactly one value is an integer, convert the integer to a list which contains that integer as its only value, then retry the comparison. For example, 
// if comparing [0,0,0] and 2, convert the right value to [2] (a list containing 2); the result is then found by instead comparing [0,0,0] and [2].

var packets = new List<Packet>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine()!;
        if (line.Length > 0)
        {
            var packet = Packet.Parse(line);
            packets.Add(packet);
        }
    }
}

var divider0 = Packet.Parse("[[2]]");
var divider1 = Packet.Parse("[[6]]"); 
packets.Add(divider0);
packets.Add(divider1);

packets.Sort();

int index0 = 0;
int index1 = 0;

for (int i = 0; i < packets.Count; i++)
{
    if (packets[i].CompareTo(divider0) == 0) 
    {
        index0 = i + 1;
    }
    else if (packets[i].CompareTo(divider1) == 0)
    {
        index1 = i + 1;
    }
}

Console.WriteLine("Result: {0}", index0 * index1);


public class Packet : IComparable<Packet>
{
    public static Packet Parse(string input)
    {
        var result = new Packet();

        var stack = new Stack<List<object>>();
        int pos = 0;
        while (pos < input.Length)
        {
            if (input[pos] == '[')
            {
                var childList = new List<object>();
                if (result.Root == null)
                {
                    result.Root = childList;
                }
                else
                {
                    stack.Peek().Add(childList);
                }
                stack.Push(childList);
                pos++;
            }
            else if (input[pos] == ']')
            {
                stack.Pop();
                pos++;
            }
            else if (input[pos] == ',')
            {
                pos++;
            }
            else
            {
                var nextPos = input.IndexOfAny(new char[] { ',', '[', ']' }, pos);
                var token = input.Substring(pos, nextPos - pos);
                int value = Convert.ToInt32(token);
                stack.Peek().Add(value);
                pos = nextPos;
            }
        }

        Console.WriteLine(result.ToString());

        return result;
    }

    public int CompareTo(Packet? other)
    {
        return CompareElement(this.Root!, other!.Root!);
    }

    public static int CompareElement(object left, object right)
    {
        var leftList = left as List<object>;
        var rightList = right as List<object>;

        if (left is int && right is int)
        {
            int result = ((int)left).CompareTo(right);
            return result;
        }
        else if (left is List<object> && right is List<object>)
        {
            return CompareLists(leftList!, rightList!);
        }
        else if (left is List<object>)
        {
            // left is list, right is number
            var temp = new List<object>();
            temp.Add(right);
            return CompareLists(leftList!, temp);
        }
        else
        {
            // right is list, left is number
            var temp = new List<object>();
            temp.Add(left);
            return CompareLists(temp, rightList!);
        }
    }

    private static int CompareLists(List<object> leftList, List<object> rightList)
    {
        for (int cursor = 0; cursor < Math.Max(leftList!.Count, rightList!.Count); cursor++)
        {
            if (cursor >= leftList.Count && cursor < rightList.Count)
            {
                return -1;
            }
            else if (cursor >= rightList.Count && cursor < leftList.Count)
            {
                return 1;
            }
            else
            {
                int result = CompareElement(leftList[cursor], rightList[cursor]);
                if (result != 0)
                {
                    return result;
                }
            }
        }
        return 0;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToStringInternal(builder, Root!);
        return builder.ToString();
    }

    private static void ToStringInternal(StringBuilder builder, List<object> list)
    {
        builder.Append('[');
        bool first = true;
        foreach (var element in list)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.Append(',');
            }

            var childList = element as List<object>;
            if (childList != null)
            {
                ToStringInternal(builder, childList);
            }
            else
            {
                builder.Append((int)element);
            }
        }
        builder.Append(']');
    }

    public List<object>? Root = null;
}

