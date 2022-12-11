using System.IO;
using System.Numerics;
using System.Collections.Generic;

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
            monkey.Items.Add(new Item(Convert.ToInt32(item)));
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

monkies[0].WorryFunction = (old => old.MultiplyBy(19));
monkies[1].WorryFunction = (old => old.Plus(6));
monkies[2].WorryFunction = (old => old.Squared());
monkies[3].WorryFunction = (old => old.Plus(3));

// monkies[0].WorryFunction = (old => old * 13);
// monkies[1].WorryFunction = (old => old + 3);
// monkies[2].WorryFunction = (old => old + 6);
// monkies[3].WorryFunction = (old => old + 2);
// monkies[4].WorryFunction = (old => old * old);
// monkies[5].WorryFunction = (old => old + 4);
// monkies[6].WorryFunction = (old => old * 7);
// monkies[7].WorryFunction = (old => old + 7);

for (int round = 0; round < 20; round++) {

    foreach (var monkey in monkies) {

        // Inspect each item
        for (int i = 0; i < monkey.Items.Count; i++) {
            
            monkey.InspectedCount++;

            // Update worry
            Item newWorry = monkey.WorryFunction(monkey.Items[i]);

            if (newWorry.IsDivisibleBy(monkey.DivisibleBy)) {
                monkies[monkey.ThrowToIfTrue].Items.Add(newWorry);
            }
            else {
                monkies[monkey.ThrowToIfFalse].Items.Add(newWorry);
            }
        }

        // Throw the items
        monkey.Items.Clear();
    }

//    if ((round + 1) % 1000 == 0 || (round + 1) == 20 || round == 0) {
        Console.WriteLine("Round {0}:", round + 1);
        for (int i = 0; i < monkies.Count; i++) {
            Console.WriteLine("Monkey {0} inspected items {1} times.", i, monkies[i].InspectedCount);
        }
//    }
}


static class Fun {
    public static string Parse(string input) {
        return input;
    }
}

public class Item {

    public Item(int startingValue) {
        this.Constant = startingValue;
        this.FactorCounts = new Dictionary<int, int>();
    }

    public bool IsDivisibleBy(int operand) {
        if (this.Constant % operand == 0) {
            foreach (var (factor, count) in this.FactorCounts) {
                if (count % operand == 0) {
                    return true;  // If any of the multiplicands are divisble by the operand, then the total is (since the constant part is)
                }
            }
        }
        return false;
    }

    public Item Plus(int c) {
        this.Constant += c;
        return this;
    }

    public Item Squared() {
        return this;        
    }

    public Item MultiplyBy(int x) {
        if (!this.FactorCounts.ContainsKey(x)) {
            this.FactorCounts.Add(x, 1);
        }
        else {
            this.FactorCounts[x]++;
        }
        return this;
    }

    public Dictionary<int, int> FactorCounts { get; set; } 

    public int Constant { get; set; }
}

public class Monkey {
    public Monkey()
    {
        this.Items = new List<Item>();
    }

    public List<Item> Items { get; private set;}

    public int InspectedCount { get; set; }

    public int DivisibleBy { get; set; }

    public int ThrowToIfTrue { get; set; }
    public int ThrowToIfFalse { get; set; }

    public Func<Item, Item> WorryFunction { get; set; }
}