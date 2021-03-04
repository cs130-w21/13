using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The Game Manager handles all of the server interaction and initializes 
/// turns which are handled by the Board Manager.
public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    private RemoteControllerTEMP rc;
    void Start()
    {
        // Temporary setup for demoing
        rc = this.gameObject.AddComponent<RemoteControllerTEMP>();
        //rc.GameFound();
        int seed = (int)Random.Range(0, 100);
        boardManager.CreateBoard(0);
        boardManager.RunTurn("MPMFRMFMFMFLMFMFMPBPBPB", "LLMFRMFMFMFLMFMFMPBPBPB");
        /*
        for 10 turns

        // TODO: Implement await function for recieving opponent command

        
        */
    }

    private void RunGame()
    {
        for (int i = 0; i < Constants.Game.MAX_TURNS; i++)
        {
            // Get client's input

            // 
            boardManager.RunTurn("MPMFRMFMFMFLMFMFMPBPBPB", "LLMFRMFMFMFLMFMFMPBPBPB");

        }
    }
}
