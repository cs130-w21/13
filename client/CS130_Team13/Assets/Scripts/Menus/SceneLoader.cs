using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    // scene management
    [SerializeField]
    private string menuScene, gameScene;
    private string currentScene;

    // system messages /////////////////////////////////////////////////
    void Awake() {
        // singleton instantiation
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // scene loading /////////////////////////////////////////////////
    /// <summary>
    /// Load a scene with specific name
    /// </summary>
    /// <param name="sceneName">Name of the scene to swtich to</param>
    public void ToSpecificScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame() {
        ToSpecificScene(gameScene);
    }
}
