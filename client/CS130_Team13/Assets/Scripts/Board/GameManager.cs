using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// The Game Manager handles all of the server interaction and initializes 
/// turns which are handled by the Board Manager.
public class GameManager : MonoBehaviour
{
    // Private enum for game state machine
    public enum GameState
    {
        Connecting,
        WaitingForOpponent,
        StartGame,
        CodingPhase,
        AwaitingOpponentCommands,
        ExecutionPhase,
        EndGame
    }

    public BoardManager bm;
    private Opponent rc;
    public CodingPanel cp;
    private CodeProcessor codeProcessor;
    private Coroutine gameTimeout;

    public GameObject ConnectingUI;
    public InputField nameInput;
    private bool nameSubmitted = false;
    public GameObject WaitingUI;
    public GameObject GameOverUI;
    public GameObject CodingUI;
    public GameObject SubmitButton;

    [HideInInspector]
    public bool isRunningTurn { get; set; } = false;

    private string playerName;
    private int myPlayerOrder = 0;
    private double seed = 0f;
    private GameState currentState;
    private GameState previousState;
    private int p1Score = 0;
    private int p2Score = 0;
    private int currentTurn = 0;
    private float turnStartTime = -1 * Constants.Game.TURN_DURATION_SECS;
    private float timeRemaining;
    private string clientCmd = null;
    private string opponentCmd = null;
    private const int SECONDS_TO_OPPONENT_TIMEOUT = 45;
    private bool codeSubmitted = false;

    public void SetCurrentState(GameState state)
    {
        currentState = state;
        if (rc != null)
            rc.SetCurrentState(state);
    }
    void Start()
    {
        codeProcessor = new CodeProcessor();
        SetCurrentState(GameState.Connecting);
        currentTurn = 1;
        gameTimeout = null;
    }

    // Called every frame. Used to manage the state machine.
    void Update()
    {
        switch (currentState)
        {
            case GameState.Connecting:
                {
                    // Name submitted in the UI
                    if (nameSubmitted)
                    {
                        // Initialize the server connection
                        playerName = nameInput.text;
                        Debug.Log("Submitted name " + playerName);
                        rc = new RemoteController();
                        StartCoroutine(rc.InitializeGame(playerName));
                        Debug.Log("Initializing connection");
                        // Toggle UI                  
                        ConnectingUI.SetActive(false);
                        WaitingUI.SetActive(true);
                        SetCurrentState(GameState.WaitingForOpponent);
                    }
                    previousState = GameState.Connecting;
                    break;
                }
            case GameState.WaitingForOpponent:
                {
                    if (rc.GetGameStarted())
                    {
                        // Game was found, so initialize the board
                        WaitingUI.SetActive(false);
                        SetCurrentState(GameState.StartGame);
                    }
                    previousState = GameState.WaitingForOpponent;
                    break;
                }
            case GameState.StartGame:
                {
                    myPlayerOrder = rc.GetPlayerNumber();
                    seed = rc.GetRandomSeed();
                    bm.CreateBoard(this, (int)(seed * 1000000000));
                    turnStartTime = Time.time;
                    CodingUI.SetActive(true);
                    SubmitButton.SetActive(true);
                    SetCurrentState(GameState.CodingPhase);
                    previousState = GameState.StartGame;
                    codeSubmitted = false;
                    break;
                }
            case GameState.CodingPhase:
                {
                    if (rc.GetGameEnded())
                    {
                        EndGame();
                    }
                    // Submit the command if the timer runs out or if the player hits submit
                    if (turnStartTime + Constants.Game.TURN_DURATION_SECS < Time.time || codeSubmitted)
                    {
                        codeSubmitted = false;
                        clientCmd = cp.GetInformation();
                        Debug.Log(clientCmd);
                        //clientCmd = "MFMF";
                        rc.SendPlayerCommands_ToServer(clientCmd);
                        WaitingUI.SetActive(true);
                        SubmitButton.SetActive(false);
                        SetCurrentState(GameState.AwaitingOpponentCommands);
                    }
                    previousState = GameState.CodingPhase;
                    break;
                }
            case GameState.AwaitingOpponentCommands:
                {
                    if (rc.GetGameEnded())
                    {
                        EndGame();
                    }
                    opponentCmd = rc.GetOpponentCommands();
                    if (opponentCmd == null)
                    {
                        break;
                    }
                    else
                    {
                        WaitingUI.SetActive(false);
                        Debug.Log("Opponent cmd: " + opponentCmd);
                        SetCurrentState(GameState.ExecutionPhase);
                    }

                    previousState = GameState.AwaitingOpponentCommands;
                    break;
                }
            case GameState.ExecutionPhase:
                {
                    if (rc.GetGameEnded())
                    {
                        EndGame();
                    }
                    if (previousState != GameState.ExecutionPhase)
                    {
                        if (gameTimeout != null)
                            StopCoroutine(gameTimeout);
                        gameTimeout = StartCoroutine(endGameInAMinute());
                        // Assume Player 1, swap if not
                        string p1Cmd;
                        string p2Cmd;
                        p1Cmd = (myPlayerOrder == 1) ? clientCmd : opponentCmd;
                        p2Cmd = (myPlayerOrder == 1) ? opponentCmd : clientCmd;
                        p1Cmd = codeProcessor.GetResult(p1Cmd);
                        p2Cmd = codeProcessor.GetResult(p2Cmd);
                        isRunningTurn = true;
                        Debug.Log("Turn starting");
                        StartCoroutine(bm.RunTurn(p1Cmd, p2Cmd));

                    }
                    else if (!isRunningTurn)
                    {
                        Debug.Log("Turn over");

                        if (currentTurn >= Constants.Game.MAX_TURNS)
                        {
                            EndGame();
                        }
                        else
                        {
                            currentTurn++;
                            turnStartTime = Time.time;
                            SubmitButton.SetActive(true);
                            SetCurrentState(GameState.CodingPhase);
                        }
                    }
                    previousState = GameState.ExecutionPhase;
                    break;
                }
            case GameState.EndGame:
                {
                    if (previousState != GameState.EndGame)
                    {
                        rc.EndCurrentGame_ToServer();
                    }
                    GameOverUI.SetActive(true);
                    previousState = GameState.EndGame;
                    rc = null;
                    break;
                }
            default:
                break;
        }

    }

