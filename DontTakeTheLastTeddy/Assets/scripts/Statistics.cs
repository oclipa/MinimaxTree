using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Record the statistics for all the games
/// </summary>
public static class Statistics {

    // store the statistics in a dictionary
    private static Dictionary<Difficulties, PlayerScores> statistics = new Dictionary<Difficulties, PlayerScores>(new DifficultiesUniqueIdEqualityComparer());

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public static void Initialize () {
        EventManager.AddGameOverListener(UpdateStatistics);
	}

    /// <summary>
    /// Update the statistics with the winner of the latest game (and the difficult levels)
    /// </summary>
    /// <param name="playerName">Player name.</param>
    /// <param name="difficulties">Difficulties.</param>
    static void UpdateStatistics (PlayerName playerName, Difficulties difficulties) 
    {
        if (statistics.ContainsKey(difficulties))
        {
            statistics[difficulties].AddScore(playerName, 1);
        }
        else
        {
            PlayerScores playerScores = new PlayerScores();
            playerScores.AddScore(playerName, 1);
            statistics.Add(difficulties, playerScores);
        }
    }

    /// <summary>
    /// Get the current score for the selected player, for games with the
    /// specified difficulty levels.
    /// </summary>
    /// <returns>The score.</returns>
    /// <param name="playerName">Player name.</param>
    /// <param name="difficulties">Difficulties.</param>
    public static int GetScore(PlayerName playerName, Difficulties difficulties)
    {
        if (statistics.ContainsKey(difficulties))
        {
            return statistics[difficulties].GetScore(playerName);
        }
        else
        {
            return 0;
        }
    }
}
