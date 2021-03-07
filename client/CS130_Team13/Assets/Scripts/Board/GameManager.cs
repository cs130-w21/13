using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// The Game Manager handles all of the server interaction and initializes 
/// turns which are handled by the Board Manager.
public class GameManager : MonoBehaviour
{
    // Private enum for game state machine
    enum GameState
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
    private RemoteController rc;
    public CodingPanel cp;

    public GameObject ConnectingUI;
    public InputField nameInput;
    private bool nameSubmitted = false;
    public GameObject WaitingUI;
    public GameObject GameOverUI;

    [HideInInspector]
    public bool isRunningTurn { get; set; } = false;

    private string playerName;
    private int myPlayerOrder = 0;
    private int seed = 0;
    private GameState currentState;
    private GameState previousState;
    private int p1Score = 0;
    private int p2Score = 0;
    private int currentTurn = 0;
    private float turnStartTime = 0;


    private float timeRemaining;

    private string clientCmd = null;
    private string opponentCmd = null;
    void Start()
    {
        currentState = GameState.Connecting;
        currentTurn = 1;
        // Get player name
        //playerName = "player";
        //rc = new RemoteController(playerName);



        //rc.GameFound();
        //int seed = Random.Range(0, 100);
        //boardManager.CreateBoard(this, seed);
        //bm.RunTurn("MPMFRMFMFMFLMFMFMPBPBPB", "MFRMFMFMFLMFMFMPBPBPB");
        /*
        for 10 turns

        // TODO: Implement await function for recieving opponent command

        
        */

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
                        playerName = nameInput.name;
                        Debug.Log("Submitted name " + playerName);
                        rc = new RemoteController(playerName);
                        StartCoroutine(rc.InitializeGame());
                        Debug.Log("Connected");
                        // Toggle UI                  
                        ConnectingUI.SetActive(false);
                        WaitingUI.SetActive(true);
                        currentState = GameState.WaitingForOpponent;
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
                        currentState = GameState.StartGame;
                    }
                    previousState = GameState.WaitingForOpponent;
                    break;
                }
            case GameState.StartGame:
                {
                    myPlayerOrder = rc.GetPlayerNumber();
                    seed = (int)rc.GetRandomSeed();
                    bm.CreateBoard(this, seed);
                    turnStartTime = Time.time;
                    currentState = GameState.CodingPhase;
                    previousState = GameState.StartGame;
                    break;
                }
            case GameState.CodingPhase:
                {
                    // Submit the command if the timer runs out or if the player hits submit
                    if (turnStartTime + Constants.Game.TURN_DURATION_SECS < Time.time)
                    {
                        //clientCmd = cp.GetInformation();
                        clientCmd = "MFMF";
                        rc.SendPlayerCommands_ToServer(clientCmd);
                        WaitingUI.SetActive(true);
                        currentState = GameState.AwaitingOpponentCommands;
                    }
                    previousState = GameState.CodingPhase;
                    break;
                }
            case GameState.AwaitingOpponentCommands:
                {
                    opponentCmd = rc.GetOpponentCommands();
                    if (opponentCmd == null)
                    {
                        break;
                    }
                    else
                    {
                        WaitingUI.SetActive(false);
                        currentState = GameState.ExecutionPhase;
                    }

                    previousState = GameState.AwaitingOpponentCommands;
                    break;
                }
            case GameState.ExecutionPhase:
                {

                    // Assume Player 1, swap if not
                    string p1Cmd;
                    string p2Cmd;
                    p1Cmd = (myPlayerOrder == 1) ? clientCmd : opponentCmd;
                    p2Cmd = (myPlayerOrder == 1) ? opponentCmd : clientCmd;

                    if (previousState != GameState.ExecutionPhase)
                    {
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
                            currentState = GameState.CodingPhase;
                        }
                    }
                    previousState = GameState.ExecutionPhase;
                    break;
                }
            case GameState.EndGame:
                {
                    GameOverUI.SetActive(true);
                    previousState = GameState.EndGame;
                    break;
                }
            default:
                break;
        }

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
        currentState = GameState.EndGame;
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

    // private IEnumerator AwaitOpponentCommand()
    // {
    //     string recievedCmd = rc.getOpponentCommands();
    //     while (recievedCmd == null)
    //     {
    //         yield return new WaitForSeconds(0.5f);
    //         recievedCmd = rc.getOpponentCommands();
    //     }
    //     opponentCmd = recievedCmd;
    // }

    public void SetNameSubmitted(bool b)
    {
        if (nameInput.text != "" && nameInput.text != null)
        {
            nameSubmitted = b;
        }
    }

}
