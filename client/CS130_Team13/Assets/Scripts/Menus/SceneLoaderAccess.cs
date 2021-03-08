using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderAccess : MonoBehaviour
{
    public void ToMenuScene() {
        SceneLoader.instance.GackToMenu();
    }

    public void ToGameScene() {
        SceneLoader.instance.StartGame();
    }

    public void EndGame() {
        Debug.Log("exit...");
        Application.Quit();
    }
}
