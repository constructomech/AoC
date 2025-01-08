
var cache = new Dictionary<(int player1score, int player2score, int player1pos, int player2pos, int turn), (long player1score, long player2score)>();
int[] threeQuantumDieRolls = { 3, 4, 4, 4, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 8, 8, 8, 9 };

(long player1wins, long player2wins) = QuantumGame(0, 0, 8, 4, 0);

Console.WriteLine("Player 1 wins in {0} universes", player1wins);
Console.WriteLine("Player 2 wins in {0} universes", player2wins);


(long player1wins, long player2wins) QuantumGame(int player1score, int player2score, int player1pos, int player2pos, int turn) {
    if (player1score >= 21) return (1, 0);
    if (player2score >= 21) return (0, 1);

    (long player1score, long player2score) result;
    if (cache.TryGetValue((player1score, player2score, player1pos, player2pos, turn), out result)) return result;

    foreach (int dieRoll in threeQuantumDieRolls) {
        (long player1score, long player2score) localResult;
        if (turn == 0) {
            int newPos = (player1pos + dieRoll - 1) % 10 + 1;
            int newScore = player1score + newPos;
            localResult = QuantumGame(newScore, player2score, newPos, player2pos, 1);
        } else {
            int newPos = (player2pos + dieRoll - 1) % 10 + 1;
            int newScore = player2score + newPos;
            localResult = QuantumGame(player1score, newScore, player1pos, newPos, 0);
        }

        result.player1score += localResult.player1score;
        result.player2score += localResult.player2score;
    }

    cache.Add((player1score, player2score, player1pos, player2pos, turn), result);
    return result;
}