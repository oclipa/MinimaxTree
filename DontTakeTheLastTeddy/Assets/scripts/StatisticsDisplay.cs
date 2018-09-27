using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the statistics
/// </summary>
public class StatisticsDisplay : MonoBehaviour
{
    [SerializeField]
    Text easyEasyPlayer1Wins;
    [SerializeField]
    Text easyEasyPlayer2Wins;

    [SerializeField]
    Text mediumMediumPlayer1Wins;
    [SerializeField]
    Text mediumMediumPlayer2Wins;

    [SerializeField]
    Text hardHardPlayer1Wins;
    [SerializeField]
    Text hardHardPlayer2Wins;

    [SerializeField]
    Text easyMediumPlayer1Wins;
    [SerializeField]
    Text easyMediumPlayer2Wins;

    [SerializeField]
    Text easyHardPlayer1Wins;
    [SerializeField]
    Text easyHardPlayer2Wins;

    [SerializeField]
    Text mediumHardPlayer1Wins;
    [SerializeField]
    Text mediumHardPlayer2Wins;

    /// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
        Difficulties difficulties = new Difficulties(Difficulty.Easy, Difficulty.Easy);
        easyEasyPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        easyEasyPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();

        difficulties = new Difficulties(Difficulty.Medium, Difficulty.Medium);
        mediumMediumPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        mediumMediumPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();

        difficulties = new Difficulties(Difficulty.Hard, Difficulty.Hard);
        hardHardPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        hardHardPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();

        difficulties = new Difficulties(Difficulty.Easy, Difficulty.Medium);
        easyMediumPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        easyMediumPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();

        difficulties = new Difficulties(Difficulty.Easy, Difficulty.Hard);
        easyHardPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        easyHardPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();

        difficulties = new Difficulties(Difficulty.Medium, Difficulty.Hard);
        mediumHardPlayer1Wins.text = Statistics.GetScore(PlayerName.Player1, difficulties).ToString();
        mediumHardPlayer2Wins.text = Statistics.GetScore(PlayerName.Player2, difficulties).ToString();
    }
}
