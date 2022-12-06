
List<string> lines = new List<string>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            lines.Add(line);
        }
    }
}

// Incomplete ok, discard corrupted

// int[] charCounts = new int[4];

Stack<CharType> stack = new Stack<CharType>();

int totalScore = 0;
foreach (string line in lines) {

    foreach (char c in line) {
        bool error = false;
        switch (c) {
            case '(':
                stack.Push(CharType.Parenthesis);
                break;
            case '[':
                stack.Push(CharType.SquareBracket);
                break;
            case '{':
                stack.Push(CharType.CurlyBracket);
                break;
            case '<':
                stack.Push(CharType.AngleBracket);
                break;
            case ')':
                error = (stack.Pop() != CharType.Parenthesis);
                break;
            case ']':
                error = (stack.Pop() != CharType.SquareBracket);
                break;
            case '}':
                error = (stack.Pop() != CharType.CurlyBracket);
                break;
            case '>':
                error = (stack.Pop() != CharType.AngleBracket);
                break;
        }

        if (error) {
            totalScore += illegalCharScore(c);
            break;
        }
    }
}

Console.WriteLine("Score: {0}", totalScore);


int illegalCharScore(char c) {
    switch (c) {
        case ')': return 3;
        case ']': return 57;
        case '}': return 1197;
        case '>': return 25137;
    }
    return 0;
}

enum CharType {
    Parenthesis,
    SquareBracket,
    CurlyBracket,
    AngleBracket,
}

