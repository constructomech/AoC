using System.IO;
using System.Collections.Generic;


// The first column is what your opponent is going to play: A for Rock, B for Paper, and C for Scissors.
// The second column, you reason, must be what you should play in response: X for Rock, Y for Paper, and Z for Scissors.

var elfMoveToSymbol = new Dictionary<string, Symbol>() { { "A", Symbol.Rock }, { "B", Symbol.Paper}, { "C", Symbol.Scissors} };
var myMoveToSymbol = new Dictionary<string, Symbol>() { { "X", Symbol.Rock }, { "Y", Symbol.Paper}, { "Z", Symbol.Scissors} };

int totalScore = 0;

using (StreamReader reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string? line = reader.ReadLine();
        if (line != null) {
            string[] parts = line.Split(' ');
            var elfMove = elfMoveToSymbol[parts[0]]; 
            var myMove = myMoveToSymbol[parts[1]];

            int score = 0;

            score += (int)myMove;

            var outcome = Fun.Play(elfMove, myMove);

            score += (int)outcome;

            totalScore += score;
        }
    }
}

// See https://aka.ms/new-console-template for more information
Console.WriteLine("{0}", totalScore);

enum Symbol {
    Rock = 1,
    Paper = 2, 
    Scissors = 3,
}

enum Outcome {
    Lost = 0,
    Draw  = 3,
    Win = 6,
}

static class Fun {
    public static Outcome Play(Symbol elfPlay, Symbol myPlay) {
        switch (elfPlay)
        {
            case Symbol.Paper:
                switch (myPlay)
                {
                    case Symbol.Paper:      return Outcome.Draw;
                    case Symbol.Rock:       return Outcome.Lost;
                    case Symbol.Scissors:   return Outcome.Win;
                }
                break;
            case Symbol.Rock:
                switch (myPlay)
                {
                    case Symbol.Paper:      return Outcome.Win;
                    case Symbol.Rock:       return Outcome.Draw;
                    case Symbol.Scissors:   return Outcome.Lost;
                }
                break;
            case Symbol.Scissors:
                switch (myPlay)
                {
                    case Symbol.Paper:      return Outcome.Lost;
                    case Symbol.Rock:       return Outcome.Win;
                    case Symbol.Scissors:   return Outcome.Draw;
                }
                break;
        }
        return Outcome.Draw;
    }
}
