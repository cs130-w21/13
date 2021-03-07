using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    public void UpdateVolume(float vol) {
        SettingsHub.instance.UpdateVolume(vol);
    }
}
