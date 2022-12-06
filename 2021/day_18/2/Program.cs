var snailfishNumbers = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream) {
        // Example:  [[[[1,2],[3,4]],[[5,6],[7,8]]],9]
        string? line = reader.ReadLine();
        if (line != null) {
            snailfishNumbers.Add(line);
        }
    }
}

// Element accum = null;

// foreach (var number in snailfishNumbers) {
//     if (accum == null) {
//         accum = number;
//     }
//     else {
//         accum = add(accum, number);
//         Console.WriteLine("Sum:     {0}", accum);

//         while (reduce(accum)) {
//         }
//     }
// }

// Console.WriteLine("Magnitude: {0}", accum.Magnitude);

long maxMagnitude = 0;

for (int i = 0; i < snailfishNumbers.Count; i++) {
    for (int j = 0; j < snailfishNumbers.Count; j++) {
        if (j != i) {
            Element first = parse(snailfishNumbers[i]);
            Element second = parse(snailfishNumbers[j]);
            Element result = add(first, second);

            while (reduce(result)) {
            }

            long magnitude = result.Magnitude;
            if (magnitude > maxMagnitude) {
                maxMagnitude = magnitude;
                Console.WriteLine("Found Max: {0} + {1}", first, second);
            }

            Console.WriteLine("---");
        }
    }
}

Console.WriteLine("Greatest magnitude: {0}", maxMagnitude);

Element parse(string line) {
    int pos = 0;
    Element priorReg = null;
    Element current = parseElement(line, ref pos, ref priorReg);
    return current;
}

Element add(Element left, Element right) {
    Element rightMostLeft = find(left, (element) => element.Value != null && element.SubsequentRegular == null);
    Element leftMostRight = find(right, (element) => element.Value != null && element.PriorRegular == null);

    rightMostLeft.SubsequentRegular = leftMostRight;
    leftMostRight.PriorRegular = rightMostLeft;

    var result = new Element() { Left = left, Right = right };

    Console.WriteLine("Sum:     {0}", result);
    return result;
}

Element find(Element root, Predicate<Element> predicate) {
    var stack = new Stack<Element>();
    stack.Push(root);
    while (stack.Count > 0) {
        Element current = stack.Pop();

        if (predicate(current)) {
            return current;
        }

        if (current.Value == null) {
            stack.Push(current.Left);
            stack.Push(current.Right);
        }
    }
    return null;
}

bool reduce(Element number) {    

    // Explode - If any pair is nested inside 4 pairs
    var explodeStack = new Stack<(Element, int)>();
    explodeStack.Push((number, 1));

    while (explodeStack.Count > 0) {
        (Element current, int depth) = explodeStack.Pop();

        // If current represents a pair
        if (current.Value == null) {
            if (depth == 5 && current.Left.Value != null && current.Right.Value != null) {
                // Found the first node to explode.

                // Add left to first regular number to left, if any
                if (current.Left.PriorRegular != null) {
                    current.Left.PriorRegular.Value += current.Left.Value;

                    // Fix up the regular chain
                    current.Left.PriorRegular.SubsequentRegular = current;
                }

                // Add right to first regular number to
                if (current.Right.SubsequentRegular != null) {
                    current.Right.SubsequentRegular.Value += current.Right.Value;

                    // Fix up the regular chain
                    current.Right.SubsequentRegular.PriorRegular = current;
                }

                // Replace this tuple with the value 0
                current.PriorRegular = current.Left.PriorRegular;
                current.SubsequentRegular = current.Right.SubsequentRegular;
                current.Left = null;
                current.Right = null;
                current.Value = 0;

                Console.WriteLine("Explode: {0}", number);
                return true;
            }

            int childDepth = depth + 1;
            explodeStack.Push((current.Right, childDepth));
            explodeStack.Push((current.Left, childDepth));
        }
    }

    // No explode, so Split
    var splitStack = new Stack<Element>();
    splitStack.Push(number);
    while (splitStack.Count > 0) {
        Element current = splitStack.Pop();

        if (current.Value != null && current.Value >= 10) {
            Element left = new Element() { Value = current.Value / 2 };
            Element right = new Element() { Value = (int)Math.Ceiling((double)current.Value / 2) };

            // Fixup the regular chain
            if (current.PriorRegular != null) {
                current.PriorRegular.SubsequentRegular = left;
            }
            if (current.SubsequentRegular != null) {
                current.SubsequentRegular.PriorRegular = right;
            }
            left.PriorRegular = current.PriorRegular;
            left.SubsequentRegular = right;
            right.PriorRegular = left;
            right.SubsequentRegular = current.SubsequentRegular;

            // Adjust this element to a pair
            current.Value = null;
            current.Left = left;
            current.Right = right;
            current.PriorRegular = null;
            current.SubsequentRegular = null;

            Console.WriteLine("Split:   {0}", number);
            return true;
        }

        if (current.Value == null) {
            splitStack.Push(current.Right);
            splitStack.Push(current.Left);
        }
    }
    return false;
}

Element parseElement(string line, ref int pos, ref Element priorReg) {
    Element rootElement = new Element();
    switch(line[pos]) {
        case '[':
            pos++; // Skip open bracket
            rootElement.Left = parseElement(line, ref pos, ref priorReg);
            pos++; // Skip comma
            rootElement.Right = parseElement(line, ref pos, ref priorReg);
            pos++; // Skip closing bracket
            break;
            
        default: // 0-9
            int index = line.IndexOfAny(new char[] { ',', ']' }, pos);
            string num = line.Substring(pos, index - pos);
            rootElement.Value = Convert.ToInt32(num);
            pos = index;

            if (priorReg != null) {
                priorReg.SubsequentRegular = rootElement;
            }
            rootElement.PriorRegular = priorReg;
            priorReg = rootElement;

            break;
    }
    return rootElement;
}

enum NumParseState {
    First,
    Second
}
class Element {

    public Element() {
        Value = null;
        Left = null;
        Right = null;
    }

    public long Magnitude {
        get {
            if (Value != null) {
                return (long)Value;
            }
            else {
                return 3 * Left.Magnitude + 2 * Right.Magnitude;
            }
        }
    }

    public override string ToString() {
        if (Value != null) {
            return Value.ToString();
        }
        else {
            return '[' + Left.ToString() + ',' + Right.ToString() + ']';
        }
    }

    public int? Value {
        get; set;
    }

    public Element? Left {
        get; set;
    }
    public Element? Right {
        get; set;
    }

    public Element? PriorRegular {
        get; set;
    }

    public Element? SubsequentRegular {
        get; set;
    }
}
