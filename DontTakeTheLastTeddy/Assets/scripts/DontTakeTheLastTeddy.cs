using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Game manager
/// </summary>
public class DontTakeTheLastTeddy : MonoBehaviour
{
    Board board;
    Player player1;
    Player player2;

    // The player is started the current game
    PlayerName started;

    // The difficulty combination of the current game
    Difficulties currentDifficulties;

    // events invoked by class
    TakeTurn takeTurnEvent = new TakeTurn();
    GameOver gameOverEvent = new GameOver();
    GameStarting gameStartingEvent = new GameStarting();

    // Controls the time between games
    Timer pauseBetweenGamesTimer;

    // count the number of games played
    int gameCounter = 0;

    // Will contain a list of the various difficulty combinations
    List<Difficulties> difficultyList = new List<Difficulties>();
    // controls the current difficult combination
    int difficultyIndex = 0;

    // Store the progress bar
    LineRenderer progressBar;

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    void Awake()
    {
        // retrieve board and player references
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();

        // add timer component
        pauseBetweenGamesTimer = gameObject.AddComponent<Timer>();
        pauseBetweenGamesTimer.Duration = GameConstants.PauseBetweenGamesSeconds;
        pauseBetweenGamesTimer.AddTimerFinishedListener(HandlePauseBetweenGamesTimerFinished);

        // Initialize Statistics
        Statistics.Initialize();

        // initialize possible difficult combinations
        difficultyList.Add(new Difficulties(Difficulty.Easy, Difficulty.Easy));
        difficultyList.Add(new Difficulties(Difficulty.Medium, Difficulty.Medium));
        difficultyList.Add(new Difficulties(Difficulty.Hard, Difficulty.Hard));
        difficultyList.Add(new Difficulties(Difficulty.Easy, Difficulty.Medium));
        difficultyList.Add(new Difficulties(Difficulty.Easy, Difficulty.Hard));
        difficultyList.Add(new Difficulties(Difficulty.Medium, Difficulty.Hard));

        // register as invoker and listener
        EventManager.AddTakeTurnInvoker(this);
        EventManager.AddGameOverInvoker(this);
        EventManager.AddGameStartingInvoker(this);
        EventManager.AddTurnOverListener(HandleTurnOverEvent);
    }

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
        // get the progress bar
        progressBar = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<LineRenderer>();

        // start with player1
        started = PlayerName.Player1;

        // start with the first difficulty combination
        currentDifficulties = difficultyList[0];

        // start the first game
        StartGame(started, currentDifficulties);
    }

    /// <summary>
    /// Adds the given listener for the TakeTurn event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddTakeTurnListener(UnityAction<PlayerName, Configuration> listener)
    {
        takeTurnEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the GameOver event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddGameOverListener(UnityAction<PlayerName, Difficulties> listener)
    {
        gameOverEvent.AddListener(listener);
    }


    /// <summary>
    /// Adds the given listener for the GameStarting event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddGameStartingListener(UnityAction listener)
    {
        gameStartingEvent.AddListener(listener);
    }

    /// <summary>
    /// Starts a game with the given player taking the
    /// first turn
    /// </summary>
    /// <param name="firstPlayer">player taking first turn</param>
    /// <param name="player1Difficulty">difficulty for player 1</param>
    /// <param name="player2Difficulty">difficulty for player 2</param>
    void StartGame(PlayerName firstPlayer, Difficulties difficulties)
    {
        gameCounter++;

        // set player difficulties
        player1.Difficulty = difficulties.DifficultyP1;
        player2.Difficulty = difficulties.DifficultyP2;

        // create new board
        board.CreateNewBoard();

        // take the current turn
        takeTurnEvent.Invoke(firstPlayer,
            board.Configuration);
    }

    /// <summary>
    /// Handles the TurnOver event by having the 
    /// other player take their turn
    /// </summary>
    /// <param name="player">who finished their turn</param>
    /// <param name="newConfiguration">the new board configuration</param>
    void HandleTurnOverEvent(PlayerName player, 
        Configuration newConfiguration)
    {
        // get the chosen configuration for the just finished turn
        board.Configuration = newConfiguration;

        // check for game over (if config is empty, all bears must have been picked)
        if (newConfiguration.Empty)
        {
            // fire event with winner
            if (player == PlayerName.Player1)
            {
                gameOverEvent.Invoke(PlayerName.Player2, currentDifficulties);
            }
            else
            {
                gameOverEvent.Invoke(PlayerName.Player1, currentDifficulties);
            }

            // pause before the next game
            pauseBetweenGamesTimer.Run();
        }
        else
        {
            // game not over, so give other player a turn
            if (player == PlayerName.Player1)
            {
                takeTurnEvent.Invoke(PlayerName.Player2,
                    newConfiguration);
            }
            else
            {
                takeTurnEvent.Invoke(PlayerName.Player1,
                    newConfiguration);
            }
        }
    }


    /// <summary>
    /// Handles the pause between games timer finishing
    /// </summary>
    void HandlePauseBetweenGamesTimerFinished()
    {
        // update the progress bar
        updateProgressBar();

        // check whether we need to move onto the next game
        if (gameCounter == GameConstants.MaxGames)
        {
            // if new game, need to reset game count
            gameCounter = 0;
            // and move to the next difficult combination
            difficultyIndex++;
        }

        // check if all games have completed
        if (difficultyIndex >= difficultyList.Count)
        {
            // if yes, load statistics scene
            SceneManager.LoadScene("statistics");
        }
        else
        {
            // if not, let listeners know that we are going to start a new game
            gameStartingEvent.Invoke();

            // alternate the starting player for each game
            started = started == PlayerName.Player1 ? PlayerName.Player2 : PlayerName.Player1;

            // get the difficulty combination for this game
            currentDifficulties = difficultyList[difficultyIndex];

            // start the new game
            StartGame(started, currentDifficulties);
        }
    }

    private void updateProgressBar()
    {
        // get current positions of the ends of the progress bar
        Vector3[] positions = new Vector3[progressBar.positionCount];
        progressBar.GetPositions(positions);
        // update the x value of one end by an appropriate value
        positions[1].x += (6f / (GameConstants.MaxGames * difficultyList.Count)); // 0.01f;
        // update the progress bar positions
        progressBar.SetPositions(positions);
    }
}
