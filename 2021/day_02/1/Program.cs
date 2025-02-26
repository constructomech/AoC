﻿using System.Diagnostics;

Stopwatch watch = new Stopwatch();
watch.Start();
var input = File.ReadAllLines("input.txt");
Run(input);
watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


void Run(string[] input) {
    var instructions = input.Select(line => {
        var parts = line.Split(' ');
        return (type: parts[0], value: int.Parse(parts[1]));
    }).ToList();

    int depth = 0;
    int position = 0;

    foreach (var instruction in instructions) {
        switch (instruction.type) {
            case "forward":
                position += instruction.value;
                break;
            case "down":
                depth += instruction.value;
                break;
            case "up":
                depth -= instruction.value;
                break;
        }
    }

    Console.WriteLine($"Result: {depth * position}");
}
