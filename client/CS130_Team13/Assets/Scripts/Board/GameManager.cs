using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The Game Manager handles all of the server interaction and initializes 
/// turns which are handled by the Board Manager.
public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    private RemoteControllerTEMP rc;
    private int p1Score;
    private int p2Score;
    void Start()
    {
        // Temporary setup for demoing
        rc = this.gameObject.AddComponent<RemoteControllerTEMP>();
        //rc.GameFound();
        int seed = (int)Random.Range(0, 100);
        boardManager.CreateBoard(this, seed);
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

    public void EndGame()
    {

    }

    /// Updates the player score. 1 = player 1, 2 = player 2.
    public int UpdateScore(int player, int scoreChange)
    {
        // Player 1
        if (player == 1)
        {
            p1Score += scoreChange;
            return p1Score;
        }
        // Player 2
        else if (player == 2)
        {
            p2Score += scoreChange;
            return p2Score;
        }
        return 0;
    }

    public int GetScore(int player)
    {
        return player == 1 ? p1Score : player == 2 ? p2Score : 0;
    }
}
