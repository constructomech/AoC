(int position, int score)[] players = new (int, int)[2];
players[0].position = 4;
players[1].position = 8;

Die die = new Die();

int playerTurn = 0;

while (players[0].score < 1000 && players[1].score < 1000) {
    int sum = 0;
    for (int roll = 0; roll < 3; roll++) {
        sum += die.Roll();
    }

    int newPosition = (players[playerTurn].position + sum -1) % 10 + 1;        
    players[playerTurn].position = newPosition;

    int newScore = players[playerTurn].score + newPosition;
    players[playerTurn].score = newScore;
    
    Console.WriteLine("Player {0} rolls {1} and moves to space {2} for a total score of {3}", playerTurn, sum, newPosition, newScore);

    playerTurn = (playerTurn + 1) % 2;
}

int result = die.RollCount * Math.Min(players[0].score, players[1].score);

Console.WriteLine("Result: {0}", result);


class Die {

    public int RollCount {
        get { return rollCount; }
    }

    public int Roll() {
        rollCount++;
        return current++;
    }

    int rollCount = 0;

    int current = 1;
}