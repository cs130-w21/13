using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    // Start is called before the first frame update
    public Text p1;
    public Text p2;
    public Text timer;

    public GameManager gameManager;

    // Update is called once per frame
    void Update()
    {
        p1.text = gameManager.getP1Score().ToString();
        p2.text = gameManager.getP2Score().ToString();
        timer.text = Mathf.Max(0,Mathf.Ceil(gameManager.getTimeRemaining())).ToString();
    }
}
