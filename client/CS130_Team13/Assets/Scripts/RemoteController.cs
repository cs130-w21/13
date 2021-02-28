// unity c# code
using Socket.Quobject.SocketIoClientDotNet.Client;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

public class RemoteController : MonoBehaviour {
    private QSocket socket;
    private UserInfo userInfo;
    private UserInfo opponentInfo;

    void Start() {
        StartCoroutine(InitializeGame());
    }

    IEnumerator InitializeGame() {
        int id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:3000/user_id")) {
            // TODO: ERROR CHECK
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            id = System.Convert.ToInt32(webRequest.downloadHandler.text);
            Debug.Log(webRequest.downloadHandler.text);
        }
        // set User Info
        userInfo = new UserInfo("pickles", id);
        setupSocket();
    }

    private void setupSocket() {
        socket = IO.Socket("http://localhost:3000");

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
            // TODO switch to socket.id use
            Debug.Log(data);
            opponentInfo = JsonUtility.FromJson<UserInfo>(data.ToString());
            Debug.Log(opponentInfo.ToString());
        });
    }

    private void Destroy() {
        Debug.Log("byeee");
        socket.Disconnect();
    }
}
