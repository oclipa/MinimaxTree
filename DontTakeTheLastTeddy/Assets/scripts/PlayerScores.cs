using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScores
{
    private Dictionary<PlayerName, int> scores;

    public PlayerScores()
    {
        scores = new Dictionary<PlayerName, int>();
    }

    public void AddScore(PlayerName player, int score)
    {
        if (scores.ContainsKey(player))
            scores[player] += score;
        else
            scores.Add(player, score);
    }

    public int GetScore(PlayerName player)
    {
        int score = 0;
        if (scores.TryGetValue(player, out score))
            return score;

        return 0;
    }

    public override string ToString()
    {
        int scoreP1 = 0;
        scores.TryGetValue(PlayerName.Player1, out scoreP1);
        int scoreP2 = 0;
        scores.TryGetValue(PlayerName.Player2, out scoreP2);
        return "P1 = " + scoreP1 + ", P2 = " + scoreP2;
    }
}
