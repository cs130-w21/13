// unity c# code
using Socket.Quobject.SocketIoClientDotNet.Client;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/////// USE THIS FOR CLIENT COMMUNICATION FOR NOW :)
public interface Opponent
{
  IEnumerator InitializeGame(string name);
  void SendPlayerCommands_ToServer(string commands);
  void EndCurrentGame_ToServer();
  bool GetGameStarted();
  int GetPlayerNumber();
  string GetOpponentCommands();
  string GetOpponentName();
  double GetRandomSeed();
  bool GetGameEnded();
  void SetCurrentState(GameManager.GameState state);
  void Destroy();
}
//////// CLIENT PEEPS LOOK NO FURTHER :0

public class RemoteController : Opponent
{
  private QSocket socket;
  private UserInfo userInfo;
  private UserInfo opponentInfo;
  private bool gameStarted = false;
  private bool gameEnded = false;
  ~RemoteController()
  {
    Destroy();
  }

  public IEnumerator InitializeGame(string name)
  {
    int id;
    // Request and wait for the desired page.
    using (UnityWebRequest webRequest = UnityWebRequest.Get(Constants.Server.SERVER_URL + "/user_id"))
    {
      yield return webRequest.SendWebRequest();
      string result = webRequest.downloadHandler.text;
      if (result.Length <= 0)
      {
        throw new Exception(webRequest.error);
      }
      Debug.Log(webRequest.downloadHandler.text);
      id = System.Convert.ToInt32(result);
    }
    // set User Info
    this.userInfo = new UserInfo(name, id);

    setupSocket();
  }

  private void setupSocket()
  {
    socket = IO.Socket(Constants.Server.SERVER_URL);

    // this happens on connect
    socket.On(QSocket.EVENT_CONNECT, () =>
    {
      Debug.Log("Connected to server");
      // The "hello" event is how a user first contacts the server to connect to a game
      if (userInfo.state != GameManager.GameState.AwaitingOpponentCommands)
        socket.Emit("hello", JsonUtility.ToJson(userInfo.exportConnectToServerRequiredInfo()));
      else
        socket.Emit("submittingTurn", JsonUtility.ToJson(userInfo.exportTurnInfo()));
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
      if (gameStarted == false)
      {
        gameStarted = true;
        opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
        userInfo.playerNumber = (opponentInfo.playerNumber == 1) ? 2 : 1;
        Debug.Log("Player " + userInfo.playerNumber
              + " sees that the game has started");
      }
    });

    // We receive opponent's commands from the server
    socket.On("receiveTurn", (data) =>
    {
      TurnInfo opponentTurnInfo = JsonUtility.FromJson<TurnInfo>(data.ToString());
      Debug.Log("Player " + userInfo.playerNumber +
            " has received the opponent's turn data");
      Debug.Log(data.ToString());
      if (opponentTurnInfo.commandsUpdated != null &&
        !String.Equals(opponentTurnInfo.commandsUpdated, opponentInfo.commandsUpdated))
        opponentInfo.setCommands(opponentTurnInfo.commands, opponentTurnInfo.commandsUpdated);
    });

    // There is an error. End game.
    socket.On("client error", (data) =>
    {
      gameEnded = true;
      Debug.Log(data.ToString());
      Destroy();
    });

    // End the current game.
    // Data should contain an empty string uder current implementation
    socket.On("endGameConfirmation", (data) =>
    {
      gameEnded = true;
      Debug.Log("Player " + userInfo.playerNumber +
            " has received the instruction to end the game");
      Destroy();
    });

        // this happens on disconnect
    socket.On(QSocket.EVENT_DISCONNECT, () =>
    {
      Debug.Log("Disconnected from server");
    });

  }

  public void Destroy()
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
    Debug.Log("Player " + userInfo.playerNumber + " is submitting their turn");
    userInfo.setCommands(commands);
    Debug.Log(userInfo.commandsUpdated);
    socket.Emit("submittingTurn", JsonUtility.ToJson(userInfo.exportTurnInfo()));
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
    socket.Emit("endGameRequest", JsonUtility.ToJson(this.userInfo.exportConnectToServerRequiredInfo()));
  }

  /// <summary>
  /// Returns the opponent's current commands
  /// Returns NULL if opponents have not submitted their commands, or there are no new commands
  /// </summary>
  public string GetOpponentCommands()
  {
    string temp = opponentInfo.getCommands();
    string temp2 = userInfo.getCommands();
    if (temp != null && temp != "null" && temp2 != null && temp2 != "null")
    {
      opponentInfo.setCommands(null);
      userInfo.setCommands(null);
      return temp;
    }
    return null;
  }

  public string GetOpponentName()
  {
    return opponentInfo.name;
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
  public int GetPlayerNumber()
  {
    return userInfo.playerNumber;
  }
  public double GetRandomSeed()
  {
    return userInfo.randomSeed;
  }
  public bool GetGameEnded()
  {
    return gameEnded;
  }

  public void SetCurrentState(GameManager.GameState state)
  {
    if (userInfo != null)
      userInfo.state = state;
  }
}
