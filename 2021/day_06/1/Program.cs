using System.IO;
using System.Collections.Generic;

List<Population> fish = new List<Population>();

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            var split = line.Split(',');
            foreach (var item in split) {
                int age = Convert.ToInt32(item);
                AddFish(fish, age, 1);
            }
        }
    }
}

for (int day = 0; day < 80; day++) {
    //Console.Write("Day {0}: ", day);

    long newFish = 0;

    for (int i = 0; i < fish.Count; i++) {
        //Console.Write("{0},", fish[i]);

        fish[i].age -= 1;

        if (fish[i].age == -1) {
            fish[i].age = 6;
            newFish += fish[i].quantity;
            //Console.Write("[Add 8] ");
        }
    }

    AddFish(fish, 8, newFish);
}

long totalFish = 0;
foreach (var pop in fish) {
    totalFish += pop.quantity;
}

Console.WriteLine("{0}", totalFish);


void AddFish(List<Population> all, int age, long quantity) {
    Population targetPop = null;

    foreach (var pop in all) {
        if (pop.age == age) {
            targetPop = pop;
        }
    }
    if (targetPop == null) {
        targetPop = new Population() {age = age, quantity = 0};
        all.Add(targetPop);
    }

    targetPop.quantity += quantity;
}

class Population {
    public long quantity;
    public int age;
}

