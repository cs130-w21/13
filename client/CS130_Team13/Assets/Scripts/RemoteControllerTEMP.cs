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

    ////// TODO CLIENT PEEPS: FILL IN WHAT SHOULD HAPPEN IF Game Found, Received Commands, and Game Ended.
    private void GameFound_FromServer() {}
    private void ReceiveOpponentCommands_FromServer(string commands) {}
    private void GameEnded_FromServer() {}
}

//////// CLIENT PEEPS LOOK NO FURTHER :0

// serializable means it can be converted to JSON info easily 
// https://docs.unity3d.com/Manual/JSONSerialization.html
[System.Serializable]
public class UserInfo {
    public string name;
    public int id;

    public UserInfo(string name, int id) {
        this.name = name;
        this.id = id;
    }

    public override string ToString() {
        return "name: " + name + ", id: " + id;
    }
}

public class RemoteControllerTEMP : MonoBehaviour {
    private QSocket socket;
    private UserInfo userInfo;
    private UserInfo opponentInfo;

    private string username = "pickles";
    private const string SERVER_URL = "http://localhost:3000";

    // public RemoteController(string username) {
    //     // TODO: Change the hardcoded values above.
    // }

    void Start() {
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame() {
        int id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(SERVER_URL + "/user_id")) {
            // Request and wait for the desired page.
            id = 0;
            yield return webRequest.SendWebRequest();
            string result = webRequest.downloadHandler.text;
            if (result.Length <= 0) {
                throw new Exception(webRequest.error);
            }
            id = System.Convert.ToInt32(result);
            Debug.Log(webRequest.downloadHandler.text);
        }
        // set User Info
        this.userInfo = new UserInfo(username, id);;
        setupSocket();
    }

    private void setupSocket() {
        socket = IO.Socket(SERVER_URL);

        // this happens on connect
        socket.On(QSocket.EVENT_CONNECT, () => {
            Debug.Log("Connected");
            // sends server the user info
            socket.Emit("hello", JsonUtility.ToJson(userInfo));
        });

        // this happens when server emits to event called initiate
        socket.On("initiate", (data) => {
            Debug.Log(data);
        });

        socket.On("gameplay", (data) => {
            Debug.Log(data);
            opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
            Debug.Log(opponentInfo.ToString());
        });
    }

    private void Destroy() {
        Debug.Log("byeee");
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
    /// Start a new game.
    ///
    /// This should be called by the Socket.IO-related functions within RemoteController,
    /// and it should call C# functions within Unity.
    /// </summary>
    private void GameFound_FromServer() {
        // TODO
    }

    /// <summary>
    /// Send the current player's command inputs to the server.
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    /// <param name="commands">the player's commands, to be sent to the server</param>
    public void SendPlayerCommands_ToServer(string commands) {
        // TODO
    }

    /// <summary>
    /// Process the opponent's commands.
    ///
    /// This should be called by the Socket.IO-related functions within RemoteController,
    /// and it should call C# functions within Unity.
    /// </summary>
    /// <param name="commands">the opponent's commands, received from the server</param>
    private void ReceiveOpponentCommands_FromServer(string commands) {
        // TODO
    }

    /// <summary>
    /// Connect to the server and tell it that the game has ended.
    ///
    /// This should be called by C# functions within Unity, and it should call
    /// Socket.IO-related functions within RemoteController.
    /// </summary>
    public void EndCurrentGame_ToServer() {
        // TODO
    }

    /// <summary>
    /// End the current game.
    ///
    /// This should be called by the Socket.IO-related functions within RemoteController,
    /// and it should call C# functions within Unity.
    /// </summary>
    private void GameEnded_FromServer() {
        // TODO
    }
}
