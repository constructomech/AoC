﻿using System.Diagnostics;

// <-- 2 units -->|
//                |____
//                ^
//                |      
//                3 units
//                |
//         Highest block

Stopwatch watch = new Stopwatch();
watch.Start();

// Parse
//
var input = File.ReadAllLines("input.txt");
var wind = new List<int>();

foreach (char dir in input[0])
{
    if (dir == '<') wind.Add(-1);
    else if (dir == '>') wind.Add(1);
    else throw new Exception();
}

// Set up pices
// 
var pieces = new List<List<(long x, long y)>>();
// x ->  ^
//       |
//       y

// ####   (0, 0), (1, 0), (2, 0), (3, 0)
pieces.Add(new List<(long x, long y)>() { (0, 0), (1, 0), (2, 0), (3, 0) });

// .#.    (1, 0), (0, 1), (1, 1), (2, 1), (1, 2)
// ###
// .#.
pieces.Add(new List<(long x, long y)>() { (1, 0), (0, 1), (1, 1), (2, 1), (1, 2) });

// ..#    (0, 0), (1, 0), (2, 0), (2, 1), (2, 2)
// ..#
// ###
pieces.Add(new List<(long x, long y)>() { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) });

// #      (0, 0), (1, 0), (2, 0), (3, 0)
// #
// #
// #
pieces.Add(new List<(long x, long y)>() { (0, 0), (0, 1), (0, 2), (0, 3) });

// ##     (0, 1), (1, 0), (1, 0), (1, 1)
// ##
pieces.Add(new List<(long x, long y)>() { (0, 0), (1, 0), (0, 1), (1, 1) });



// Stat simulation
//
var board = new Board();
var windIndex = 0;
var pieceIndex = 0;

for (long i = 0; i < 2022; i++)
{
    Piece piece = GetNextPiece();
    piece.LowerLeft = (2, board.HighWaterMark + 4);

    bool resting = false;
    while (!resting)
    {
        // First Wind  -- don't care if move doesn't move left or right, doesn't impact gameplay.
        int xWindOffset = GetNextWindXOffset();
        piece.Move(board, (xWindOffset, 0));

        // Second Fall
        resting = !piece.Move(board, (0, -1));
        if (resting)
        {
            board.Commit(piece.GetPieceLocations());
//            board.Print();
        }
    }
}

watch.Stop();
Console.WriteLine($"Total Height: {board.HighWaterMark + 1}, Completed in {watch.ElapsedMilliseconds}ms");


int GetNextWindXOffset()
{
    var result = wind[windIndex];
    windIndex++;
    if (windIndex >= wind.Count) windIndex = 0;
    return result;
}

Piece GetNextPiece()
{
    var result = pieces[pieceIndex];
    pieceIndex++;
    if (pieceIndex >= pieces.Count) pieceIndex = 0;
    return new Piece(result);
}

class Piece
{
    public Piece(List<(long x, long y)> points)
    {
        this.points = points;
    }

    public bool Move(Board board, (long x, long y) offset)
    {
        if (board.CheckFree(this.GetPieceLocations(offset)))
        {
            LowerLeft = (LowerLeft.x + offset.x, LowerLeft.y + offset.y);
            return true;
        }
        return false;
    }

    public IEnumerable<(long x, long y)> GetPieceLocations((long x, long y) offset = default)
    {
        foreach (var point in points)
        {
            yield return (point.x + LowerLeft.x + offset.x, point.y + LowerLeft.y + offset.y);
        }
    }

    public (long x, long y) LowerLeft { get; set; }

    private List<(long x, long y)> points;
}

class Board
{
    public char this[(long x, long y) point]
    {
        get
        {
            if (point.x <= leftWallX || point.x >= rightWallX)
            {
                return '|';
            }
            if (point.y <= floorY)
            {
                return '-';
            }
            else
            {
                char result;
                if (map.TryGetValue(point, out result))
                {
                    return result;
                }
                return '.';
            }
        }
    }

    public bool CheckFree(IEnumerable<(long x, long y)> points) 
    {
        foreach (var point in points)         
        {
            if (this[point] != '.')
            {
                return false;                               
            }
        }
        return true;
    }

    public void Commit(IEnumerable<(long x, long y)> points) 
    {
        foreach (var point in points)
        {
            if (this.HighWaterMark < point.y)
            {
                this.HighWaterMark = point.y;
            }
            this.map[point] = '#';
        }
    }

    public void Print() 
    {
        Console.WriteLine();
        for (long y = this.HighWaterMark; y >= -1; y--)
        {
            for (int x = -1; x <= 7; x++) 
            {
                Console.Write(this[(x, y)]);
            }
            Console.WriteLine();
        }        
    }

    public long HighWaterMark = -1;

    private Dictionary<(long x, long y), char> map = new Dictionary<(long x, long y), char>();
    private int floorY = -1;
    private int leftWallX = -1;  // Not valid X
    private int rightWallX = 7;  // Not valid X
}