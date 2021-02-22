using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoPanelPlug : ProtoDraggable, ICodeInfo
{
    [SerializeField]
    private string myInformation = "";
    
    public string GetInformation() {
        return myInformation;
    }
}
