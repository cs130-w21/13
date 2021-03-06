// unity c# code
using Socket.Quobject.SocketIoClientDotNet.Client;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/////// USE THIS FOR CLIENT COMMUNICATION FOR NOW :)
<<<<<<< HEAD
public interface Opponent
{
  void SendPlayerCommands_ToServer(string commands);
  void EndCurrentGame_ToServer();
  bool GetGameStarted();
  int GetPlayerNumber();
  string GetOpponentCommands();
  double GetRandomSeed();
  bool GetGameEnded();
}
//////// CLIENT PEEPS LOOK NO FURTHER :0

namespace RemoteControllerNamespace
{
  public class RemoteController : Opponent
=======
public class RemoteController
{
  public RemoteController(string name) { }
  public void SendPlayerCommands_ToServer(string commands) { }
  public void EndCurrentGame_ToServer() { }

  private string opponentCommands;
  private bool gameStarted = false;

  public bool GetGameStarted()
  {
    return gameStarted;
  }

  public int GetPlayerOrder()
  {
    return 1;
  }

  /// <summary>
  /// Returns the opponent's current commands
  /// Returns NULL if opponents have not submitted their commands, or there are no new commands
  ///
  /// This should be called by C# functions within Unity, and it should call
  /// Socket.IO-related functions within RemoteController.
  /// </summary>
  public string getOpponentCommands()
  {
    return "*";
  }
}
  //////// CLIENT PEEPS LOOK NO FURTHER :0

namespace RemoteControllerNamespace
{
  public class RemoteControllerTEMP : MonoBehaviour
>>>>>>> master
  {
    private QSocket socket;
    private UserInfo userInfo;
    private UserInfo opponentInfo;
    private TurnInfo turnInfo;
<<<<<<< HEAD
    private string username;
    private const string SERVER_URL = "https://cs130-hacman.herokuapp.com";
    //http://localhost:3000
    private string opponentCommands;
    private bool gameStarted = false;
    private bool gameEnded = false;
    private double randomSeed;

    public RemoteController(string name)
    {
      username = name;
      InitializeGame();
=======

    // TODO: have non-hardcoded usernames
    private string username = "pickles";
    private const string SERVER_URL = "http://localhost:3000";
    private string opponentCommands;
    private bool gameStarted = false;

    void Start()
    {
      StartCoroutine(InitializeGame());
>>>>>>> master
    }

    IEnumerator InitializeGame()
    {
      int id;
      // Request and wait for the desired page.
      using (UnityWebRequest webRequest = UnityWebRequest.Get(SERVER_URL + "/user_id"))
      {
        yield return webRequest.SendWebRequest();
        string result = webRequest.downloadHandler.text;
        if (result.Length <= 0)
        {
          throw new Exception(webRequest.error);
        }
        id = System.Convert.ToInt32(result);
        Debug.Log(webRequest.downloadHandler.text);
      }
      // set User Info
      this.userInfo = new UserInfo(username, id);

      setupSocket();
<<<<<<< HEAD
=======

      // TODO send a turn
      for (int i = 0; i < 15; i++)
      {
        yield return new WaitForSeconds(20);
        SendPlayerCommands_ToServer("F");
      }
      EndCurrentGame_ToServer();
>>>>>>> master
    }

    private void setupSocket()
    {
      socket = IO.Socket(SERVER_URL);

      // this happens on connect
      socket.On(QSocket.EVENT_CONNECT, () =>
      {
        Debug.Log("Connected to server");
        // The "hello" event is how a user first contacts the server to connect to a game
        socket.Emit("hello", JsonUtility.ToJson(userInfo));
      });

      // Server sends an pairing event if the player successfully connects
      // Justification for change: in multigames, won't really know what player number you are until game starts
      socket.On("pairing", (data) =>
      {
        Debug.Log(data + " received from pairing");
      });

      // We know a game is found when the server sends the first "gameplay" event
      // We retreive the opponent info from this
      socket.On("gameplay", (data) =>
      {
<<<<<<< HEAD
        gameStarted = true;
        opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
        userInfo.playerNumber = (opponentInfo.playerNumber == 1) ? 2 : 1;
        Debug.Log("Player " + userInfo.playerNumber
            + " sees that the game has started");
        randomSeed = userInfo.randomSeed;
=======
        Debug.Log("SEEEE");
        gameStarted = true;
        opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
        userInfo.playerOrder = (opponentInfo.playerOrder == 1) ? 2 : 1;
        Debug.Log("Player " + userInfo.playerOrder
            + " sees that the game has started");
>>>>>>> master
      });

      // We receive opponent's commands from the server
      socket.On("receiveTurn", (data) =>
      {
        TurnInfo opponentTurnInfo = JsonUtility.FromJson<TurnInfo>(data.ToString());
<<<<<<< HEAD
        Debug.Log("Player " + userInfo.playerNumber +
=======
        Debug.Log("Player " + userInfo.playerOrder +
>>>>>>> master
            " has received the opponent's turn data");
        opponentCommands = opponentTurnInfo.commands;
      });

      // End the current game.
      // Data should contain an empty string uder current implementation
      socket.On("endGameConfirmation", (data) =>
      {
<<<<<<< HEAD
        gameEnded = true;
        Debug.Log("Player " + userInfo.playerNumber +
=======
        Debug.Log("Player " + userInfo.playerOrder +
>>>>>>> master
            " has received the instruction to end the game");
        Destroy();
      });
    }

    private void Destroy()
    {
      Debug.Log("User is disconnecting from socket");
      socket.Disconnect();
    }

    // --------------------------------------------------------------------------------
    // The following functions should not do anything other than call other functions
    // present elsewhere in the code. They exist as an abstraction layer between
    // server and client: if the implementation of one is changed, then you'll have
    // to change some function names here -- but the benefit is that you _won't_ have
    // to go digging through the rest of the code to find all the references in the
    // other half of the project.
    // --------------------------------------------------------------------------------

    /// <summary>
    /// Send the current player's command inputs to the server.
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    /// <param name="commands">the player's commands, to be sent to the server</param>
    public void SendPlayerCommands_ToServer(string commands)
    {
      // Create a TurnInfo object
<<<<<<< HEAD
      Debug.Log("Player " + userInfo.playerNumber + " is submitting their turn");
=======
      Debug.Log("Player " + userInfo.playerOrder + " is submitting their turn");
>>>>>>> master
      this.turnInfo = new TurnInfo(commands, this.userInfo.id);
      socket.Emit("submittingTurn", JsonUtility.ToJson(turnInfo));
    }

    /// <summary>
    /// Connect to the server and tell it that the game has ended.
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public void EndCurrentGame_ToServer()
    {
      // According to our current specs, either user can end a game at any time
      // If that were to change, we can adjust the event to send verification, etc.
      Debug.Log("Client is ending game end request");
<<<<<<< HEAD
      socket.Emit("endGameRequest", JsonUtility.ToJson(this.userInfo));
=======
      socket.Emit("endGameRequest", "");
>>>>>>> master
    }

    /// <summary>
    /// Returns the opponent's current commands
    /// Returns NULL if opponents have not submitted their commands, or there are no new commands
    /// </summary>
    public string GetOpponentCommands()
    {
      string temp = opponentCommands;
      if (temp != null && temp != "null")
      {
        opponentCommands = null;
        return temp;
      }
      return null;
    }

    /// <summary>
    /// Returns a boolean that describes if the game has started
    ///     true: client has received gameplay event, game has started
    ///     false: client has been initialized but have not connected to server 
    ///         or no opponent has been found yet
    /// </summary>
    public bool GetGameStarted()
    {
      return gameStarted;
    }

    /// <summary>
    /// Returns an int that describes the player's order number
    ///      null : the players is not a valid player 1 or player 2, likely because there are already 2 players
    ///      1 : this user is player 1 
    ///      2 : this user is player 2 
    /// Should not be called unless the game has started (GetGameStarted returns true)
    /// </summary>
<<<<<<< HEAD
    public int GetPlayerNumber()
    {
      return userInfo.playerNumber;
    }
    public double GetRandomSeed()
    {
      return randomSeed;
    }
    public bool GetGameEnded()
    {
      return gameEnded;
=======
    public int GetPlayerOrder()
    {
      return userInfo.playerOrder;
>>>>>>> master
    }
  }
}
