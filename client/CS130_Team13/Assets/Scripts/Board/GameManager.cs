using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The Game Manager handles all of the server interaction and initializes 
/// turns which are handled by the Board Manager.
public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    void Start()
    {
        // Temporary setup for demoing
        int seed = (int)Random.Range(0, 100);
        boardManager.CreateBoard(0);
        boardManager.RunTurn("MPMFRMFMFMFLMFMFMPBPBPB", "LLMFRMFMFMFLMFMFMPBPBPB");
    }
}
