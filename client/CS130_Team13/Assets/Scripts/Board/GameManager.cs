using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    void Start()
    {
        int seed = (int)Random.Range(0,100);
        boardManager.CreateBoard(seed);
    }
}