    void OnDestroy()
    {
      if (rc != null)
        rc.Destroy();
    }

    public IEnumerator endGameInAMinute()
    {
        yield return new WaitForSeconds(Constants.Game.SECONDS_TO_OPPONENT_TIMEOUT);
        EndGame();
        //SetCurrentState(GameState.EndGame);
    }

    // private void RunGame()
    // {
    //     string ci;
    //     for (int i = 0; i < Constants.Game.MAX_TURNS; i++)
    //     {


    //         // Get client's input
    //         ci = cp.GetComponent<CodingPanel>().GetInformation();
    //         Debug.Log(ci);
    //         // 
    //         bm.RunTurn("", "");

    //     }
    // }

    public void EndGame()
    {
        CodingUI.SetActive(false);
        WaitingUI.SetActive(false);
        SetCurrentState(GameState.EndGame);
    }

    /// Updates the player score. 1 = player 1, 2 = player 2.
    public int UpdateScore(int player, int scoreChange)
    {
        // Player 1
        int updatedScore = 0;
        if (player == 1)
        {
            p1Score += scoreChange;
            updatedScore = p1Score;
        }
        // Player 2
        else if (player == 2)
        {
            p2Score += scoreChange;
            updatedScore = p2Score;
        }
        if (updatedScore > Constants.Game.TARGET_SCORE)
        {
            EndGame();
        }
        return updatedScore;
    }

    /// Returns the score of a given player.
    public int GetScore(int player)
    {
        // Please just ignore this awful code
        return player == 1 ? p1Score : player == 2 ? p2Score : 0;
    }

    public float getTimeRemaining()
    {
        return turnStartTime + Constants.Game.TURN_DURATION_SECS - Time.time;
    }

    public int getP1Score()
    {
        return p1Score;
    }

    public int getP2Score()
    {
        return p2Score;
    }

    public void SubmitCode()
    {
        codeSubmitted = true;
    }

    public void SetNameSubmitted(bool b)
    {
        if (nameInput.text != "" && nameInput.text != null)
        {
            nameSubmitted = b;
        }
    }

}
