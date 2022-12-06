
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

List<long> lineScores = new List<long>();

foreach (string line in lines) {

    Stack<CharType> stack = new Stack<CharType>();
    long lineScore = 0;
    bool error = false;

    foreach (char c in line) {
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
            // Skip illegal lines
            break;
        }
    }

    if (!error) {
        while (stack.Count > 0) {
            CharType charType = stack.Pop();
            lineScore *= 5;
            switch (charType) {
                case CharType.Parenthesis:
                    lineScore += 1;
                    break;
                case CharType.SquareBracket:
                    lineScore += 2;
                    break;
                case CharType.CurlyBracket:
                    lineScore += 3;
                    break;
                case CharType.AngleBracket:
                    lineScore += 4;
                    break;
            }
        }

        lineScores.Add(lineScore);
    }
}

lineScores.Sort();
long score = lineScores[lineScores.Count / 2];

Console.WriteLine("Score: {0}", score);

enum CharType {
    Parenthesis,
    SquareBracket,
    CurlyBracket,
    AngleBracket,
}

