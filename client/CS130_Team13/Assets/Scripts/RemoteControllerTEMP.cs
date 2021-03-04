// unity c# code
using Socket.Quobject.SocketIoClientDotNet.Client;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/////// USE THIS FOR CLIENT COMMUNICATION FOR NOW :)
public class RemoteController {
    public RemoteController (string name){}
    public void SendPlayerCommands_ToServer(string commands) {}
    public void EndCurrentGame_ToServer() {}

    private string opponentCommands;
    private bool gameStarted = false;

    public bool GetGameStarted() {
        return gameStarted;
    }

    public int GetPlayerOrder() {
        return 1;
    }

    /// <summary>
    /// Returns the opponent's current commands
    /// Returns NULL if opponents have not submitted their commands, or there are no new commands
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public string getOpponentCommands() {
        return "*";
    }
}

//////// CLIENT PEEPS LOOK NO FURTHER :0

// serializable means it can be converted to JSON info easily 
// https://docs.unity3d.com/Manual/JSONSerialization.html
[System.Serializable]
public class UserInfo {
    public string name;
    public int id;
    public int playerOrder;

    public UserInfo(string name, int id) {
        this.name = name;
        this.id = id;
        this.playerOrder = 0;
    }

    public override string ToString() {
        return "name: " + name + ", id: " + id;
    }
}

[System.Serializable]
public class TurnInfo
{
    public string commandString;
    public int playerId;

    public TurnInfo(string commandString, int playerId)
    {
        this.commandString = commandString;
        this.playerId = playerId;
    }
}

public class RemoteControllerTEMP : MonoBehaviour {
    private QSocket socket;
    private UserInfo userInfo;
    private UserInfo opponentInfo;
    private TurnInfo turnInfo;

    // TODO: have non-cardcoded usernames
    private string username = "pickles";
    private const string SERVER_URL = "http://localhost:3000";
    private string opponentCommands;
    private bool gameStarted = false; 

    // TODO: write a constructor that can pass in the string username

    void Start() {
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame() {
        int id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(SERVER_URL + "/user_id")) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            string result = webRequest.downloadHandler.text;
            if (result.Length <= 0) {
                throw new Exception(webRequest.error);
            }
            id = System.Convert.ToInt32(result);
            Debug.Log(webRequest.downloadHandler.text);
        }
        // set User Info
        this.userInfo = new UserInfo(username, id);
        setupSocket();

        // TODO send a turn
        for(int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(3);
            SendPlayerCommands_ToServer("F");
        }
        EndCurrentGame_ToServer();
    }

    private void setupSocket() {
        socket = IO.Socket(SERVER_URL);

        // this happens on connect
        socket.On(QSocket.EVENT_CONNECT, () => {
            Debug.Log("Connected to server");

            // Send user info to the server
            // The "hello" event is how a user first contacts the server to connect to a game
            socket.Emit("hello", JsonUtility.ToJson(userInfo));
        });

        // Server sends an initiate event if the player successfully connects
        // Server sends a message saying if the user is player 1 or players 2
        // data is an integer with the player's order number (1 or 2)
        socket.On("initiate", (data) => {
            Debug.Log("I am player " + data);
            userInfo.playerOrder = System.Convert.ToInt32(data);
        });

        // We known a game is found when the server sends the first "gameplay" event
        // We retreive the opponent info from this
        socket.On("gameplay", (data) => {
            Debug.Log("Player " + userInfo.playerOrder 
                + " sees that the game has started");
            gameStarted = true;
            Debug.Log(data);
            opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
            Debug.Log(opponentInfo.ToString());
            opponentCommands = null;
        });

        // We receive opponent's commands from the server
        socket.On("receiveTurn", (data) => {
            Debug.Log("Player " + userInfo.playerOrder + 
                " has received the opponent's turn data");
            opponentCommands = "" + data;
        });

        // End the current game.
        // Data should contain an empty string uder current implementation
        socket.On("endGameConfirmation", (data) => {
            Debug.Log("Player " + userInfo.playerOrder + 
                " has received the instruction to end the game");
            Destroy();
        });
    }

    private void Destroy() {
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
    public void SendPlayerCommands_ToServer(string commands) {
        // Create a TurnInfo object
        Debug.Log("Player " + userInfo.playerOrder + " is submitting their turn");
        this.turnInfo = new TurnInfo(commands, this.userInfo.id);
        socket.Emit("submittingTurn", JsonUtility.ToJson(turnInfo));
    }

    /// <summary>
    /// Connect to the server and tell it that the game has ended.
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public void EndCurrentGame_ToServer() {
        // According to our current specs, either user can end a game at any time
        // If that were to change, we can adjust the event to send verification, etc.
        Debug.Log("Client is ending game end request");
        socket.Emit("endGameRequest", "");
    }

    /// <summary>
    /// Returns the opponent's current commands
    /// Returns NULL if opponents have not submitted their commands, or there are no new commands
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public string GetOpponentCommands() {
        string temp = opponentCommands;
        if(temp != null && temp != "*") {
            opponentCommands = null;
            return temp;
        }
        return null;
    }

    /// <summary>
    /// Returns a boolean that describes if the game has started
    ///     false: client has been initialized but have not connected to server 
    ///         or no opponent has been found yet
    ///     true: client has received gameplay event, game has started
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public bool GetGameStarted() {
        return gameStarted;
    }

    /// <summary>
    /// Returns an int that describes the player's order number
    ///      0 : the players is not a valid player 1 or player 2, likely because there are already 2 players
    ///      1 : this user is player 1 
    ///      2 : this user is player 2 
    /// Should not be called unless the game has started (GetGameStarted returns true)
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public int GetPlayerOrder() {
        return userInfo.playerOrder;
    }
}
