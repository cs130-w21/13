using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForLoopBlock : PanelItem, ISubPanel
{
    [SerializeField]
    private GameObject myPanel = null;
    
    public void IsOccupied() {
        // simulate a guard probe event
        myPanel.GetComponent<CodingPanel>().ReportGuardProbe();
    }

    public void ItemCame(GameObject newItem) {
        myPanel.GetComponent<CodingPanel>().PutItem(newItem);
    }
}
