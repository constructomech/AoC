using System.IO;
using System.Collections.Generic;

long result = 0;

var monkies = new List<Monkey>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        reader.ReadLine();  // Moneky #
        string startingItems = reader.ReadLine();
        string operation = reader.ReadLine();
        string test = reader.ReadLine();
        string testTrueClause = reader.ReadLine();
        string testFalseClause = reader.ReadLine();
        reader.ReadLine();

        var monkey = new Monkey();

        var parts = startingItems.Split(':');
        foreach (var item in parts[1].Split(',')) {
            monkey.Items.Add(Convert.ToInt32(item));
        }

        // Skip ops, adding manually.
        
        parts = test.Split(' ');
        monkey.DivisibleBy = Convert.ToInt32(parts.Last());

        parts = testTrueClause.Split(' ');
        monkey.ThrowToIfTrue = Convert.ToInt32(parts.Last());

        parts = testFalseClause.Split(' ');
        monkey.ThrowToIfFalse = Convert.ToInt32(parts.Last());

        monkies.Add(monkey);
    }
}

// monkies[0].WorryFunction = (old => old * 19);
// monkies[1].WorryFunction = (old => old + 6);
// monkies[2].WorryFunction = (old => old * old);
// monkies[3].WorryFunction = (old => old + 3);

monkies[0].WorryFunction = (old => old * 13);
monkies[1].WorryFunction = (old => old + 3);
monkies[2].WorryFunction = (old => old + 6);
monkies[3].WorryFunction = (old => old + 2);
monkies[4].WorryFunction = (old => old * old);
monkies[5].WorryFunction = (old => old + 4);
monkies[6].WorryFunction = (old => old * 7);
monkies[7].WorryFunction = (old => old + 7);

for (int round = 0; round < 20; round++) {

    foreach (var monkey in monkies) {

        // Inspect each item
        for (int i = 0; i < monkey.Items.Count; i++) {
            
            monkey.InspectedCount++;

            // Update worry
            int newWorry = monkey.WorryFunction(monkey.Items[i]);

            newWorry /= 3;

            if (newWorry % monkey.DivisibleBy == 0) {
                monkies[monkey.ThrowToIfTrue].Items.Add(newWorry);                
            }
            else {
                monkies[monkey.ThrowToIfFalse].Items.Add(newWorry);
            }
        }

        // Throw the items
        monkey.Items.Clear();
    }
}

for (int i = 0; i < monkies.Count; i++) {
    Console.WriteLine("Monkey {0} inspected items {1} times.", i, monkies[i].InspectedCount);
}

Console.WriteLine("Result: {0}", result);


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}

public class Monkey {
    public Monkey()
    {
        this.Items = new List<int>();
    }

    public List<int> Items { get; private set;}

    public int InspectedCount { get; set; }

    public int DivisibleBy { get; set; }

    public int ThrowToIfTrue { get; set; }
    public int ThrowToIfFalse { get; set; }

    public Func<int, int> WorryFunction { get; set; }
}