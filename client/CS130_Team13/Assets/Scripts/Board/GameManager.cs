using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public BoardManager boardManager;
    void Start()
    {
        int seed = (int)Random.Range(0,100);
        boardManager.CreateBoard(seed);
    }
}
