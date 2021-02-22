using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoPanelGuard : MonoBehaviour, IDroppable {
    [SerializeField]
    private string myLayer;

    public ProtoCodingPanel myPanel;
    
    public GameObject GetCurrentItem() {
        throw new System.NotImplementedException();
    }

    public string GetLayer() {
        return myLayer;
    }

    public bool IsOccupied() {
        myPanel.ReportGuardProbe();
        return true;
    }

    public void ItemCame(GameObject item) {
        throw new System.NotImplementedException();
    }

    public void ItemLeft(GameObject item) {
        throw new System.NotImplementedException();
    }
}
