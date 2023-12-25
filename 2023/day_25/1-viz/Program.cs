using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics;


Stopwatch watch = new Stopwatch();
watch.Start();

var input = File.ReadAllLines("input.txt");

Fun.Run(input);

watch.Stop();
Console.WriteLine($"Completed in {watch.ElapsedMilliseconds}ms");


static class Fun {

    public static void Run(string[] data) {

        Console.WriteLine("digraph G {");
        Console.WriteLine("  graph [fontname = \"Handlee\"]");
        Console.WriteLine("  node [fontname = \"Handlee\"]");
        Console.WriteLine("  edge [fontname = \"Handlee\"]");

        foreach (var line in data) {
            var nodeNames = line.Split(':', ' ').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();

            foreach (var nodeName in nodeNames.Skip(1)) {
                Console.WriteLine($"{nodeNames[0]} -> {nodeName};");
            }
        }

        Console.WriteLine("}");
    }
}