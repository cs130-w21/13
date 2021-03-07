using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHub : MonoBehaviour
{
    public static SettingsHub instance = null;

    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private float currentVolume = 0;
    
    public void UpdateVolume(float newValue) {

    }

    /// <summary>
    /// Return the current volume percentage (0 - 1)
    /// </summary>
    /// <returns>float: the percentage of max volume</returns>
    public float GetVolume() {
        return currentVolume;
    }
}
